using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using MealMatchAPI.Models;
using MealMatchAPI.Models.APIModels;
using MealMatchAPI.Models.DTOs;
using MealMatchAPI.Repository.IRepository;
using MealMatchAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MealMatchAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly IRepositories _repositories;
        private readonly IMapper _mapper;
        private readonly IEdamamApiService _edamamApiService;
        private readonly IImageStorageService _imageStorageService;

        public RecipesController(IRepositories repositories, IMapper mapper, IEdamamApiService edamamApiService, IImageStorageService imageStorageService)
        {
            _repositories = repositories;
            _mapper = mapper;
            _edamamApiService = edamamApiService;
            _imageStorageService = imageStorageService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<RecipeTransfer>>> GetRecipes()
        {
            if (_repositories.Recipe == null)
            {
                return NotFound();
            }

            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");


            var recipes = await _repositories.Recipe.GetAllAsync(recipe => recipe.UserId == GetIdFromToken(token));
            return recipes.Select(recipe => _mapper.Map<RecipeTransfer>(recipe)).ToList();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<RecipeTransfer>> GetRecipe(int id)
        {
            if (_repositories.Recipe == null)
            {
                return NotFound();
            }

            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            var recipe = await _repositories.Recipe.GetFirstOrDefaultAsync(r =>
                r.UserId == GetIdFromToken(token) && r.RecipeId == id
            );

            if (recipe == null)
            {
                return NotFound();
            }

            return _mapper.Map<RecipeTransfer>(recipe);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<RecipeTransfer>> PostRecipe(RecipeTransfer newRecipe)
        {
            if (_repositories.Recipe == null)
            {
                return Problem("Entity set 'RecipeContext.Recipe'  is null.");
            }

            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            var isRecipeInDb = await _repositories.Recipe.Exists(r => r.SourceUrl == newRecipe.SourceUrl);
            Recipe recipe;
            
            if (isRecipeInDb)
            {
                recipe = await _repositories.Recipe.GetFirstOrDefaultAsync(r => r.SourceUrl == newRecipe.SourceUrl);
            }
            else
            {
                recipe = _mapper.Map<Recipe>(newRecipe);
                recipe.RecipeId = 0;
                recipe.UserId = 1;
                recipe.CreatedAt = DateTime.Now;
                recipe.PictureUrl = await _imageStorageService.UploadFileFromEdamam(recipe.PictureUrl);

                await _repositories.Recipe.AddAsync(recipe);
                await _repositories.Save();
            }
            
            var isFavoriteRecipeInDb = await _repositories.FavoriteRecipe.Exists(r =>
                r.UserId == GetIdFromToken(token) && r.RecipeId == recipe.RecipeId
            );

            if (!isFavoriteRecipeInDb)
            {
                var favoriteRecipe = new FavoriteRecipe
                {
                    UserId = GetIdFromToken(token),
                    RecipeId = recipe.RecipeId
                };
                await _repositories.FavoriteRecipe.AddAsync(favoriteRecipe);
                await _repositories.Save();
            }

            return CreatedAtAction("GetRecipe", new { id = recipe.RecipeId }, _mapper.Map<RecipeTransfer>(recipe));
        }
        
        [HttpPost("New")]
        [Authorize]
        [Consumes("multipart/form-data")] 
        public async Task<ActionResult<Recipe>> PostNewRecipe([FromForm]NewRecipe newRecipe)
        {
            if (_repositories.Recipe == null)
            {
                return Problem("Entity set 'RecipeContext.Recipe'  is null.");
            }

            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            var recipe = _mapper.Map<Recipe>(newRecipe);
            recipe.RecipeId = 0;
            recipe.UserId = GetIdFromToken(token);
            recipe.CreatedAt = DateTime.Now;
            recipe.PictureUrl = await _imageStorageService.UploadFile(newRecipe.RecipePicture.OpenReadStream(),
                newRecipe.RecipePicture.FileName, newRecipe.RecipePicture.ContentType);

            await _repositories.Recipe.AddAsync(recipe);
            await _repositories.Save();
            
            var favoriteRecipe = new FavoriteRecipe
            {
                UserId = GetIdFromToken(token),
                RecipeId = recipe.RecipeId
            };
            await _repositories.FavoriteRecipe.AddAsync(favoriteRecipe);
            await _repositories.Save();

            return CreatedAtAction("GetRecipe", new { id = recipe.RecipeId }, _mapper.Map<RecipeTransfer>(recipe));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutRecipe(int id, [FromBody] RecipeTransfer updatedRecipe)
        {
            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            if (id != updatedRecipe.RecipeId || updatedRecipe.UserId != GetIdFromToken(token))
            {
                return BadRequest();
            }

            var recipe = _mapper.Map<Recipe>(updatedRecipe);

            try
            {
                _repositories.Recipe.Update(recipe);
                await _repositories.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            if (_repositories.Recipe == null)
            {
                return NotFound();
            }

            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            var recipe = await _repositories.Recipe.GetFirstOrDefaultAsync(r =>
                r.RecipeId == id && r.UserId == GetIdFromToken(token)
            );

            if (recipe == null)
            {
                return NotFound();
            }

            _repositories.Recipe.Delete(recipe);
            await _repositories.Save();

            return NoContent();
        }
        
        [HttpDelete("Owned/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteOwnedRecipe(int id)
        {
            if (_repositories.Recipe == null)
            {
                return NotFound();
            }

            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            var recipe = await _repositories.Recipe.GetFirstOrDefaultAsync(r =>
                r.RecipeId == id && r.UserId == GetIdFromToken(token)
            );

            if (recipe == null)
            {
                return NotFound();
            }

            try
            {
                recipe.UserId = 1;
                _repositories.Recipe.Update(recipe);
                await _repositories.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("Edamam")]
        [Authorize]
        public async Task<ActionResult<EdamamResponse>> GetEdamamRecipes(
            [FromQuery] string? SearchTerm,
            [FromQuery] List<string>? cuisineType,
            [FromQuery] List<string>? dietLabel,
            [FromQuery] List<string>? healthLabel,
            [FromQuery] List<string>? mealType,
            [FromQuery] List<string>? dishType)
        {
            if (string.IsNullOrEmpty(SearchTerm) &&
                cuisineType.IsNullOrEmpty() &&
                dietLabel.IsNullOrEmpty() &&
                healthLabel.IsNullOrEmpty() &&
                mealType.IsNullOrEmpty() &&
                dishType.IsNullOrEmpty())
            {
                return await _edamamApiService.GetRecipesFromApi();
            }

            var recipeQuery = new RecipeQuery()
            {
                SearchTerm = SearchTerm,
                CuisineType = cuisineType,
                DietLabels = dietLabel,
                DishType = dishType,
                HealthLabels = healthLabel,
                MealType = mealType
            };

            try
            {
                return await _edamamApiService.GetQueriedRecipesFromApi(recipeQuery);
            }
            catch (Exception e)
            {
                return  NotFound();
            }
        }

        [HttpGet("Edamam/Next")]
        [Authorize]
        public async Task<ActionResult<EdamamResponse>> GetEdamamRecipes(string nextUrl)
        {
            return await _edamamApiService.GetNextRecipesFromApi(nextUrl);
        }

        private int GetIdFromToken(string token)
        {
            var jwtSecurityToken = new JwtSecurityToken(jwtEncodedString: token);
            string id = jwtSecurityToken.Claims.First(c => c.Type == "unique_name").Value;
            return Int32.Parse(id);
        }
    }
}