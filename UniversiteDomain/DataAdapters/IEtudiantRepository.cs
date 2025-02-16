using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;

public interface IEtudiantRepository : IRepository<Etudiant>
{
    // Affecter un étudiant à un parcours
    Task AffecterParcoursAsync(long idEtudiant, long idParcours);
    Task<Etudiant?> GetByIdAsync(long id);
    public Task<Etudiant?> FindEtudiantCompletAsync(long idEtudiant);

}