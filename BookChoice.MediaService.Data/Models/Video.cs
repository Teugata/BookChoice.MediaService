using System.Text.Json.Serialization;

namespace BookChoice.MediaService.Data.Models
{
    public class Video
    {
        public string Id { get; set; }

        public string Iso_3166_1 { get; set; }

        public string Iso_639_1 { get; set; }

        public string Key { get; set; }

        public string Name { get; set; }

        public bool Official { get; set; }

        [JsonPropertyName("published_at")]
        public DateTime PublishedAt { get; set; }

        public string Site { get; set; }

        public int Size { get; set; }

        public string Type { get; set; }
    }
}
