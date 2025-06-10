namespace BookChoice.MediaService.Helpers
{
    public static class CacheKeyHelper
    {
        public static string CreateMovieKey(string id, bool includeYouTube, int maxYouTube)
            => $"movie:{Uri.EscapeDataString(id)}:yt:{includeYouTube}:max:{maxYouTube}";

        public static string CreateSearchKey(string query, int page)
            => $"query:{Uri.EscapeDataString(query)}:page:{page}";
    }
}