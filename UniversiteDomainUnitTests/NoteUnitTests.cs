using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.NoteExceptions;
using UniversiteDomain.UseCases.NoteUseCases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversiteDomainUnitTests
{
    public class NoteUnitTests
    {
        private Mock<INoteRepository> _noteRepositoryMock;
        private Mock<IParcoursRepository> _parcoursRepositoryMock;
        private Mock<IRepositoryFactory> _repositoryFactoryMock;
        private AjouterNoteUseCase _ajouterNoteUseCase;

        [SetUp]
        public void Setup()
        {
            // Mock des repositories
            _noteRepositoryMock = new Mock<INoteRepository>();
            _parcoursRepositoryMock = new Mock<IParcoursRepository>();
            _repositoryFactoryMock = new Mock<IRepositoryFactory>();

            // Configuration de la factory pour retourner les repositories mockés
            _repositoryFactoryMock.Setup(f => f.NoteRepository()).Returns(_noteRepositoryMock.Object);
            _repositoryFactoryMock.Setup(f => f.ParcoursRepository()).Returns(_parcoursRepositoryMock.Object);

            // Initialisation du use case avec la factory mockée
            _ajouterNoteUseCase = new AjouterNoteUseCase(_repositoryFactoryMock.Object);
        }

        // ✅ TEST POUR L'AJOUT D'UNE NOTE VALIDE ✅
        [Test]
        public async Task ExecuteAsync_Should_Add_Note_When_Valid()
        {
            // Arrange
            long etudiantId = 1;
            long ueId = 2;
            float valeur = 15.5f;

            var parcours = new Parcours
            {
                Id = 1,
                NomParcours = "Parcours Informatique",
                AnneeFormation = 2025,
                UesEnseignees = new List<Ue> { new Ue { Id = ueId, NumeroUe = "UE1", Intitule = "Programmation avancée" } }
            };

            _parcoursRepositoryMock
                .Setup(repo => repo.GetParcoursByEtudiantAsync(etudiantId))
                .ReturnsAsync(parcours);

            _noteRepositoryMock
                .Setup(repo => repo.GetNoteByEtudiantAndUeAsync(etudiantId, ueId))
                .ReturnsAsync((Note?)null); // Aucune note existante

            _noteRepositoryMock
                .Setup(repo => repo.AddNoteAsync(It.IsAny<Note>()))
                .ReturnsAsync((Note note) => note);

            // Act
            var result = await _ajouterNoteUseCase.ExecuteAsync(etudiantId, ueId, valeur);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Valeur, Is.EqualTo(valeur));
            Assert.That(result.EtudiantId, Is.EqualTo(etudiantId));
            Assert.That(result.UeId, Is.EqualTo(ueId));

            _noteRepositoryMock.Verify(repo => repo.AddNoteAsync(It.IsAny<Note>()), Times.Once);
        }

        // ✅ TEST POUR UNE NOTE HORS DE LA PLAGE [0, 20] ✅
        [Test]
        public void ExecuteAsync_Should_Throw_InvalidNoteException_When_Note_Out_Of_Range()
        {
            // Arrange
            long etudiantId = 1;
            long ueId = 2;

            // Act & Assert (valeurs hors limites)
            Assert.ThrowsAsync<InvalidNoteException>(async () => await _ajouterNoteUseCase.ExecuteAsync(etudiantId, ueId, -5f));
            Assert.ThrowsAsync<InvalidNoteException>(async () => await _ajouterNoteUseCase.ExecuteAsync(etudiantId, ueId, 25f));
        }

        // ✅ TEST POUR UNE NOTE DÉJÀ EXISTANTE ✅
        [Test]
        public void ExecuteAsync_Should_Throw_DuplicateNoteException_When_Note_Already_Exists()
        {
            // Arrange
            long etudiantId = 1;
            long ueId = 2;
            float valeur = 14f;

            var existingNote = new Note { Id = 10, EtudiantId = etudiantId, UeId = ueId, Valeur = valeur };

            _noteRepositoryMock
                .Setup(repo => repo.GetNoteByEtudiantAndUeAsync(etudiantId, ueId))
                .ReturnsAsync(existingNote); // Simule une note existante

            // Act & Assert
            var exception = Assert.ThrowsAsync<DuplicateNoteException>(async () => await _ajouterNoteUseCase.ExecuteAsync(etudiantId, ueId, valeur));
            Assert.That(exception.Message, Is.EqualTo($"L'étudiant {etudiantId} a déjà une note dans l'UE {ueId}."));
        }

        // ✅ TEST POUR UN ÉTUDIANT NON INSCRIT DANS UN PARCOURS ✅
        [Test]
        public void ExecuteAsync_Should_Throw_UnauthorizedNoteException_When_Student_Has_No_Parcours()
        {
            // Arrange
            long etudiantId = 1;
            long ueId = 2;
            float valeur = 10f;

            _parcoursRepositoryMock
                .Setup(repo => repo.GetParcoursByEtudiantAsync(etudiantId))
                .ReturnsAsync((Parcours?)null); // L'étudiant n'a pas de parcours

            // Act & Assert
            var exception = Assert.ThrowsAsync<UnauthorizedNoteException>(async () => await _ajouterNoteUseCase.ExecuteAsync(etudiantId, ueId, valeur));
            Assert.That(exception.Message, Is.EqualTo($"L'étudiant {etudiantId} n'est inscrit dans aucun parcours."));
        }

        // ✅ TEST POUR UNE UE QUI N'EST PAS DANS LE PARCOURS DE L'ÉTUDIANT ✅
        [Test]
        public void ExecuteAsync_Should_Throw_UnauthorizedNoteException_When_Ue_Not_In_Student_Parcours()
        {
            // Arrange
            long etudiantId = 1;
            long ueId = 2; // Cette UE ne sera pas dans le parcours
            float valeur = 12f;

            var parcours = new Parcours
            {
                Id = 1,
                NomParcours = "Parcours Informatique",
                AnneeFormation = 2025,
                UesEnseignees = new List<Ue> // Cette liste ne contient PAS l'UE 2
                {
                    new Ue { Id = 3, NumeroUe = "UE3", Intitule = "Base de Données" }
                }
            };

            _parcoursRepositoryMock
                .Setup(repo => repo.GetParcoursByEtudiantAsync(etudiantId))
                .ReturnsAsync(parcours);

            // Act & Assert
            var exception = Assert.ThrowsAsync<UnauthorizedNoteException>(async () => await _ajouterNoteUseCase.ExecuteAsync(etudiantId, ueId, valeur));
            Assert.That(exception.Message, Is.EqualTo($"L'étudiant {etudiantId} ne peut pas être noté pour l'UE {ueId} car elle ne fait pas partie de son parcours."));
        }
    }
}
