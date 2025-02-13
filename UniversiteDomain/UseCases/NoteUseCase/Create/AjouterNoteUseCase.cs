using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.NoteExceptions;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.NoteUseCases
{
    public class AjouterNoteUseCase
    {
        private readonly INoteRepository _noteRepository;
        private readonly IParcoursRepository _parcoursRepository;

        public AjouterNoteUseCase(IRepositoryFactory repositoryFactory)
        {
            _noteRepository = repositoryFactory?.NoteRepository() ?? throw new ArgumentNullException(nameof(repositoryFactory));
            _parcoursRepository = repositoryFactory?.ParcoursRepository() ?? throw new ArgumentNullException(nameof(repositoryFactory));
        }

        public async Task<Note> ExecuteAsync(long etudiantId, long ueId, float valeur)
        {
            await CheckBusinessRules(etudiantId, ueId, valeur);

            var note = new Note
            {
                EtudiantId = etudiantId,
                UeId = ueId,
                Valeur = valeur
            };

            return await _noteRepository.CreateAsync(note);
        }

        private async Task CheckBusinessRules(long etudiantId, long ueId, float valeur)
        {
            if (valeur < 0 || valeur > 20)
                throw new InvalidNoteException($"La note {valeur} est invalide. Elle doit être entre 0 et 20.");

            // Vérifier si l'étudiant a déjà une note pour cette UE
            var existingNote = await _noteRepository.GetNoteByEtudiantAndUeAsync(etudiantId, ueId);
            if (existingNote != null)
                throw new DuplicateNoteException($"L'étudiant {etudiantId} a déjà une note dans l'UE {ueId}.");

            // Vérifier que l'étudiant est bien inscrit à un parcours contenant cette UE
            var parcoursEtudiant = await _parcoursRepository.GetParcoursByEtudiantAsync(etudiantId);
            if (parcoursEtudiant == null)
                throw new UnauthorizedNoteException($"L'étudiant {etudiantId} n'est inscrit dans aucun parcours.");

            // Vérifier si l'UE fait bien partie du parcours de l'étudiant
            bool ueDansParcours = parcoursEtudiant.UesEnseignees.Any(ue => ue.Id == ueId);
            if (!ueDansParcours)
                throw new UnauthorizedNoteException($"L'étudiant {etudiantId} ne peut pas être noté pour l'UE {ueId} car elle ne fait pas partie de son parcours.");
        }
    }
}
