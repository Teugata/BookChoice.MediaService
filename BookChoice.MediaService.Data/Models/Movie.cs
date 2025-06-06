using System.Text.Json.Serialization;

namespace BookChoice.MediaService.Data.Models
{
    public partial class Movie
    {
        public bool Adult { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string? BackdropPath { get; set; }

        public long Budget { get; set; }

        public IEnumerable<Genre> Genres { get; set; } = [];

        public string? Homepage { get; set; }

        public int Id { get; set; }

        public Images? Images { get; set; }

        [JsonPropertyName("imdb_id")]
        public string? ImdbId { get; set; }

        [JsonPropertyName("original_language")]
        public string? OriginalLanguage { get; set; }

        [JsonPropertyName("original_title")]
        public string? OriginalTitle { get; set; }

        public string? Overview { get; set; }

        public double? Popularity { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("production_companies")]
        public IEnumerable<ProductionCompany> ProductionCompanies { get; set; } = [];

        [JsonPropertyName("production_countries")]
        public IEnumerable<ProductionCountry> ProductionCountries { get; set; } = [];

        [JsonPropertyName("release_date")]
        public DateTime? ReleaseDate { get; set; }

        public long Revenue { get; set; }

        public int? Runtime { get; set; }

        [JsonPropertyName("spoken_languages")]
        public IEnumerable<SpokenLanguage> SpokenLanguages { get; set; } = [];

        public string? Status { get; set; }

        public string? Tagline { get; set; }

        public string? Title { get; set; }

        public bool Video { get; set; }

        public IEnumerable<Video> Videos { get; set; } = [];

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("vote_count")]
        public int VoteCount { get; set; }
    }
}
