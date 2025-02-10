using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Create;

public class CreateParcoursUseCase(IParcoursRepository parcoursRepository)
{
    public async Task<Parcours> ExecuteAsync(string nomParcours, int anneeFormation)
    {
        var parcours = new Parcours { NomParcours = nomParcours, AnneeFormation = anneeFormation };
        return await ExecuteAsync(parcours);
    }

    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules(parcours);
        Parcours createdParcours = await parcoursRepository.CreateAsync(parcours);
        parcoursRepository.SaveChangesAsync().Wait();
        return createdParcours;
    }

    private async Task CheckBusinessRules(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(parcours.NomParcours);
        ArgumentNullException.ThrowIfNull(parcoursRepository);

        // Vérification si un parcours avec le même nom existe déjà
        List<Parcours> existe = await parcoursRepository.FindByConditionAsync(p => p.NomParcours.Equals(parcours.NomParcours));

        if (existe.Any())
            throw new DuplicateNomParcoursException($"Le parcours '{parcours.NomParcours}' existe déjà.");

        // Vérification que le nom du parcours contient au moins 3 caractères
        if (parcours.NomParcours.Length < 3)
            throw new InvalidNomParcoursException($"Nom '{parcours.NomParcours}' incorrect - Il doit contenir au moins 3 caractères.");

        // Vérification que l'année de formation est raisonnable (exemple : entre 1900 et l'année actuelle + 10 ans)
        int anneeActuelle = DateTime.Now.Year;
        if (parcours.AnneeFormation < 1900 || parcours.AnneeFormation > anneeActuelle + 10)
            throw new InvalidAnneeFormationException($"L'année de formation '{parcours.AnneeFormation}' est invalide.");
    }
}
