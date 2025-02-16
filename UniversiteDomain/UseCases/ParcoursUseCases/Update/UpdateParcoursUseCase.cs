using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

public class UpdateParcoursUseCase
{
    private readonly IParcoursRepository _parcoursRepository;

    public UpdateParcoursUseCase(IRepositoryFactory repositoryFactory)
    {
        _parcoursRepository = repositoryFactory.ParcoursRepository();
    }

    public async Task ExecuteAsync(Parcours parcours)
    {
        if (parcours == null) throw new ArgumentNullException(nameof(parcours));
        await _parcoursRepository.UpdateAsync(parcours);
    }
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}