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
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteRecipesController : ControllerBase
    {
        private readonly IRepositories _repositories;
        private readonly IMapper _mapper;

        public FavoriteRecipesController(IRepositories repositories, IMapper mapper)
        {
            _repositories = repositories;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<FavoriteRecipeTransfer>>> GetRecipes()
        {
            if (_repositories.Recipe == null)
            {
                return NotFound();
            }

            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");


            var recipes = await _repositories.FavoriteRecipe.GetAllAsync(recipe => recipe.UserId == GetIdFromToken(token));
            return recipes.Select(recipe => _mapper.Map<FavoriteRecipeTransfer>(recipe)).ToList();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<FavoriteRecipeTransfer>> GetFavoriteRecipe(int id)
        {
            if (_repositories.Recipe == null)
            {
                return NotFound();
            }

            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            var recipe = await _repositories.FavoriteRecipe.GetFirstOrDefaultAsync(r =>
                r.UserId == GetIdFromToken(token) && r.RecipeId == id
            );

            if (recipe == null)
            {
                return NotFound();
            }

            return _mapper.Map<FavoriteRecipeTransfer>(recipe);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<FavoriteRecipe>> PostFavoriteRecipe([FromBody] NewFavoriteRecipe newRecipe)
        {
            if (_repositories.Recipe == null)
            {
                return Problem("Entity set 'RecipeContext.Recipe'  is null.");
            }

            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            var recipe = await _repositories.Recipe.GetByIdAsync(newRecipe.RecipeId);

            if (recipe == null)
            {
                return BadRequest();
            }

            var favoriteRecipe = new FavoriteRecipe
            {
                UserId = GetIdFromToken(token),
                RecipeId = newRecipe.RecipeId
            };

            await _repositories.FavoriteRecipe.AddAsync(favoriteRecipe);
            await _repositories.Save();

            return CreatedAtAction("GetFavoriteRecipe", new { id = favoriteRecipe.FavoriteRecipeId}, favoriteRecipe);
        }

        

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteFavoriteRecipe(int id)
        {
            if (_repositories.Recipe == null)
            {
                return NotFound();
            }

            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            var recipe = await _repositories.FavoriteRecipe.GetFirstOrDefaultAsync(r =>
                r.RecipeId == id && r.UserId == GetIdFromToken(token)
            );

            if (recipe == null)
            {
                return NotFound();
            }

            _repositories.FavoriteRecipe.Delete(recipe);
            await _repositories.Save();

            return NoContent();
        }


        private int GetIdFromToken(string token)
        {
            var jwtSecurityToken = new JwtSecurityToken(jwtEncodedString: token);
            string id = jwtSecurityToken.Claims.First(c => c.Type == "unique_name").Value;
            return Int32.Parse(id);
        }
    }
}