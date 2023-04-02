using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using MealMatchAPI.Data;
using MealMatchAPI.Models;
using MealMatchAPI.Models.DTOs;
using MealMatchAPI.Repository.IRepository;
using MealMatchAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MealMatchAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRepositories _repositories;
        private readonly IMapper _mapper;
        private readonly IImageStorageService _imageStorageService;

        public UsersController(IRepositories repositories, IMapper mapper, IImageStorageService imageStorageService)
        {
            _repositories = repositories;
            _mapper = mapper;
            _imageStorageService = imageStorageService;
        }

        [HttpPost("Authentication/login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest loginRequest)
        {
            var loginResponse = await _repositories.User.Login(loginRequest);
            if (loginResponse.Id == 0 || string.IsNullOrEmpty(loginResponse.Name) || string.IsNullOrEmpty(loginResponse.Token))
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }
            return loginResponse;
        }
        
        [HttpPost("Authentication/register")]
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
        public async Task<ActionResult<UserResponse>> GetUserById(int id)
        {
            if (_repositories.User == null)
            {
                return NotFound();
            }

            var user = await _repositories.User.GetFirstOrDefaultAsync(c => c.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            return _mapper.Map<UserResponse>(user);
        }

        [HttpGet("{id}/Recipes")]
        [Authorize]
        public async Task<ActionResult<List<RecipeTransfer>>> GetUserRecipesFromId(int id)
        {
            if (_repositories.Recipe == null)
            {
                return NotFound();
            }
            
            var recipes = await _repositories.Recipe.GetAllAsync(recipe => 
                recipe.UserId == id
            );

            return recipes.Select(recipe => _mapper.Map<RecipeTransfer>(recipe)).ToList();
        }
        
        [HttpGet("{id}/FavoriteRecipes")]
        [Authorize]
        public async Task<ActionResult<List<FavoriteRecipeTransfer>>> GetUserFavoriteRecipesFromId(int id)
        {
            if (_repositories.Recipe == null)
            {
                return NotFound();
            }

            var recipes = await _repositories.FavoriteRecipe.GetAllAsync(recipe => recipe.UserId == id);
            return recipes.Select(recipe => _mapper.Map<FavoriteRecipeTransfer>(recipe)).ToList();
        }
        
        [HttpGet("comments")]
        [Authorize]
        public async Task<ActionResult<List<CommentTransfer>>> GetUserComments()
        {
            if (_repositories.Comment == null)
            {
                return NotFound();
            }
            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            
            var comments = await _repositories.Comment.GetAllAsync(comment => 
                comment.UserId == GetIdFromToken(token)
            );

            return comments.Select(comment => _mapper.Map<CommentTransfer>(comment)).ToList();
        }
        
        [HttpPut("{id}/Update")]
        [Authorize]
        [Consumes("multipart/form-data")] 
        public async Task<ActionResult<string>> PutUser(int id, [FromForm] UserUpdateRequest updatedUser)
        {
            if (_repositories.User == null)
            {
                return NotFound();
            }
            
            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            
            if (id != GetIdFromToken(token))
            {
                return BadRequest();
            }

            var user =  await _repositories.User.GetFirstOrDefaultAsync(c => c.UserId == id);
            
            if (user == null)
            {
                return NotFound();
            }
            
            user.Name = updatedUser.Name ?? user.Name;

            if (updatedUser.ProfilePicture != null)
            {
                if (user.ProfilePictureUrl != null)
                {
                    await _imageStorageService.DeleteFile(user.ProfilePictureUrl);
                }
                
                user.ProfilePictureUrl = await _imageStorageService.UploadFile(updatedUser.ProfilePicture.OpenReadStream(),
                    updatedUser.ProfilePicture.FileName, updatedUser.ProfilePicture.ContentType);
            }

            user.ProfileSettings = updatedUser.ProfileSettings == null
                ? user.ProfileSettings
                : String.Join("<//>", updatedUser.ProfileSettings);
            
            user.HealthLabels = updatedUser.HealthLabels == null
                ? user.HealthLabels
                : String.Join("<//>", updatedUser.HealthLabels);
            
            user.DietLabels = updatedUser.DietLabels == null
                ? user.DietLabels
                : String.Join("<//>", updatedUser.DietLabels);
        
            try
            {
                _repositories.User.Update(user);
                await _repositories.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        
            return Ok(user.ProfilePictureUrl);
        }

        private int GetIdFromToken(string token)
        {
            var jwtSecurityToken = new JwtSecurityToken(jwtEncodedString: token);
            string id = jwtSecurityToken.Claims.First(c => c.Type == "unique_name").Value;
            return Int32.Parse(id);
        }
        
        
        
        // Likes
    }
}