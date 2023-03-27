using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MealMatchAPI.Data;
using MealMatchAPI.Models;
using MealMatchAPI.Models.DTOs;
using MealMatchAPI.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MealMatchAPI.Repository;

public class UserRepository : GenericRepositoryAsync<User>, IUserRepository
{
    private readonly ApplicationDbContext _db;
    private readonly string _secretKey;

    public UserRepository(ApplicationDbContext db, IConfiguration configuration) : base(db)
    {
        _db = db;
        _secretKey = configuration.GetValue<String>("ApiSettings:Secret");
    }

    public bool IsUniqueUser(string userName)
    {
        return _db.Users.All(user => user.Username != userName);
    }

    public async Task<LoginResponse> Login(LoginRequest loginRequest)
    {
        var passwordHasher = new PasswordHasher<User>();
        var user = await _db.Users.FirstOrDefaultAsync(user => user.Username.ToLower() == loginRequest.Username.ToLower());
        if (user == null)
        {
            return new LoginResponse()
            {
                Id = 0,
                Name = "",
                Token = ""
            };
        }
        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginRequest.Password);
        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            return new LoginResponse()
            {
                Id = 0,
                Name = "",
                Token = ""
            };
        }

        // Generate JWT token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.UserId.ToString())
            }),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var loginResponse = new LoginResponse()
        {
            Id = user.UserId,
            Name = user.Name,
            Token = tokenHandler.WriteToken(token)
        };
        
        return loginResponse;
    }

    public async Task<User> Register(RegistrationRequest registrationRequest)
    {
        var passwordHasher = new PasswordHasher<User>();
        var user = new User()
        {
            Username = registrationRequest.Username,
            Name = registrationRequest.Name,
            CreatedAt = DateTime.Now
        };
        user.PasswordHash = passwordHasher.HashPassword(user, registrationRequest.Password);

        await _db.AddAsync(user);
        await _db.SaveChangesAsync();

        return user;
    }
}