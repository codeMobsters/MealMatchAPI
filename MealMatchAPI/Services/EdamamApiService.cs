using System.Text.Json;
using MealMatchAPI.Models.APIModels;
using MealMatchAPI.Models.DTOs;

namespace MealMatchAPI.Services
{
    public class EdamamApiService : IEdamamApiService
    {
        private readonly string _secretId;
        private readonly string _secretKey;
        private readonly List<string> _cuisineTypes;

        public EdamamApiService(IConfiguration configuration)
        {
            _secretId = configuration.GetValue<String>("EdamamAPI_ID");
            _secretKey = configuration.GetValue<String>("EdamamAPI_Key");
            _cuisineTypes = new List<string>
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

        public async Task<List<RecipeTransfer>> GetRecipesFromApi()
        {
            var cuisine = _cuisineTypes[new Random().Next(_cuisineTypes.Count)];
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri =
                    new Uri(
                        $"https://api.edamam.com/api/recipes/v2?type=public&app_id={_secretId}&app_key=%20{_secretKey}&cuisineType={cuisine}"),
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStreamAsync();
                var modelResponse = await JsonSerializer.DeserializeAsync<ApiResponse>(body);
                return modelResponse.Hits.Select(hit => hit.Recipe).ToList();
            }
        }

        public async Task<List<RecipeTransfer>> GetQueriedRecipesFromApi(RecipeQuery recipeQuery)
        {
            var query = "";
            
            recipeQuery.CuisineType?.ForEach(cuisineType => query += $"&cuisineType={cuisineType.ToLower()}");
            recipeQuery.DietLabels?.ForEach(dietLabel => query += $"&diet={dietLabel.ToLower()}");
            recipeQuery.HealthLabels?.ForEach(healthLabel => query += $"&health={healthLabel.ToLower()}");
            recipeQuery.MealType?.ForEach(mealType => query += $"&mealType={mealType.ToLower()}");
            recipeQuery.DishType?.ForEach(dishType => query += $"&dishType={dishType.ToLower()}");
            
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri =
                    new Uri(
                        $"https://api.edamam.com/api/recipes/v2?type=public&app_id={_secretId}&app_key=%20{_secretKey}{query}"),
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStreamAsync();
                var modelResponse = await JsonSerializer.DeserializeAsync<ApiResponse>(body);
                return modelResponse.Hits.Select(hit => hit.Recipe).ToList();
            }
        }
    }
}