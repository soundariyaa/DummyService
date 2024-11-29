using AutoMapper;
using PieHandlerService.Core.Models;

namespace PieHandlerService.Infrastructure.Services.Converters;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile() {

        CreateMap<EndOfLineContext, EndOfLine>()
            .ForMember(dest => dest.Ecus, opt => opt.MapFrom(src => src.Ecus))
            .ForMember(dest => dest.Orderer, opt => opt.MapFrom(src => src.Orderer))
            .ForMember(dest => dest.VinUnique, opt => opt.MapFrom(src => src.VinUnique))
            .ForMember(dest => dest.Pno12, opt => opt.MapFrom(src => src.Pno12)).ReverseMap();

        CreateMap<PreFlashContext, PreFlash>()
            .ForMember(dest => dest.Ecus, opt => opt.MapFrom(src => src.Ecus)).ReverseMap();

    }
}