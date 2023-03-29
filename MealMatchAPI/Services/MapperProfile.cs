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
            .ForMember(d => d.Instructions, opt => opt.ConvertUsing(new ListConverter()));

        CreateMap<NewRecipe, Recipe>()
            .ForMember(d => d.Ingredients, opt => opt.ConvertUsing(new StringConverter()))
            .ForMember(d => d.CuisineType, opt => opt.ConvertUsing(new StringConverter()))
            .ForMember(d => d.DietLabels, opt => opt.ConvertUsing(new StringConverter()))
            .ForMember(d => d.DishType, opt => opt.ConvertUsing(new StringConverter()))
            .ForMember(d => d.HealthLabels, opt => opt.ConvertUsing(new StringConverter()))
            .ForMember(d => d.MealType, opt => opt.ConvertUsing(new StringConverter()))
            .ForMember(d => d.Instructions, opt => opt.ConvertUsing(new StringConverter()));
        
        CreateMap<Recipe, NewRecipe>()
            .ForMember(d => d.Ingredients, opt => opt.ConvertUsing(new ListConverter()))
            .ForMember(d => d.CuisineType, opt => opt.ConvertUsing(new ListConverter()))
            .ForMember(d => d.DietLabels, opt => opt.ConvertUsing(new ListConverter()))
            .ForMember(d => d.DishType, opt => opt.ConvertUsing(new ListConverter()))
            .ForMember(d => d.HealthLabels, opt => opt.ConvertUsing(new ListConverter()))
            .ForMember(d => d.MealType, opt => opt.ConvertUsing(new ListConverter()))
            .ForMember(d => d.Instructions, opt => opt.ConvertUsing(new ListConverter()));

        
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
}