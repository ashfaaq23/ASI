using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class UeRepository(UniversiteDbContext context) : Repository<Ue>(context), IUeRepository
{
    // Récupérer une UE par son ID avec ses parcours associés
    public async Task<Ue?> GetUeByIdAsync(long ueId)
    {
        return await Context.Ues
            .Include(ue => ue.EnseigneeDans)
            .FirstOrDefaultAsync(ue => ue.Id == ueId);
    }

    // Récupérer toutes les UEs enseignées dans un parcours donné
    public async Task<List<Ue>> GetUesByParcoursAsync(long parcoursId)
    {
        var parcours = await Context.Parcours
            .Include(p => p.UesEnseignees)
            .FirstOrDefaultAsync(p => p.Id == parcoursId);

        return parcours?.UesEnseignees ?? new List<Ue>();
    }

    // Ajouter une UE dans un parcours
    public async Task<Ue> AddUeToParcoursAsync(long ueId, long parcoursId)
    {
        var ue = await FindAsync(ueId)
                 ?? throw new KeyNotFoundException($"L'UE {ueId} n'existe pas.");
        var parcours = await Context.Parcours.FindAsync(parcoursId)
                       ?? throw new KeyNotFoundException($"Le parcours {parcoursId} n'existe pas.");

        if (parcours.UesEnseignees!.Any(u => u.Id == ueId))
            throw new InvalidOperationException($"L'UE {ueId} est déjà enseignée dans le parcours {parcoursId}.");

        parcours.UesEnseignees!.Add(ue);
        await SaveChangesAsync();

        return ue;
    }
}