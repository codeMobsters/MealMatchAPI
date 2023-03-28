using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using MealMatchAPI.Data;
using MealMatchAPI.Models;
using MealMatchAPI.Models.DTOs;
using MealMatchAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MealMatchAPI.Controllers
{
    [Route("api/UsersAuthentication")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRepositories _repositories;
        private readonly IMapper _mapper;

        public UsersController(IRepositories repositories, IMapper mapper)
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
        
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            if (_repositories.User == null)
            {
                return NotFound();
            }
            
            return await _repositories.User.GetAllAsync();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            if (_repositories.User == null)
            {
                return NotFound();
            }

            return await _repositories.User.GetFirstOrDefaultAsync(c => c.UserId == id);;
        }
        
        [HttpGet("favoriterecipes")]
        [Authorize]
        public async Task<ActionResult<List<RecipeTransfer>>> GetFavoriteRecipes()
        {
            if (_repositories.Recipe == null)
            {
                return NotFound();
            }
            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            
            var user = await _repositories.User.GetFirstOrDefaultAsync(user => 
                user.UserId == GetIdFromToken(token)
            );
            if (user == null)
            {
                return NotFound();
            }

            var favorites = user.Favorites?.Split("<//>").ToList();
            if (favorites == null)
            {
                return NotFound();
            }

            var recipes = favorites.Select(fav =>
                _repositories.Recipe.GetFirstOrDefaultAsync(recipe =>
                    recipe.RecipeId == Int32.Parse(fav))).ToList();

            if (recipes == null)
            {
                return NotFound();
            }

            return recipes.Select(recipe => _mapper.Map<RecipeTransfer>(recipe)).ToList();
        }
        
        [HttpGet("recipes")]
        [Authorize]
        public async Task<ActionResult<List<Recipe>>> GetRecipes()
        {
            if (_repositories.Recipe == null)
            {
                return NotFound();
            }
            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            
            var recipes = await _repositories.Recipe.GetAllAsync(recipe => 
                recipe.UserId == GetIdFromToken(token)
            );

            return Ok(recipes);
        }
        
        [HttpGet("comments")]
        [Authorize]
        public async Task<ActionResult<List<Comment>>> GetComments()
        {
            if (_repositories.Comment == null)
            {
                return NotFound();
            }
            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            
            var comments = await _repositories.Comment.GetAllAsync(comment => 
                comment.UserId == GetIdFromToken(token)
            );

            return Ok(comments);
        }
        
        private int GetIdFromToken(string token)
        {
            var jwtSecurityToken = new JwtSecurityToken(jwtEncodedString: token);
            string id = jwtSecurityToken.Claims.First(c => c.Type == "unique_name").Value;
            return Int32.Parse(id);
        }
    }
}