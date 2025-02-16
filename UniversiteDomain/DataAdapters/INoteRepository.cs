using UniversiteDomain.Entities;

using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;

public interface INoteRepository : IRepository<Note>
{
    
    //  Trouver une note spécifique entre un étudiant et une UE
    Task<Note?> GetNoteByEtudiantAndUeAsync(long etudiantId, long ueId);

    //  Mettre à jour une note existante
    Task UpdateNoteAsync(long etudiantId, long ueId, float nouvelleValeur);
    Task<Note> AddNoteAsync(Note note);
}
