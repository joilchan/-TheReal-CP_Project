using BroShopApp.Model;
using System.Text.Json.Serialization;

public class Review
{
    public int ProductId { get; set; }
    public int UserId { get; set; }

    [JsonPropertyName("Text")] // Явно говорим: в JSON это должно быть "Text" с большой буквы
    public string Text { get; set; }

    [JsonPropertyName("Rating")]
    public double Rating { get; set; }
    [JsonPropertyName("User")]
    public User User { get; set; }

    public string UserLogin => User?.Login ?? "Пользователь";
}