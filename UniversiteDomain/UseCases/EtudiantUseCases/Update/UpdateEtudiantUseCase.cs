using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

public class UpdateEtudiantUseCase
{
    private readonly IEtudiantRepository _etudiantRepository;

    public UpdateEtudiantUseCase(IRepositoryFactory repositoryFactory)
    {
        _etudiantRepository = repositoryFactory.EtudiantRepository();
    }

    public bool IsAuthorized(string role)
    {
        return role == Roles.Responsable || role == Roles.Scolarite;
    }

    public async Task ExecuteAsync(Etudiant etudiant)
    {
        if (etudiant == null) throw new ArgumentNullException(nameof(etudiant));
        await _etudiantRepository.UpdateAsync(etudiant);
    }
}