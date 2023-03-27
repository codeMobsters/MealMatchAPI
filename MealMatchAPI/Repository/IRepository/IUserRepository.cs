using MealMatchAPI.Models;
using MealMatchAPI.Models.DTOs;

namespace MealMatchAPI.Repository.IRepository;

public interface IUserRepository : IGenericRepositoryAsync<User>
{
    bool IsUniqueUser(string userName);
    Task<LoginResponse> Login(LoginRequest loginRequest);
    Task<User> Register(RegistrationRequest registrationRequest);
}