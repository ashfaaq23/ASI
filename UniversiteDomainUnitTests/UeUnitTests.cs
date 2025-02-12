using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;
using UniversiteDomain.Exceptions.UeExceptions;
using UniversiteDomain.UseCases.ParcoursUseCases.UeDansParcours;
using UniversiteDomain.UseCases.UeUseCases.Create;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversiteDomainUnitTests
{
    public class UeUnitTests
    {
        private Mock<IUeRepository> _ueRepositoryMock;
        private Mock<IParcoursRepository> _parcoursRepositoryMock;
        private Mock<IRepositoryFactory> _repositoryFactoryMock;
        private AddUeDansParcoursUseCase _addUeUseCase;
        private CreateUeUseCase _createUeUseCase;

        [SetUp]
        public void Setup()
        {
            // Mock des repositories
            _ueRepositoryMock = new Mock<IUeRepository>();
            _parcoursRepositoryMock = new Mock<IParcoursRepository>();
            _repositoryFactoryMock = new Mock<IRepositoryFactory>();

            // Configuration de la factory pour retourner les repositories mockés
            _repositoryFactoryMock.Setup(f => f.UeRepository()).Returns(_ueRepositoryMock.Object);
            _repositoryFactoryMock.Setup(f => f.ParcoursRepository()).Returns(_parcoursRepositoryMock.Object);

            // Initialisation des use cases avec la factory mockée
            _addUeUseCase = new AddUeDansParcoursUseCase(_repositoryFactoryMock.Object);
            _createUeUseCase = new CreateUeUseCase(_repositoryFactoryMock.Object);
        }

        // TESTS POUR LA CREATION D'UNE UE 
        [Test]
        public async Task ExecuteAsync_Should_Create_Ue_When_Valid()
        {
            // Arrange
            string numeroUe = "UE123";
            string intitule = "Programmation Orientée Objet";
            Ue ueSansId = new Ue { NumeroUe = numeroUe, Intitule = intitule };
            Ue ueCree = new Ue { Id = 1, NumeroUe = numeroUe, Intitule = intitule };

            // Configuration du repository mock
            _ueRepositoryMock
                .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
                .ReturnsAsync(new List<Ue>()); // Aucune UE existante avec ce numéro

            _ueRepositoryMock
                .Setup(repo => repo.CreateAsync(ueSansId))
                .ReturnsAsync(ueCree);

            _ueRepositoryMock
                .Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _createUeUseCase.ExecuteAsync(ueSansId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(ueCree.Id));
            Assert.That(result.NumeroUe, Is.EqualTo(ueCree.NumeroUe));
            Assert.That(result.Intitule, Is.EqualTo(ueCree.Intitule));
        }

        [Test]
        public void ExecuteAsync_Should_Throw_DuplicateNumeroUeException_When_Ue_Already_Exists()
        {
            // Arrange
            string numeroUe = "UE123";
            string intitule = "Programmation Orientée Objet";
            Ue ueExistante = new Ue { Id = 1, NumeroUe = numeroUe, Intitule = intitule };

            _ueRepositoryMock
                .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
                .ReturnsAsync(new List<Ue> { ueExistante }); // UE déjà existante

            // Act & Assert
            var exception = Assert.ThrowsAsync<DuplicateNumeroUeException>(async () => await _createUeUseCase.ExecuteAsync(numeroUe, intitule));
            Assert.That(exception.Message, Is.EqualTo($"L'UE avec le numéro '{numeroUe}' existe déjà."));
        }

        [Test]
        public void ExecuteAsync_Should_Throw_InvalidUeIntituleException_When_Intitule_Too_Short()
        {
            // Arrange
            string numeroUe = "UE123";
            string intitule = "AB"; // Trop court

            // Act & Assert
            var exception = Assert.ThrowsAsync<InvalidUeIntituleException>(async () => await _createUeUseCase.ExecuteAsync(numeroUe, intitule));
            Assert.That(exception.Message, Is.EqualTo($"L'intitulé '{intitule}' est invalide - Il doit contenir plus de 3 caractères."));
        }

        // TESTS POUR L'AJOUT D'UNE UE DANS UN PARCOURS 

        [Test]
        public async Task ExecuteAsync_Should_Add_Ue_To_Parcours_When_Valid()
        {
            // Arrange
            long idParcours = 1;
            long idUe = 2;

            var parcours = new Parcours
            {
                Id = idParcours,
                NomParcours = "Parcours 1",
                AnneeFormation = 2025,
                UesEnseignees = new List<Ue>() // Aucun UE présent initialement
            };

            var ue = new Ue
            {
                Id = idUe,
                NumeroUe = "UE1",
                Intitule = "Programmation avancée"
            };

            // Assurez-vous que FindByConditionAsync retourne le parcours sans l'UE
            _parcoursRepositoryMock
                .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Parcours, bool>>>()))
                .ReturnsAsync(new List<Parcours> { parcours });

            _ueRepositoryMock
                .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
                .ReturnsAsync(new List<Ue> { ue });

            // Simuler l'ajout de l'UE dans le parcours
            _parcoursRepositoryMock
                .Setup(repo => repo.AddUeAsync(idParcours, idUe))
                .ReturnsAsync(() =>
                {
                    parcours.UesEnseignees.Add(ue);
                    return parcours;
                });

            // Act
            var result = await _addUeUseCase.ExecuteAsync(idParcours, idUe);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(idParcours));
            Assert.That(result.UesEnseignees, Is.Not.Null);
            Assert.That(result.UesEnseignees.Count, Is.EqualTo(1));
            Assert.That(result.UesEnseignees[0].Id, Is.EqualTo(idUe));

            _parcoursRepositoryMock.Verify(repo => repo.AddUeAsync(idParcours, idUe), Times.Once);
        }




        [Test]
        public void ExecuteAsync_Should_Throw_DuplicateUeDansParcoursException_When_Ue_Already_In_Parcours()
        {
            // Arrange
            long idParcours = 1;
            long idUe = 2;

            var ue = new Ue { Id = idUe, NumeroUe = "UE1", Intitule = "Programmation avancée" };
            var parcours = new Parcours { Id = idParcours, NomParcours = "Parcours 1", AnneeFormation = 2025, UesEnseignees = new List<Ue> { ue } };

            _parcoursRepositoryMock
                .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Parcours, bool>>>()))
                .ReturnsAsync(new List<Parcours> { parcours });

            _ueRepositoryMock
                .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
                .ReturnsAsync(new List<Ue> { ue });

            // Act & Assert
            var exception = Assert.ThrowsAsync<DuplicateUeDansParcoursException>(async () => await _addUeUseCase.ExecuteAsync(idParcours, idUe));
            Assert.That(exception.Message, Is.EqualTo($"{idUe} est déjà présente dans le parcours : {idParcours}"));
        }
    }
}
