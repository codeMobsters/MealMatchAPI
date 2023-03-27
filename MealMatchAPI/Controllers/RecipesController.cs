using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using MealMatchAPI.Data;
using MealMatchAPI.Models;
using MealMatchAPI.Models.DTOs;
using MealMatchAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MealMatchAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly IRepositories _repositories;
        private readonly IMapper _mapper;

        public RecipesController(IRepositories repositories, IMapper mapper)
        {
            _repositories = repositories;
            _mapper = mapper;
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
            
        
            var recipes =await _repositories.Recipe.GetAllAsync(recipe => recipe.UserId == GetIdFromToken(token));
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
            
            var recipe = await _repositories.Recipe.GetFirstOrDefaultAsync(c => 
                c.UserId == GetIdFromToken(token) && c.RecipeId == id
            );
        
            if (recipe == null)
            {
                return NotFound();
            }
        
            return _mapper.Map<RecipeTransfer>(recipe);
        }
        
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Recipe>> PostRecipe(RecipeTransfer newRecipe)
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
        
            await _repositories.Recipe.AddAsync(recipe);
            await _repositories.Save();
        
            return CreatedAtAction("GetRecipe", new { id = recipe.RecipeId }, recipe);
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
        
        private int GetIdFromToken(string token)
        {
            var jwtSecurityToken = new JwtSecurityToken(jwtEncodedString: token);
            string id = jwtSecurityToken.Claims.First(c => c.Type == "unique_name").Value;
            return Int32.Parse(id);
        }
    }
}