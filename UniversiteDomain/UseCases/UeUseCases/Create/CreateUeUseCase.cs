using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;

namespace UniversiteDomain.UseCases.UeUseCases.Create
{
    public class CreateUeUseCase
    {
        private readonly IUeRepository _ueRepository;

        public CreateUeUseCase(IRepositoryFactory repositoryFactory)
        {
            _ueRepository = repositoryFactory?.UeRepository() ?? throw new ArgumentNullException(nameof(repositoryFactory));
        }

        public async Task<Ue> ExecuteAsync(string numeroUe, string intitule)
        {
            var ue = new Ue { NumeroUe = numeroUe, Intitule = intitule };
            return await ExecuteAsync(ue);
        }

        public async Task<Ue> ExecuteAsync(Ue ue)
        {
            await CheckBusinessRules(ue);
            Ue createdUe = await _ueRepository.CreateAsync(ue);
            await _ueRepository.SaveChangesAsync();
            return createdUe;
        }

        private async Task CheckBusinessRules(Ue ue)
        {
            ArgumentNullException.ThrowIfNull(ue);
            ArgumentNullException.ThrowIfNull(ue.NumeroUe);
            ArgumentNullException.ThrowIfNull(ue.Intitule);

            // Vérifier que l’intitulé contient plus de 3 caractères
            if (ue.Intitule.Length <= 3)
                throw new InvalidUeIntituleException($"L'intitulé '{ue.Intitule}' est invalide - Il doit contenir plus de 3 caractères.");

            // Vérifier si un UE avec le même numéro existe déjà
            List<Ue> existe = await _ueRepository.FindByConditionAsync(u => u.NumeroUe.Equals(ue.NumeroUe));

            if (existe.Any())
                throw new DuplicateNumeroUeException($"L'UE avec le numéro '{ue.NumeroUe}' existe déjà.");
        }
    }
}