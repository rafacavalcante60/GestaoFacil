using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestaoFacil.Server.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected int UsuarioId
        {
            get
            {
                var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (int.TryParse(idClaim, out var id))
                {
                    return id;
                }

                throw new UnauthorizedAccessException("Usuário inválido.");
            }
        }
    }
}
