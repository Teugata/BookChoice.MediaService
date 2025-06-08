using AutoMapper;
using BookChoice.MediaService.Data.Models.YouTube;
using Google.Apis.YouTube.v3.Data;

public class YouTubeMappingProfile : Profile
{
    public YouTubeMappingProfile()
    {
        CreateMap<SearchResult, YouTubeVideo>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Snippet.Title))
            .ForMember(dest => dest.VideoId, opt => opt.MapFrom(src => src.Id.VideoId))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Snippet.Description));
    }
}