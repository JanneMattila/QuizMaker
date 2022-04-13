using System.Text.Json.Serialization;

namespace QuizUserSimulator.Interfaces;

public class Connection
{
    [JsonPropertyName("counter")]
    public int Counter { get; set; }
}
