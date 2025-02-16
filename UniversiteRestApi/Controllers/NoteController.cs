using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.UseCases.NoteUseCases;

namespace UniversiteRestApi.Controllers
{
    [Route("api/notes")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public NoteController(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        //  Générer un fichier CSV pour la saisie des notes
        [HttpGet("generate/{ueId}")]
        public async Task<IActionResult> GenerateCsv(long ueId)
        {
            var useCase = new GenerateCsvForNotesUseCase(_repositoryFactory);
            var fileBytes = await useCase.ExecuteAsync(ueId);

            return File(fileBytes, "text/csv", $"notes_UE_{ueId}.csv");
        }

        //  Importer un fichier CSV
        [HttpPost("upload")]
        public async Task<IActionResult> ImportCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Aucun fichier sélectionné.");

            var useCase = new ImportCsvForNotesUseCase(_repositoryFactory);
            using var stream = file.OpenReadStream();
            var errors = await useCase.ExecuteAsync(stream);

            if (errors.Any())
                return BadRequest(errors);

            return Ok("Importation réussie !");
        }
    }
}