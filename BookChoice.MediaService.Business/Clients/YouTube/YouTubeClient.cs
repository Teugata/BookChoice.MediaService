using AutoMapper;
using BookChoice.MediaService.Data.Models.YouTube;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Options;

namespace BookChoice.MediaService.Business.Clients.YouTube
{
    public class YouTubeClient : IYouTubeClient
    {
        private readonly YouTubeClientOptions _options;
        private readonly IMapper _mapper;

        public YouTubeClient(IMapper mapper, IOptions<YouTubeClientOptions> options)
        {
            _mapper = mapper;
            _options = options.Value;

            if (string.IsNullOrEmpty(_options.ApiKey))
            {
                throw new ArgumentException("Api Key must be provided for YouTubeClient.", nameof(options));
            }
        }

        public async Task<IEnumerable<YouTubeVideo>> SearchVideosAsync(string query, int maxResults = 10)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = _options.ApiKey,
                ApplicationName = "BookChoice.MediaService"
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = query;
            searchListRequest.MaxResults = maxResults - 1;

            var searchListResponse = await searchListRequest.ExecuteAsync();

            var videos = _mapper.Map<IList<YouTubeVideo>>(searchListResponse.Items);

            return videos;
        }
    }
}
