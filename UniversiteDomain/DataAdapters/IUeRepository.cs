using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;
public interface IUeRepository : IRepository<Ue>
{
    //  Récupérer une UE par son ID avec les parcours associés
    Task<Ue?> GetUeByIdAsync(long ueId);

    //  Récupérer toutes les UEs enseignées dans un parcours
    Task<List<Ue>> GetUesByParcoursAsync(long parcoursId);

    //  Ajouter une UE à un parcours
    Task<Ue> AddUeToParcoursAsync(long ueId, long parcoursId);
}