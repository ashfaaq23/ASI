using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.NoteUseCases;
using UniversiteDomain.UseCases.SecurityUseCases.Get;

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
            string role = "";
            string email = "";
            IUniversiteUser user = null;

            try
            {
                CheckSecu(out role, out email, out user);
            }
            catch (Exception)
            {
                return Unauthorized();
            }

            if (role != Roles.Scolarite) // Restriction: Only "scolarité" can generate CSV
                return Forbid("Accès refusé : Seule la scolarité peut générer le fichier CSV des notes.");

            var useCase = new GenerateCsvForNotesUseCase(_repositoryFactory);
            var fileBytes = await useCase.ExecuteAsync(ueId);

            return File(fileBytes, "text/csv", $"notes_UE_{ueId}.csv");
        }

        //  Importer un fichier CSV pour enregistrer les notes
        [HttpPost("upload")]
        public async Task<IActionResult> ImportCsv(IFormFile file)
        {
            string role = "";
            string email = "";
            IUniversiteUser user = null;

            try
            {
                CheckSecu(out role, out email, out user);
            }
            catch (Exception)
            {
                return Unauthorized();
            }

            if (role != Roles.Scolarite) // Restriction: Only "scolarité" can import notes
                return Forbid("Accès refusé : Seule la scolarité peut importer des notes.");

            if (file == null || file.Length == 0)
                return BadRequest("Aucun fichier sélectionné.");

            var useCase = new ImportCsvForNotesUseCase(_repositoryFactory);
            using var stream = file.OpenReadStream();
            var errors = await useCase.ExecuteAsync(stream);

            if (errors.Any())
                return BadRequest(errors);

            return Ok("Importation réussie !");
        }

        /// **Vérifie l'authentification et extrait le rôle**
        private void CheckSecu(out string role, out string email, out IUniversiteUser user)
        {
            role = "";
            ClaimsPrincipal claims = HttpContext.User;

            if (claims.FindFirst(ClaimTypes.Email) == null) throw new UnauthorizedAccessException();
            email = claims.FindFirst(ClaimTypes.Email)?.Value ?? throw new UnauthorizedAccessException();

            user = new FindUniversiteUserByEmailUseCase(_repositoryFactory).ExecuteAsync(email).Result;
            if (user == null) throw new UnauthorizedAccessException();

            if (claims.Identity?.IsAuthenticated != true) throw new UnauthorizedAccessException();

            var ident = claims.Identities.FirstOrDefault();
            if (ident == null) throw new UnauthorizedAccessException();

            if (claims.FindFirst(ClaimTypes.Role) == null) throw new UnauthorizedAccessException();
            role = ident.FindFirst(ClaimTypes.Role)?.Value ?? throw new UnauthorizedAccessException();
        }
    }
}
