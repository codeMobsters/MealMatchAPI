using System.Text.Json;
using MealMatchAPI.Models.DTOs;

namespace MealMatchAPI.Services
{
    public class EdamamApiService : IEdamamApiService
    {
        public async Task<RecipeTransfer> GetRecipesFromApi()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://random-recipes.p.rapidapi.com/ai-quotes/1"),
                Headers =
                {
                   
                },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStreamAsync();
                return (await JsonSerializer.DeserializeAsync<List<RecipeTransfer>>(body)).FirstOrDefault();
            }
        }
    }
}