using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

public class DeleteUeUseCase
{
    private readonly IUeRepository _ueRepository;

    public DeleteUeUseCase(IRepositoryFactory repositoryFactory)
    {
        _ueRepository = repositoryFactory.UeRepository();
    }

    public async Task ExecuteAsync(long id)
    {
        await _ueRepository.DeleteAsync(id);
    }
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}