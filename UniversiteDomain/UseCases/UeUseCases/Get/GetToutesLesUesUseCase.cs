using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

public class GetToutesLesUesUseCase
{
    private readonly IUeRepository _ueRepository;

    public GetToutesLesUesUseCase(IRepositoryFactory repositoryFactory)
    {
        _ueRepository = repositoryFactory.UeRepository();
    }

    public async Task<List<Ue>> ExecuteAsync()
    {
        return await _ueRepository.GetAllAsync();
    }
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}