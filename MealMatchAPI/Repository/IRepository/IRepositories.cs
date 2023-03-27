namespace MealMatchAPI.Repository.IRepository;

public interface IRepositories
{
    IRecipeRepository Recipe { get; }
    IUserRepository User { get; }
    ICommentRepository Comment { get; }
    Task Save();
}