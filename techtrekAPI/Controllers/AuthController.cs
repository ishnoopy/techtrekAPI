using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using techtrekAPI.Entities;
using techtrekAPI.Helpers;

namespace techtrekAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly TokenService _tokenService;
        private TechtrekContext _context;

        public AuthController(TokenService tokenService, TechtrekContext context)
        {
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("login")]
        public ActionResult<AuthResponse> Authenticate([FromBody] AuthRequest request)
        {
            var userInDb = _context.users.SingleOrDefault(user => user.email_address == request.email_address && user.password == request.password);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (userInDb == null)
            {
                return BadRequest("Invalid email or password");
            }
            else
            {
                var user = new ApplicationUser { Email = userInDb.email_address, UserName = userInDb.email_address, Id = Guid.NewGuid().ToString(), Role = userInDb.role };
                var accesstoken = _tokenService.CreateToken(user);

                return Ok(new AuthResponse
                {
                    id = userInDb.id,
                    email_address = userInDb.email_address,
                    last_name = userInDb.last_name,
                    first_name = userInDb.first_name,
                    role = userInDb.role,
                    token = accesstoken
                });
            }

        }


        [HttpPost("create")]
        public async Task<HttpStatusCode> CreateUser([FromBody] User user) // DOCU: [FromBody] means It will look at the body of the HttpRequest.
        {
            var entity = new User()
            {
                first_name = user.first_name,
                last_name = user.last_name,
                email_address = user.email_address,
                password = user.password,
                role = "user"
            };


            _context.users.Add(entity);
            await _context.SaveChangesAsync();
            return HttpStatusCode.OK;
        }

        [HttpGet("getUserById/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.users.SingleOrDefaultAsync(user => user.id == id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}
