namespace UniversiteDomain.Entities;

public interface IUniversiteUser
{
    long ? EtudiantId { get; set; }
    Etudiant? Etudiant { get; set; }
    string UserName { get; set; }
    string Email { get; set; }
}