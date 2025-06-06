using System.Text.Json.Serialization;

namespace BookChoice.MediaService.Data.Models
{
    public class ProductionCompany
    {
        public int Id { get; set; } = 0;

        [JsonPropertyName("logo_path")]
        public string? LogoPath { get; set; }
        public string? Name { get; set; }

        [JsonPropertyName("origin_country")]
        public string? OriginCountry { get; set; }
    }
}
