using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;
using UniversiteDomain.Util;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace UniversiteDomainUnitTests
{
    public class EtudiantUnitTest
    {
        [SetUp]
        public void Setup()
        {
        }
        
        [Test]
        public async Task CreateEtudiantUseCase_WithFactoryMock_ReturnsCreatedEtudiant()
        {
            // Arrange
            long id = 1;
            string numEtud = "et1";
            string nom = "Durant";
            string prenom = "Jean";
            string email = "jean.durant@etud.u-picardie.fr";
            
            // On crée l'étudiant qui doit être ajouté en base (sans Id)
            Etudiant etudiantSansId = new Etudiant 
            { 
                NumEtud = numEtud, 
                Nom = nom, 
                Prenom = prenom, 
                Email = email 
            };

            // Création du mock du repository IEtudiantRepository
            var etudiantRepositoryMock = new Mock<IEtudiantRepository>();

            // Configuration de FindByConditionAsync pour renvoyer une liste vide
            etudiantRepositoryMock
                .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()))
                .ReturnsAsync(new List<Etudiant>());

            // Configuration de CreateAsync pour renvoyer un étudiant avec l'Id assigné
            Etudiant etudiantCree = new Etudiant 
            { 
                Id = id, 
                NumEtud = numEtud, 
                Nom = nom, 
                Prenom = prenom, 
                Email = email 
            };
            etudiantRepositoryMock
                .Setup(repo => repo.CreateAsync(etudiantSansId))
                .ReturnsAsync(etudiantCree);

            // Configuration de SaveChangesAsync pour renvoyer une tâche complétée
            etudiantRepositoryMock
                .Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Création du mock de la factory
            var repositoryFactoryMock = new Mock<IRepositoryFactory>();
            // Quand on appelle la méthode EtudiantRepository, on renvoie notre mock du repository
            repositoryFactoryMock
                .Setup(factory => factory.EtudiantRepository())
                .Returns(etudiantRepositoryMock.Object);

            // Création du use case en injectant le mock de la factory
            CreateEtudiantUseCase useCase = new CreateEtudiantUseCase(repositoryFactoryMock.Object);

            // Act
            var etudiantTest = await useCase.ExecuteAsync(etudiantSansId);

            // Assert
            Assert.That(etudiantTest.Id, Is.EqualTo(etudiantCree.Id));
            Assert.That(etudiantTest.NumEtud, Is.EqualTo(etudiantCree.NumEtud));
            Assert.That(etudiantTest.Nom, Is.EqualTo(etudiantCree.Nom));
            Assert.That(etudiantTest.Prenom, Is.EqualTo(etudiantCree.Prenom));
            Assert.That(etudiantTest.Email, Is.EqualTo(etudiantCree.Email));

            // Optionnel : Vérifier que les méthodes du repository ont été appelées
            etudiantRepositoryMock.Verify(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()), Times.AtLeastOnce);
            etudiantRepositoryMock.Verify(repo => repo.CreateAsync(etudiantSansId), Times.Once);
            etudiantRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
    }
}
