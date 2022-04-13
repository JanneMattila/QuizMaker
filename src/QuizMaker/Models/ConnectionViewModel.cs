using System.Text.Json.Serialization;

namespace QuizMaker.Models;

public class ConnectionViewModel
{
    [JsonPropertyName("counter")]
    public int Counter { get; set; }
}
