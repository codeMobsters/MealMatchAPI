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
    public class LikesController : ControllerBase
    {
        private readonly IRepositories _repositories;
        private readonly IMapper _mapper;

        public LikesController(IRepositories repositories, IMapper mapper)
        {
            _repositories = repositories;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PostLikedRecipe([FromBody] NewFavoriteRecipe newRecipe)
        {
            if (_repositories.Recipe == null)
            {
                return Problem("Entity set 'RecipeContext.Recipe'  is null.");
            }

            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            var isRecipeInDb = await _repositories.Recipe.Exists(r => r.RecipeId == newRecipe.RecipeId);
            var isLikedRecipeInDb = await _repositories.Like.Exists(r =>
                r.UserId == GetIdFromToken(token) && r.RecipeId == newRecipe.RecipeId
            );

            if (!isRecipeInDb || isLikedRecipeInDb)
            {
                return BadRequest();
            }

            var likedRecipe = new Like
            {
                UserId = GetIdFromToken(token),
                RecipeId = newRecipe.RecipeId
            };

            await _repositories.Like.AddAsync(likedRecipe);
            await _repositories.Save();

            return Ok();
        }

        

        [HttpDelete("{recipeId}")]
        [Authorize]
        public async Task<IActionResult> DeleteLikeFromRecipe(int recipeId)
        {
            if (_repositories.Recipe == null)
            {
                return NotFound();
            }

            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            var recipe = await _repositories.Like.GetFirstOrDefaultAsync(r =>
                r.RecipeId == recipeId && r.UserId == GetIdFromToken(token)
            );

            if (recipe == null)
            {
                return NotFound();
            }

            _repositories.Like.Delete(recipe);
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