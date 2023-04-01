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
    public class FollowersController : ControllerBase
    {
        private readonly IRepositories _repositories;
        private readonly IMapper _mapper;

        public FollowersController(IRepositories repositories, IMapper mapper)
        {
            _repositories = repositories;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PostFollower([FromBody] FollowRequest request)
        {
            if (_repositories.Follower == null)
            {
                return Problem("Entity set 'RecipeContext.Recipe' is null.");
            }

            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            var followingUserId = GetIdFromToken(token);

            var isFollowedUserInDb = await _repositories.User.Exists(u => u.UserId == request.FollowedUserId);
            var isFollowRelationshipInDb = await _repositories.Follower.Exists(u =>
                u.FollowingUserId == followingUserId && u.FollowedUserId == request.FollowedUserId
            );

            if (!isFollowedUserInDb || isFollowRelationshipInDb || followingUserId == request.FollowedUserId)
            {
                return BadRequest();
            }

            var follower = new Follower
            {
                FollowingUserId = followingUserId,
                FollowedUserId = request.FollowedUserId
            };

            await _repositories.Follower.AddAsync(follower);
            await _repositories.Save();

            return Ok();
        }

        

        [HttpDelete("{followedUserId}")]
        [Authorize]
        public async Task<IActionResult> DeleteLikeFromRecipe(int followedUserId)
        {
            if (_repositories.Recipe == null)
            {
                return NotFound();
            }

            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            var follower = await _repositories.Follower.GetFirstOrDefaultAsync(u =>
                u.FollowedUserId == followedUserId && u.FollowingUserId == GetIdFromToken(token)
            );

            if (follower == null)
            {
                return NotFound();
            }

            _repositories.Follower.Delete(follower);
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