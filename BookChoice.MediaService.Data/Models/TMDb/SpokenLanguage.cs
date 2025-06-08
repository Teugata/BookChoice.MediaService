using System.Text.Json.Serialization;

namespace BookChoice.MediaService.Data.Models.TMDb
{
    public class SpokenLanguage
    {
        [JsonPropertyName("english_name")]
        public string? EnglishName { get; set; }

        public string? Name { get; set; }
    }
}
