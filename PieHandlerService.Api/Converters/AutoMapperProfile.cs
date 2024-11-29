using AutoMapper;
using PieHandlerService.Api.Contracts;
using PieHandlerService.Core.Models;

namespace PieHandlerService.Api.Converters;

public sealed class AutoMapperProfile : Profile
{

    public AutoMapperProfile()
    {
        CreateMap<OrderSpecificationRequest, SiigOrderQuerySpecification>()
            .ForMember(dest => dest.MixNumber, opt => opt.MapFrom(src => src.MixNumber))
            .ForMember(dest => dest.OrderType, opt => opt.MapFrom(src => src.OrderType))
            .ForMember(dest => dest.OeIdentifier, opt => opt.MapFrom(src => src.OeIdentifier));

        CreateMap<Contracts.PieMessageSpecification, Core.Models.PieMessageSpecification>()
            .ForMember(dest => dest.MixNumber, opt => opt.MapFrom(src => src.MixNumber))
            .ForMember(dest => dest.RequestType, opt => opt.MapFrom(src => src.RequestType))
            .ForMember(dest => dest.OeIdentifier, opt => opt.MapFrom(src => src.OeIdentifier));

        CreateMap<Contracts.BroadcastMessageSpecification, Core.Models.BroadcastMessageSpecification>()
            .ForMember(dest => dest.MixNumber, opt => opt.MapFrom(src => src.MixNumber))
            .ForMember(dest => dest.RequestType, opt => opt.MapFrom(src => src.RequestType))
            .ForMember(dest => dest.OeIdentifier, opt => opt.MapFrom(src => src.OeIdentifier));
    }

}