namespace BookChoice.MediaService.Data.Models.YouTube
{
    public class YouTubeVideo
    {
        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? VideoId { get; set; }

        public Uri? Link => string.IsNullOrEmpty(VideoId)
            ? null
            : new Uri($"https://www.youtube.com/watch?v={VideoId}");
    }
}
