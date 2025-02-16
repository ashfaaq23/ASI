using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

public class GetUeByIdUseCase
{
    private readonly IUeRepository _ueRepository;

    public GetUeByIdUseCase(IRepositoryFactory repositoryFactory)
    {
        _ueRepository = repositoryFactory.UeRepository();
    }

    public async Task<Ue?> ExecuteAsync(long id)
    {
        return await _ueRepository.GetByIdAsync(id);
    }
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}