using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

public class GetEtudiantByIdUseCase
{
    private readonly IEtudiantRepository _etudiantRepository;

    public GetEtudiantByIdUseCase(IRepositoryFactory repositoryFactory)
    {
        _etudiantRepository = repositoryFactory.EtudiantRepository();
    }

    public bool IsAuthorized(string role, IUniversiteUser user, long etudiantId)
    {
        return role == Roles.Responsable || role == Roles.Scolarite || (user.Etudiant?.Id == etudiantId);
    }

    public async Task<Etudiant?> ExecuteAsync(long id)
    {
        return await _etudiantRepository.GetByIdAsync(id);
    }
}