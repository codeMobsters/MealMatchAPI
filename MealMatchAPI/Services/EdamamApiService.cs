using System.Text.Json;
using MealMatchAPI.Models.APIModels;
using MealMatchAPI.Models.DTOs;

namespace MealMatchAPI.Services
{
    public class EdamamApiService : IEdamamApiService
    {
        private readonly string _secretId;
        private readonly string _secretKey;

        public EdamamApiService(IConfiguration configuration)
        {
            _secretId = configuration.GetValue<String>("EdamamAPI_ID")!;
            _secretKey = configuration.GetValue<String>("EdamamAPI_Key")!;
        }

        public async Task<EdamamResponse> GetRecipesFromApi()
        {
            var cuisine = CuisineTypes[new Random().Next(CuisineTypes.Count)];
            var url =
                $"https://api.edamam.com/api/recipes/v2?type=public&app_id={_secretId}&app_key=%20{_secretKey}&cuisineType={cuisine}";
            
            return await CallApi(url);
        }

        public async Task<EdamamResponse> GetQueriedRecipesFromApi(RecipeQuery recipeQuery)
        {
            var url = $"https://api.edamam.com/api/recipes/v2?type=public&app_id={_secretId}&app_key=%20{_secretKey}";

            if (!String.IsNullOrEmpty(recipeQuery.SearchTerm))
            {
                url += $"&q={recipeQuery.SearchTerm.ToLower()}";
            }

            recipeQuery.CuisineType?.ForEach(cuisineType => url += $"&cuisineType={cuisineType.ToLower()}");
            recipeQuery.DietLabels?.ForEach(dietLabel => url += $"&diet={dietLabel.ToLower()}");
            recipeQuery.HealthLabels?.ForEach(healthLabel => url += $"&health={healthLabel.ToLower()}");
            recipeQuery.MealType?.ForEach(mealType => url += $"&mealType={mealType.ToLower()}");
            recipeQuery.DishType?.ForEach(dishType => url += $"&dishType={dishType.ToLower()}");

            return await CallApi(url);
        }

        public async Task<EdamamResponse> GetNextRecipesFromApi(string nextUrl)
        {
            return await CallApi(nextUrl);
        }

        private async Task<EdamamResponse> CallApi(string url)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };

            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStreamAsync();
            var modelResponse = await JsonSerializer.DeserializeAsync<ApiResponse>(body);
            return new EdamamResponse()
            {
                Url = modelResponse?.Links.Next.Url,
                Recipes = modelResponse?.Hits.Select(hit => hit.Recipe).ToList()
            };
        }
        
        private static readonly IReadOnlyList<string> CuisineTypes = new[] 
        {
            "american",
            "asian",
            "british",
            "caribbean",
            "central europe",
            "chinese",
            "eastern europe",
            "french",
            "greek",
            "indian",
            "italian",
            "japanese",
            "korean",
            "kosher",
            "mediterranean",
            "mexican",
            "middle eastern",
            "nordic",
            "south american",
            "south east asian",
            "world"
        };
    }
}