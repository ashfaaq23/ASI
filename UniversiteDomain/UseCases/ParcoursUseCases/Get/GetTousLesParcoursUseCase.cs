using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

public class GetTousLesParcoursUseCase
{
    private readonly IParcoursRepository _parcoursRepository;

    public GetTousLesParcoursUseCase(IRepositoryFactory repositoryFactory)
    {
        _parcoursRepository = repositoryFactory.ParcoursRepository();
    }

    public async Task<List<Parcours>> ExecuteAsync()
    {
        return await _parcoursRepository.GetAllAsync();
    }
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}