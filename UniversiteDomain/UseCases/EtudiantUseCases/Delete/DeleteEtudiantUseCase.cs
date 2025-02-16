using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Delete
{
    public class DeleteEtudiantUseCase
    {
        private readonly IEtudiantRepository _etudiantRepository;

        public DeleteEtudiantUseCase(IRepositoryFactory repositoryFactory)
        {
            if (repositoryFactory == null)
                throw new ArgumentNullException(nameof(repositoryFactory));

            _etudiantRepository = repositoryFactory.EtudiantRepository();
        }

        public async Task<bool> ExecuteAsync(long etudiantId)
        {
            // Vérifier si l'étudiant existe
            var etudiant = await _etudiantRepository.GetByIdAsync(etudiantId);
            if (etudiant == null)
            {
                throw new EtudiantNotFoundException($"L'étudiant avec l'ID {etudiantId} n'existe pas.");
            }

            // Supprimer l'étudiant
            await _etudiantRepository.DeleteAsync(etudiant);
            await _etudiantRepository.SaveChangesAsync();

            return true; // Indique que la suppression a réussi
        }
    }
}