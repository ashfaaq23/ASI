using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

public class DeleteUniversiteUserUseCase
{
    private readonly IUniversiteUserRepository _universiteUserRepository;

    public DeleteUniversiteUserUseCase(IRepositoryFactory repositoryFactory)
    {
        _universiteUserRepository = repositoryFactory.UniversiteUserRepository();
    }

    public bool IsAuthorized(string role)
    {
        return role == Roles.Responsable;
    }

    public async Task ExecuteAsync(long userId)
    {
        await _universiteUserRepository.DeleteAsync(userId);
    }
}