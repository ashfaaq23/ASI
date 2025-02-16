using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

public class UpdateUniversiteUserUseCase
{
    private readonly IUniversiteUserRepository _universiteUserRepository;

    public UpdateUniversiteUserUseCase(IRepositoryFactory repositoryFactory)
    {
        _universiteUserRepository = repositoryFactory.UniversiteUserRepository();
    }

    public bool IsAuthorized(string role)
    {
        return role == Roles.Responsable;  // Make sure this role matches your authorization logic
    }

    public async Task ExecuteAsync(IUniversiteUser user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        await _universiteUserRepository.UpdateAsync(user);
    }
}