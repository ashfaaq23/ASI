using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;

public class ImportCsvForNotesUseCase
{
    private readonly IRepositoryFactory _repositoryFactory;

    public ImportCsvForNotesUseCase(IRepositoryFactory repositoryFactory)
    {
        _repositoryFactory = repositoryFactory;
    }

    public async Task<List<string>> ExecuteAsync(Stream csvFile)
    {
        var errors = new List<string>();

        using var reader = new StreamReader(csvFile);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "," });

        var records = csv.GetRecords<NoteCsvDto>().ToList();

        // Get repositories properly
        var etudiantRepository = _repositoryFactory.GetEtudiantRepository();
        var ueRepository = _repositoryFactory.GetUeRepository();
        var noteRepository = _repositoryFactory.GetNoteRepository();

        foreach (var record in records)
        {
            // Check note validity
            if (record.Note is < 0 or > 20)
            {
                errors.Add($"Erreur: La note {record.Note} pour l'étudiant {record.NumEtud} est invalide.");
                continue;
            }

            // Check if student exists
            var etudiant = await etudiantRepository.GetByIdAsync(record.NumEtud);
            if (etudiant == null)
            {
                errors.Add($"Erreur: L'étudiant {record.NumEtud} n'existe pas.");
                continue;
            }

            // Check if UE exists
            var ue = await ueRepository.GetByIdAsync(record.NumeroUe);
            if (ue == null)
            {
                errors.Add($"Erreur: L'UE {record.NumeroUe} n'existe pas.");
                continue;
            }

            // Check if the note already exists
            var existingNote = await noteRepository.GetNoteByEtudiantAndUeAsync(record.NumEtud, record.NumeroUe);
            if (existingNote != null)
            {
                await noteRepository.UpdateNoteAsync(record.NumEtud, record.NumeroUe, record.Note ?? 0);
            }
            else
            {
                await noteRepository.AddNoteAsync(new Note
                {
                    EtudiantId = record.NumEtud,
                    UeId = record.NumeroUe,
                    Valeur = record.Note ?? 0
                });
            }
        }

        return errors;
    }
}
