using System.Text.Json.Serialization;

namespace BookChoice.MediaService.Data.Models
{
    public class ImageData
    {
        [JsonPropertyName("aspect_ratio")]
        public double AspectRatio { get; set; }

        [JsonPropertyName("file_path")]
        public string? FilePath { get; set; }

        public int Height { get; set; }

        public string? Iso_639_1 { get; set; }

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("vote_count")]
        public int VoteCount { get; set; }

        public int Width { get; set; }
    }
}
