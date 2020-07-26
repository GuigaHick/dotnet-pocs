using JWTSecurity.Models;
using JWTSecurity.Repositories;
using JWTSecurity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JWTSecurity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> Authenticate([FromBody] User user)
        {
            var localUser = UserRepository.Get(user.Name, user.Password);

            if (localUser == null)
                return NotFound(new { message = "User or Invalid password" });

            var token = TokenService.GenerateToken(localUser);

            localUser.Password = "";

            return new
            {
                user = localUser,
                token = token
            };
        }

        [HttpGet]
        [Route("anonymous")]
        [AllowAnonymous]
        public string Anonymous() => "Hello, please authenticate to see application content";

        [HttpGet]
        [Route("authenticated")]
        [Authorize]

        public string Authenticated() => "Authenticated";

        [HttpGet]
        [Route("employee")]
        [Authorize(Roles = "employee,manager")]
        public string Employee() => $"Employee: {User.Identity.Name}";


        [HttpGet]
        [Route("manager")]
        [Authorize(Roles = "manager")]
        public string Manager() => $"Manager: {User.Identity.Name}";

    }
}