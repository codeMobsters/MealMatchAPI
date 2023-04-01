using AutoMapper;
using MealMatchAPI.Models;
using MealMatchAPI.Models.DTOs;

namespace MealMatchAPI.Services;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<RecipeTransfer, Recipe>()
            .ForMember(d => d.Ingredients, opt => opt.ConvertUsing(new StringConverter()))
            .ForMember(d => d.CuisineType, opt => opt.ConvertUsing(new StringConverter()))
            .ForMember(d => d.DietLabels, opt => opt.ConvertUsing(new StringConverter()))
            .ForMember(d => d.DishType, opt => opt.ConvertUsing(new StringConverter()))
            .ForMember(d => d.HealthLabels, opt => opt.ConvertUsing(new StringConverter()))
            .ForMember(d => d.MealType, opt => opt.ConvertUsing(new StringConverter()))
            .ForMember(d => d.Instructions, opt => opt.ConvertUsing(new StringConverter()));

        CreateMap<Recipe, RecipeTransfer>()
            .ForMember(d => d.Ingredients, opt => opt.ConvertUsing(new ListConverter()))
            .ForMember(d => d.CuisineType, opt => opt.ConvertUsing(new ListConverter()))
            .ForMember(d => d.DietLabels, opt => opt.ConvertUsing(new ListConverter()))
            .ForMember(d => d.DishType, opt => opt.ConvertUsing(new ListConverter()))
            .ForMember(d => d.HealthLabels, opt => opt.ConvertUsing(new ListConverter()))
            .ForMember(d => d.MealType, opt => opt.ConvertUsing(new ListConverter()))
            .ForMember(d => d.Instructions, opt => opt.ConvertUsing(new ListConverter()))
            .ForMember(d => d.Likes, opt => opt.ConvertUsing(new LikeConverter()));
        
        CreateMap<NewRecipe, Recipe>()
            .ForMember(d => d.Ingredients, opt => opt.ConvertUsing(new StringConverter()))
            .ForMember(d => d.CuisineType, opt => opt.ConvertUsing(new StringConverter()))
            .ForMember(d => d.DietLabels, opt => opt.ConvertUsing(new StringConverter()))
            .ForMember(d => d.DishType, opt => opt.ConvertUsing(new StringConverter()))
            .ForMember(d => d.HealthLabels, opt => opt.ConvertUsing(new StringConverter()))
            .ForMember(d => d.MealType, opt => opt.ConvertUsing(new StringConverter()))
            .ForMember(d => d.Instructions, opt => opt.ConvertUsing(new StringConverter()));

        CreateMap<User, UserResponse>()
            .ForMember(u => u.HealthLabels, opt => opt.ConvertUsing(new ListConverter()))
            .ForMember(u => u.ProfileSettings, opt => opt.ConvertUsing(new ListConverter()))
            .ForMember(u => u.DietLabels, opt => opt.ConvertUsing(new ListConverter()))
            .ForMember(u => u.FavoriteRecipes, opt => opt.ConvertUsing(new FavoriteConverter()))
            .ForMember(u => u.OwnedRecipes, opt => opt.ConvertUsing(new RecipeConverter()));
        
        CreateMap<FavoriteRecipeTransfer, FavoriteRecipe>();
        CreateMap<FavoriteRecipe, FavoriteRecipeTransfer>();

        CreateMap<Comment, CommentTransfer>();
        CreateMap<CommentTransfer, Comment>();
    }
    
    private class StringConverter : IValueConverter<List<string>?, string?> {
        public string? Convert(List<string>? source, ResolutionContext context)
        {
            return source != null ? String.Join("<//>", source) : null;
        }
    }
    private class ListConverter : IValueConverter<string?, List<string>?> {
        public List<string>? Convert(string? source, ResolutionContext context)
        {
            return source?.Split("<//>").ToList();
        }
    }
    
    private class LikeConverter : IValueConverter<List<Like>?, List<int>?> {
        public List<int>? Convert(List<Like>? source, ResolutionContext context)
        {
            return source?.Select(l => l.UserId).ToList();
        }
    }
    
    private class FavoriteConverter : IValueConverter<List<FavoriteRecipe>?, int?> {
        public int? Convert(List<FavoriteRecipe>? source, ResolutionContext context)
        {
            return source?.Count;
        }
    }
    
    private class RecipeConverter : IValueConverter<List<Recipe>?, int?> {
        public int? Convert(List<Recipe>? source, ResolutionContext context)
        {
            return source?.Count;
        }
    }
}