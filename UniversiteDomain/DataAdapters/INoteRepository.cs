using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;

public interface INoteRepository: IRepository<Note>
{
    Task<Note> AddNoteAsync(Note note);
    Task<List<Note>> GetNotesByEtudiantAsync(long etudiantId);
    Task<List<Note>> GetNotesByUeAsync(long ueId);
    Task<Note?> GetNoteByEtudiantAndUeAsync(long etudiantId, long ueId);
}
