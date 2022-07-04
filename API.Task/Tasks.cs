using System.Text.Json.Serialization;

namespace API.Task
{
    internal class Tasks
    {
        [JsonPropertyName("id")]
        public int id { get; set; }

        [JsonPropertyName("title")]
        public string title { get; set; }
    }
}