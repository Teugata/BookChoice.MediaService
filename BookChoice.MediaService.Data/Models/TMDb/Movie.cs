using BookChoice.MediaService.Data.Models.YouTube;
using System.Text.Json.Serialization;

namespace BookChoice.MediaService.Data.Models.TMDb
{
    public partial class Movie
    {
        public bool Adult { get; set; }

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
        public string? ReleaseDate { get; set; }

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

        public IEnumerable<YouTubeVideo> YouTubeVideos { get; set; } = [];
    }
}
