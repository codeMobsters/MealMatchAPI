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
    public class CommentsController : ControllerBase
    {
        private readonly IRepositories _repositories;
        private readonly IMapper _mapper;

        public CommentsController(IRepositories repositories, IMapper mapper)
        {
            _repositories = repositories;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<CommentTransfer>>> GetComments() // byUserId
        {
            if (_repositories.Comment == null)
            {
                return NotFound();
            }
            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            var comments = await _repositories.Comment.GetAllAsync(comment =>
                comment.UserId == GetIdFromToken(token));

            return comments.Select(comment => _mapper.Map<CommentTransfer>(comment)).ToList();
        }
        
        [HttpGet("recipe/{recipeId}")]
        [Authorize]
        public async Task<ActionResult<List<CommentTransfer>>> GetCommentByRecipeId(int recipeId)
        {
            if (_repositories.Recipe == null)
            {
                return NotFound();
            }
            
            var comments = await _repositories.Comment.GetAllAsync(c => 
                c.RecipeId == recipeId
            );

            return comments.Select(comment => _mapper.Map<CommentTransfer>(comment)).ToList();
        }
        
        [HttpGet("{commentId}")]
        [Authorize]
        public async Task<ActionResult<List<CommentTransfer>>> GetCommentByCommentId(int commentId)
        {
            if (_repositories.Comment == null)
            {
                return NotFound();
            }
            
            var comments = await _repositories.Comment.GetAllAsync(c => 
                c.CommentId == commentId
            );

            return comments.Select(comment => _mapper.Map<CommentTransfer>(comment)).ToList();
        }
        
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Comment>> PostComment(Comment request)
        {
            if (_repositories.Comment == null)
            {
                return Problem("Entity set 'CommentContext.Recipe'  is null.");
            }
            
            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            
            request.UserId = GetIdFromToken(token);
            request.CommentAt = DateTime.Now;
        
            await _repositories.Comment.AddAsync(request);
            await _repositories.Save();
        
            return CreatedAtAction("GetCommentByRecipeId", new { recipeId = request.RecipeId }, _mapper.Map<CommentTransfer>(request));
        }
        
        [HttpPut("{commentId}")]
        [Authorize]
        public async Task<IActionResult> UpdateComment(int commentId, [FromBody] Comment request)
        {
            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            if (commentId != request.CommentId || request.UserId != GetIdFromToken(token))
            {
                return BadRequest();
            }
            try
            {
                _repositories.Comment.Update(request);
                await _repositories.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{commentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            if (_repositories.Comment == null)
            {
                return NotFound();
            }
            
            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        
            var comment = await _repositories.Comment.GetFirstOrDefaultAsync(r =>
                r.CommentId == commentId && r.UserId == GetIdFromToken(token)
            );
            
            if (comment == null)
            {
                return NotFound();
            }
        
            _repositories.Comment.Delete(comment);
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