using System.Text.Json.Serialization;

namespace BookChoice.MediaService.Data.Models
{
    public class ProductionCountry
    {
        [JsonPropertyName("iso_3166_1")]
        public string? Iso31661 { get; set; }
        public string? Name { get; set; }
    }
}
