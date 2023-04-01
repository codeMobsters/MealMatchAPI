namespace MealMatchAPI.Repository.IRepository;

public interface IRepositories
{
    IRecipeRepository Recipe { get; }
    IFavoriteRecipeRepository FavoriteRecipe { get; }
    IUserRepository User { get; }
    ICommentRepository Comment { get; }
    ILikeRepository Like { get; }
    Task Save();
}