using System.Text.Json.Serialization;

namespace BookChoice.MediaService.Data.Models.TMDb
{
    public class ImageData
    {
        [JsonPropertyName("aspect_ratio")]
        public double AspectRatio { get; set; }

        [JsonPropertyName("file_path")]
        public string? FilePath { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public Uri? Link => string.IsNullOrEmpty(FilePath) ? null :
            new Uri($"https://image.tmdb.org/t/p/original/{FilePath}");
    }
}
