using System.Text.Json.Serialization;

namespace BookChoice.MediaService.Data.Models.TMDb
{
    public class ProductionCompany
    {
        [JsonPropertyName("logo_path")]
        public string? LogoPath { get; set; }
        public string? Name { get; set; }

        [JsonPropertyName("origin_country")]
        public string? OriginCountry { get; set; }
    }
}
