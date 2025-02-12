using UniversiteDomain.Exceptions.NoteExceptions;

namespace UniversiteDomain.Entities;

public class Note
{
    public long Id { get; set; }

    // La valeur de la note (entre 0 et 20)
    private float _valeur;
    public float Valeur
    {
        get => _valeur;
        set
        {
            if (value < 0 || value > 20)
                throw new InvalidNoteException($"La note {value} est invalide. Elle doit être comprise entre 0 et 20.");
            _valeur = value;
        }
    }

    // Clés étrangères
    public long EtudiantId { get; set; }
    public long UeId { get; set; }

    // Relations
    public Etudiant Etudiant { get; set; }
    public Ue Ue { get; set; }
}
