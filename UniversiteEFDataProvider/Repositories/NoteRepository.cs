using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class NoteRepository(UniversiteDbContext context) : Repository<Note>(context), INoteRepository
{
    //  Ajouter une note pour un étudiant et une UE
    public async Task<Note> AddNoteAsync(Note note)
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);
        await Context.Notes.AddAsync(note);
        await Context.SaveChangesAsync();
        return note;
    }

    //  Vérifier si un étudiant a déjà une note dans une UE
    public async Task<Note?> GetNoteByEtudiantAndUeAsync(long etudiantId, long ueId)
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);
        return await Context.Notes
            .FirstOrDefaultAsync(n => n.EtudiantId == etudiantId && n.UeId == ueId);
    }

    //  Modifier une note existante
    public async Task UpdateNoteAsync(long etudiantId, long ueId, float nouvelleValeur)
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);
        
        var note = await Context.Notes
            .FirstOrDefaultAsync(n => n.EtudiantId == etudiantId && n.UeId == ueId);

        if (note == null)
            throw new KeyNotFoundException($"Aucune note trouvée pour l'étudiant {etudiantId} dans l'UE {ueId}.");

        note.Valeur = nouvelleValeur; // Vérification de la valeur incluse dans l'entité
        await Context.SaveChangesAsync();
    }

    //  Supprimer une note
    public async Task DeleteNoteAsync(long etudiantId, long ueId)
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);
        
        var note = await Context.Notes
            .FirstOrDefaultAsync(n => n.EtudiantId == etudiantId && n.UeId == ueId);

        if (note == null)
            throw new KeyNotFoundException($"Aucune note trouvée pour l'étudiant {etudiantId} dans l'UE {ueId}.");

        Context.Notes.Remove(note);
        await Context.SaveChangesAsync();
    }
}
