using System.Text.Json.Serialization;
using MealMatchAPI.Models.DTOs;

namespace MealMatchAPI.Models.APIModels;

public class ApiResponse
{
    [JsonPropertyName("from")]
    public int From { get; set; }
    [JsonPropertyName("to")]
    public int To { get; set; }
    [JsonPropertyName("count")]
    public int Count { get; set; }
    [JsonPropertyName("_links")]
    public Link Links { get; set; }
    [JsonPropertyName("hits")]
    public List<Hit> Hits { get; set; }
}

public class Link
{
    [JsonPropertyName("next")]
    public Next Next { get; set; }
}

public class Next
{
    [JsonPropertyName("href")]
    public string Url { get; set; }
}

public class Hit
{
    [JsonPropertyName("recipe")]
    public RecipeTransfer Recipe { get; set; }
}