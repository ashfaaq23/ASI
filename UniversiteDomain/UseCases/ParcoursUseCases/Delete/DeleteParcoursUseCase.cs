using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

public class DeleteParcoursUseCase
{
    private readonly IParcoursRepository _parcoursRepository;

    public DeleteParcoursUseCase(IRepositoryFactory repositoryFactory)
    {
        _parcoursRepository = repositoryFactory.ParcoursRepository();
    }

    public async Task ExecuteAsync(long id)
    {
        await _parcoursRepository.DeleteAsync(id);
    }
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}