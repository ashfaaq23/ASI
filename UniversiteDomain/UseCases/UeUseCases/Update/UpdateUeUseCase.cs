using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

public class UpdateUeUseCase
{
    private readonly IUeRepository _ueRepository;

    public UpdateUeUseCase(IRepositoryFactory repositoryFactory)
    {
        _ueRepository = repositoryFactory.UeRepository();
    }

    public async Task ExecuteAsync(Ue ue)
    {
        if (ue == null) throw new ArgumentNullException(nameof(ue));
        await _ueRepository.UpdateAsync(ue);
    }
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}