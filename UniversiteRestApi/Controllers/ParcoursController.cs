using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.ParcoursUseCases.Create;
using UniversiteDomain.UseCases.SecurityUseCases.Get;
using UniversiteEFDataProvider.Entities;

namespace UniversiteRestApi.Controllers
{
    [Route("api/parcours")]
    [ApiController]
    public class ParcoursController : ControllerBase
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public ParcoursController(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        //  GET: api/parcours - Get all parcours
        [HttpGet]
        public async Task<ActionResult<List<ParcoursDto>>> GetAllAsync()
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

            GetTousLesParcoursUseCase uc = new GetTousLesParcoursUseCase(_repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();

            List<Parcours> parcours;
            try
            {
                parcours = await uc.ExecuteAsync();
            }
            catch (Exception)
            {
                return ValidationProblem();
            }

            return ParcoursDto.ToDtos(parcours);
        }

        //  GET: api/parcours/{id} - Get a parcours by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ParcoursDto>> GetByIdAsync(long id)
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

            GetParcoursByIdUseCase uc = new GetParcoursByIdUseCase(_repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();

            Parcours? parcours;
            try
            {
                parcours = await uc.ExecuteAsync(id);
            }
            catch (Exception)
            {
                return ValidationProblem();
            }

            if (parcours == null) return NotFound();
            return new ParcoursDto().ToDto(parcours);
        }

        //  POST: api/parcours - Create a new parcours
        [HttpPost]
        public async Task<ActionResult<ParcoursDto>> CreateAsync([FromBody] ParcoursDto parcoursDto)
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

            CreateParcoursUseCase uc = new CreateParcoursUseCase(_repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();

            Parcours parcours;
            try
            {
                parcours = await uc.ExecuteAsync(parcoursDto.ToEntity());
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }

            return CreatedAtAction(nameof(GetByIdAsync), new { id = parcours.Id }, new ParcoursDto().ToDto(parcours));
        }

        //  PUT: api/parcours/{id} - Update a parcours
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(long id, [FromBody] ParcoursDto parcoursDto)
        {
            if (id != parcoursDto.Id) return BadRequest();

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

            UpdateParcoursUseCase uc = new UpdateParcoursUseCase(_repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();

            try
            {
                await uc.ExecuteAsync(parcoursDto.ToEntity());
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }

            return NoContent();
        }

        //  DELETE: api/parcours/{id} - Delete a parcours
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(long id)
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

            DeleteParcoursUseCase uc = new DeleteParcoursUseCase(_repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();

            try
            {
                await uc.ExecuteAsync(id);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }

            return NoContent();
        }

        //  Security Check Method
        private void CheckSecu(out string role, out string email, out IUniversiteUser user)
        {
            role = "";
            ClaimsPrincipal claims = HttpContext.User;
            if (claims.FindFirst(ClaimTypes.Email) == null) throw new UnauthorizedAccessException();
            email = claims.FindFirst(ClaimTypes.Email).Value;
            if (email == null) throw new UnauthorizedAccessException();

            user = new FindUniversiteUserByEmailUseCase(_repositoryFactory).ExecuteAsync(email).Result;
            if (user == null) throw new UnauthorizedAccessException();
            if (claims.Identity?.IsAuthenticated != true) throw new UnauthorizedAccessException();

            var ident = claims.Identities.FirstOrDefault();
            if (ident == null) throw new UnauthorizedAccessException();
            if (claims.FindFirst(ClaimTypes.Role) == null) throw new UnauthorizedAccessException();

            role = ident.FindFirst(ClaimTypes.Role).Value;
            if (role == null) throw new UnauthorizedAccessException();
        }
    }
}
