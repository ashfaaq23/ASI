using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

public class GetParcoursByIdUseCase
{
    private readonly IParcoursRepository _parcoursRepository;

    public GetParcoursByIdUseCase(IRepositoryFactory repositoryFactory)
    {
        _parcoursRepository = repositoryFactory.ParcoursRepository();
    }

    public async Task<Parcours?> ExecuteAsync(long id)
    {
        return await _parcoursRepository.GetByIdAsync(id);
    }
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}