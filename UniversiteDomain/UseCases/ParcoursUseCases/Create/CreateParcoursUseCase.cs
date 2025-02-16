using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Create
{
    public class CreateParcoursUseCase
    {
        private readonly IRepositoryFactory _repositoryFactory;

        // Injection de la factory dans le constructeur
        public CreateParcoursUseCase(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task<Parcours> ExecuteAsync(string nomParcours, int anneeFormation)
        {
            var parcours = new Parcours 
            { 
                NomParcours = nomParcours, 
                AnneeFormation = anneeFormation 
            };
            return await ExecuteAsync(parcours);
        }

        public async Task<Parcours> ExecuteAsync(Parcours parcours)
        {
            // Récupération du repository via la factory
            var parcoursRepository = _repositoryFactory.ParcoursRepository();

            await CheckBusinessRules(parcours, parcoursRepository);

            // Création du parcours et sauvegarde des changements
            Parcours createdParcours = await parcoursRepository.CreateAsync(parcours);
            await parcoursRepository.SaveChangesAsync();

            return createdParcours;
        }

        private async Task CheckBusinessRules(Parcours parcours, IParcoursRepository parcoursRepository)
        {
            ArgumentNullException.ThrowIfNull(parcours);
            ArgumentNullException.ThrowIfNull(parcours.NomParcours);
            ArgumentNullException.ThrowIfNull(parcoursRepository);

            // Vérification si un parcours avec le même nom existe déjà
            List<Parcours> existe = await parcoursRepository.FindByConditionAsync(
                p => p.NomParcours.Equals(parcours.NomParcours)
            );

            if (existe.Any())
                throw new DuplicateNomParcoursException(
                    $"Le parcours '{parcours.NomParcours}' existe déjà."
                );

            // Vérification que le nom du parcours contient au moins 2 caractères
            if (parcours.NomParcours.Length < 2)
                throw new InvalidNomParcoursException(
                    $"Nom '{parcours.NomParcours}' incorrect - Il doit contenir au moins 3 caractères."
                );
        }
    }
}
