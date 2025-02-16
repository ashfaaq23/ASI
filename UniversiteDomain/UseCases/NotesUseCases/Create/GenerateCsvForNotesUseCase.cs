using System.Globalization;
using System.Text;
using CsvHelper;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;

public class GenerateCsvForNotesUseCase
{
    private readonly IRepositoryFactory _repositoryFactory;

    public GenerateCsvForNotesUseCase(IRepositoryFactory repositoryFactory)
    {
        _repositoryFactory = repositoryFactory;
    }

    public async Task<byte[]> ExecuteAsync(long ueId)
    {
        var etudiantRepository = _repositoryFactory.GetEtudiantRepository();
        var noteRepository = _repositoryFactory.GetNoteRepository();
        var ueRepository = _repositoryFactory.GetUeRepository();

        var etudiants = await etudiantRepository.GetEtudiantsByUeAsync(ueId);
        var ue = await ueRepository.GetByIdAsync(ueId); // Fetch UE details

        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        csv.WriteHeader<NoteCsvDto>();
        csv.NextRecord();

        foreach (var etudiant in etudiants)
        {
            var note = await noteRepository.GetNoteByEtudiantAndUeAsync(etudiant.Id, ueId);
            csv.WriteRecord(new NoteCsvDto
            {
                NumEtud = etudiant.Id,
                Nom = etudiant.Nom,
                Prenom = etudiant.Prenom,
                NumeroUe = ue?.Id ?? 0, // Ensure correct UE ID
                Intitule = ue?.Intitule ?? "Unknown UE", // Ensure correct UE Name
                Note = note?.Valeur
            });
            csv.NextRecord();
        }

        writer.Flush();
        return memoryStream.ToArray();
    }


}