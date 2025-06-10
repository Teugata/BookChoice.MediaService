using AutoMapper;
using BookChoice.MediaService.Data.Models.YouTube;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Options;

namespace BookChoice.MediaService.Business.Clients.YouTube
{
    public class YouTubeClient(IMapper _mapper, IOptions<YouTubeClientOptions> _options) : IYouTubeClient
    {
        public async Task<IEnumerable<YouTubeVideo>> SearchVideosAsync(string query, int maxResults, CancellationToken cancellationToken)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = _options.Value.ApiKey,
                ApplicationName = "BookChoice.MediaService"
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = query;
            searchListRequest.MaxResults = maxResults - 1;

            var searchListResponse = await searchListRequest.ExecuteAsync(cancellationToken);

            var videos = _mapper.Map<IList<YouTubeVideo>>(searchListResponse.Items);

            return videos;
        }
    }
}
