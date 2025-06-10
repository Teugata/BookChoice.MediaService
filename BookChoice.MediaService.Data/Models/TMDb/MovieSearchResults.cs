using System.Text.Json.Serialization;

namespace BookChoice.MediaService.Data.Models.TMDb
{
    public class MovieSearchResults
    {
        public int Page { get; set; }

        public IEnumerable<Movie> Results { get; set; } = [];

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("total_results")]
        public int TotalResults { get; set; }
    }
}
