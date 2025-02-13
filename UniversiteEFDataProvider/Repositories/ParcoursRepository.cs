using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class ParcoursRepository(UniversiteDbContext context) : Repository<Parcours>(context), IParcoursRepository
{
    //  Ajouter un étudiant dans un parcours via Objet `Parcours` et `Etudiant`
    public async Task<Parcours> AddEtudiantAsync(Parcours parcours, Etudiant etudiant)
    {
        if (parcours == null || etudiant == null)
            throw new ArgumentNullException("Le parcours et l'étudiant ne doivent pas être null.");

        if (parcours.Inscrits!.Any(e => e.Id == etudiant.Id))
            throw new InvalidOperationException($"L'étudiant {etudiant.Id} est déjà inscrit dans ce parcours.");

        parcours.Inscrits!.Add(etudiant);
        await SaveChangesAsync();

        return parcours;
    }

    //  Ajouter UN étudiant dans un parcours via ID
    public async Task<Parcours> AddEtudiantAsync(long idParcours, long idEtudiant)
    {
        var parcours = await FindAsync(idParcours)
            ?? throw new KeyNotFoundException($"Le parcours {idParcours} n'existe pas.");
        var etudiant = await Context.Etudiants.FindAsync(idEtudiant)
            ?? throw new KeyNotFoundException($"L'étudiant {idEtudiant} n'existe pas.");

        return await AddEtudiantAsync(parcours, etudiant);
    }

    //  Ajouter plusieurs étudiants dans un parcours via Objet `Parcours`
    public async Task<Parcours> AddEtudiantAsync(Parcours? parcours, List<Etudiant> etudiants)
    {
        if (parcours == null || etudiants == null)
            throw new ArgumentNullException("Le parcours et les étudiants ne doivent pas être null.");

        foreach (var etudiant in etudiants)
        {
            if (!parcours.Inscrits!.Any(e => e.Id == etudiant.Id))
                parcours.Inscrits!.Add(etudiant);
        }

        await SaveChangesAsync();
        return parcours;
    }

    //  Ajouter plusieurs étudiants dans un parcours via Liste d'IDs
    public async Task<Parcours> AddEtudiantAsync(long idParcours, long[] idEtudiants)
    {
        var parcours = await FindAsync(idParcours)
            ?? throw new KeyNotFoundException($"Le parcours {idParcours} n'existe pas.");

        var etudiants = await Context.Etudiants
            .Where(e => idEtudiants.Contains(e.Id))
            .ToListAsync();

        if (etudiants.Count != idEtudiants.Length)
            throw new InvalidOperationException("Certains étudiants fournis n'existent pas.");

        return await AddEtudiantAsync(parcours, etudiants);
    }

    //  Ajouter UNE UE dans un parcours
    public async Task<Parcours> AddUeAsync(long idParcours, long idUe)
    {
        var parcours = await FindAsync(idParcours)
            ?? throw new KeyNotFoundException($"Le parcours {idParcours} n'existe pas.");
        var ue = await Context.Ues.FindAsync(idUe)
            ?? throw new KeyNotFoundException($"L'UE {idUe} n'existe pas.");

        if (parcours.UesEnseignees!.Any(u => u.Id == idUe))
            throw new InvalidOperationException($"L'UE {idUe} est déjà enseignée dans ce parcours.");

        parcours.UesEnseignees!.Add(ue);
        await SaveChangesAsync();

        return parcours;
    }

    //  Ajouter plusieurs UEs dans un parcours
    public async Task<Parcours> AddUeAsync(long idParcours, long[] idUes)
    {
        var parcours = await FindAsync(idParcours)
            ?? throw new KeyNotFoundException($"Le parcours {idParcours} n'existe pas.");

        var ues = await Context.Ues
            .Where(ue => idUes.Contains(ue.Id))
            .ToListAsync();

        if (ues.Count != idUes.Length)
            throw new InvalidOperationException("Certaines UEs fournies n'existent pas.");

        foreach (var ue in ues)
        {
            if (!parcours.UesEnseignees!.Any(u => u.Id == ue.Id))
                parcours.UesEnseignees!.Add(ue);
        }

        await SaveChangesAsync();
        return parcours;
    }

    //  Trouver le parcours d'un étudiant
    public async Task<Parcours?> GetParcoursByEtudiantAsync(long etudiantId)
    {
        return await Context.Parcours
            .Include(p => p.Inscrits)
            .FirstOrDefaultAsync(p => p.Inscrits!.Any(e => e.Id == etudiantId));
    }
}
