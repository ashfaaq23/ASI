using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Util;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Create
{
    public class CreateEtudiantUseCase
    {
        private readonly IEtudiantRepository _etudiantRepository;

        // Injection de la factory dans le constructeur, et récupération du repository
        public CreateEtudiantUseCase(IRepositoryFactory repositoryFactory)
        {
            if (repositoryFactory == null)
                throw new ArgumentNullException(nameof(repositoryFactory));

            _etudiantRepository = repositoryFactory.EtudiantRepository();
        }

        public async Task<Etudiant> ExecuteAsync(string numEtud, string nom, string prenom, string email)
        {
            var etudiant = new Etudiant
            {
                NumEtud = numEtud,
                Nom = nom,
                Prenom = prenom,
                Email = email
            };

            return await ExecuteAsync(etudiant);
        }

        public async Task<Etudiant> ExecuteAsync(Etudiant etudiant)
        {
            await CheckBusinessRules(etudiant);
            Etudiant et = await _etudiantRepository.CreateAsync(etudiant);
            await _etudiantRepository.SaveChangesAsync();
            return et;
        }

        private async Task CheckBusinessRules(Etudiant etudiant)
        {
            ArgumentNullException.ThrowIfNull(etudiant);
            ArgumentNullException.ThrowIfNull(etudiant.NumEtud);
            ArgumentNullException.ThrowIfNull(etudiant.Email);
            // La vérification de _etudiantRepository n'est plus forcément utile
            // car elle est initialisée via la factory dans le constructeur.
            // Toutefois, on peut la garder pour être sûr.
            ArgumentNullException.ThrowIfNull(_etudiantRepository);

            // Vérification si un étudiant avec le même numéro étudiant existe déjà
            List<Etudiant> existe = await _etudiantRepository.FindByConditionAsync(e => e.NumEtud.Equals(etudiant.NumEtud));
            if (existe is { Count: > 0 })
                throw new DuplicateNumEtudException(etudiant.NumEtud + " - ce numéro d'étudiant est déjà affecté à un étudiant");

            // Vérification du format du mail
            if (!CheckEmail.IsValidEmail(etudiant.Email))
                throw new InvalidEmailException(etudiant.Email + " - Email mal formé");

            // Vérification que l'email n'est pas déjà utilisé
            existe = await _etudiantRepository.FindByConditionAsync(e => e.Email.Equals(etudiant.Email));
            if (existe is { Count: > 0 })
                throw new DuplicateEmailException(etudiant.Email + " est déjà affecté à un étudiant");

            // Le métier définit que le nom doit contenir plus de 3 lettres
            if (etudiant.Nom.Length < 3)
                throw new InvalidNomEtudiantException(etudiant.Nom + " incorrect - Le nom d'un étudiant doit contenir plus de 3 caractères");
        }
    }
}
