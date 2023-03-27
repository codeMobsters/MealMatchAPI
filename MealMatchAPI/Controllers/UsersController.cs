using AutoMapper;
using MealMatchAPI.Data;
using MealMatchAPI.Models.DTOs;
using MealMatchAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace MealMatchAPI.Controllers
{
    [Route("api/UsersAuthentication")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRepositories _repositories;
        private readonly IMapper _mapper;

        public UsersController(ApplicationDbContext context, IRepositories repositories, IMapper mapper)
        {
            _repositories = repositories;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest loginRequest)
        {
            var loginResponse = await _repositories.User.Login(loginRequest);
            if (loginResponse.Id == 0 || string.IsNullOrEmpty(loginResponse.Name) || string.IsNullOrEmpty(loginResponse.Token))
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }

            return loginResponse;

        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequest registrationRequest)
        {
            if (!_repositories.User.IsUniqueUser(registrationRequest.Username))
            {
                return BadRequest(new { message = "Please choose a new Username" });
            }

            var user = await _repositories.User.Register(registrationRequest);
            if (user == null)
            {
                return BadRequest(new { message = "Error while registering" });
            }

            return Ok();
        }
        
        
    }
}