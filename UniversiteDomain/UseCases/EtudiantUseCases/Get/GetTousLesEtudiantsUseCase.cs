using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

public class GetTousLesEtudiantsUseCase
{
    private readonly IEtudiantRepository _etudiantRepository;

    public GetTousLesEtudiantsUseCase(IRepositoryFactory repositoryFactory)
    {
        _etudiantRepository = repositoryFactory.EtudiantRepository();
    }

    public bool IsAuthorized(string role)
    {
        // Vérifie si l'utilisateur a les droits pour voir les étudiants
        return role == Roles.Responsable || role == Roles.Scolarite;
    }

    public async Task<List<Etudiant>> ExecuteAsync()
    {
        return await _etudiantRepository.GetAllAsync();
    }
}