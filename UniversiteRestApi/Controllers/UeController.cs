using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.UeUseCases.Create;
using UniversiteDomain.UseCases.SecurityUseCases.Get;

namespace UniversiteRestApi.Controllers
{
    [Route("api/ue")]
    [ApiController]
    public class UeController(IRepositoryFactory repositoryFactory) : ControllerBase
    {
        private readonly IRepositoryFactory _repositoryFactory = repositoryFactory;

        // GET: api/ue
        [HttpGet]
        public async Task<ActionResult<List<UeDto>>> GetAllAsync()
        {
            string role, email;
            IUniversiteUser user;
            try { CheckSecu(out role, out email, out user); }
            catch { return Unauthorized(); }

            var useCase = new GetToutesLesUesUseCase(_repositoryFactory);
            if (!useCase.IsAuthorized(role)) return Unauthorized();
            
            var ues = await useCase.ExecuteAsync();
            return UeDto.ToDtos(ues);
        }

        // GET: api/ue/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UeDto>> GetByIdAsync(long id)
        {
            string role, email;
            IUniversiteUser user;
            try { CheckSecu(out role, out email, out user); }
            catch { return Unauthorized(); }

            var useCase = new GetUeByIdUseCase(_repositoryFactory);
            if (!useCase.IsAuthorized(role)) return Unauthorized();
            
            var ue = await useCase.ExecuteAsync(id);
            if (ue == null) return NotFound();
            return new UeDto().ToDto(ue);
        }

        // POST: api/ue
        [HttpPost]
        public async Task<ActionResult<UeDto>> CreateAsync([FromBody] UeDto ueDto)
        {
            string role, email;
            IUniversiteUser user;
            try { CheckSecu(out role, out email, out user); }
            catch { return Unauthorized(); }

            var useCase = new CreateUeUseCase(_repositoryFactory);
            if (!useCase.IsAuthorized(role)) return Unauthorized();
            
            var ue = await useCase.ExecuteAsync(ueDto.ToEntity());
            return CreatedAtAction(nameof(GetByIdAsync), new { id = ue.Id }, new UeDto().ToDto(ue));
        }

        // PUT: api/ue/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(long id, [FromBody] UeDto ueDto)
        {
            if (id != ueDto.Id) return BadRequest();

            string role, email;
            IUniversiteUser user;
            try { CheckSecu(out role, out email, out user); }
            catch { return Unauthorized(); }

            var useCase = new UpdateUeUseCase(_repositoryFactory);
            if (!useCase.IsAuthorized(role)) return Unauthorized();
            
            await useCase.ExecuteAsync(ueDto.ToEntity());
            return NoContent();
        }

        // DELETE: api/ue/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            string role, email;
            IUniversiteUser user;
            try { CheckSecu(out role, out email, out user); }
            catch { return Unauthorized(); }

            var useCase = new DeleteUeUseCase(_repositoryFactory);
            if (!useCase.IsAuthorized(role)) return Unauthorized();
            
            await useCase.ExecuteAsync(id);
            return NoContent();
        }

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
