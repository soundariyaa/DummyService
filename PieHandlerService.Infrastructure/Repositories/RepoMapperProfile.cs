using AutoMapper;
using PieHandlerService.Core.Models;

namespace PieHandlerService.Infrastructure.Repositories;

public sealed class RepoMapperProfile : Profile
{
    public RepoMapperProfile()
    {
        CreateMap<SiigOrderQuerySpecification, Order.Data.SiigOrder>()
            .ForMember(dest => dest.MixNumber, opt => opt.MapFrom(src => src.MixNumber))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedUtcTs, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedUtcTs, opt => opt.Ignore())
            .ForMember(dest => dest.OrderResponse, opt => opt.Ignore())
            .ForMember(dest => dest.OrderStatus, opt => opt.Ignore())
            .ForMember(dest => dest.OrderType, opt => opt.Ignore())
            .ForMember(dest => dest.OeIdentifier, opt => opt.MapFrom(src => src.OeIdentifier)).ReverseMap();

        CreateMap<Core.Models.SiigOrder, Order.Data.SiigOrder>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.MixNumber, opt => opt.MapFrom(src => src.MixNumber))
            .ForMember(dest => dest.ModifiedUtcTs, opt => opt.MapFrom(src => src.ModifiedUtcTs))
            .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus))
            .ForMember(dest => dest.OrderType, opt => opt.MapFrom(src => src.OrderType))
            .ForMember(dest => dest.OrderResponse, opt => opt.MapFrom(src => src.OrderResponse))
            .ForMember(dest => dest.CreatedUtcTs, opt => opt.MapFrom(src => src.CreatedUtcTs))
            .ForMember(dest => dest.OeIdentifier, opt => opt.MapFrom(src => src.OeIdentifier)).ReverseMap();

        CreateMap<Core.Models.PieResponseMessage, Message.Data.PieOrderMetadata>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.MixNumber, opt => opt.MapFrom(src => src.MixNumber))
            .ForMember(dest => dest.IsPriority, opt => opt.MapFrom(src => src.IsPriority))
            .ForMember(dest => dest.ModifiedUtcMs, opt => opt.MapFrom(src => src.ModifiedUtcMs))
            .ForMember(dest => dest.CreatedUtcMs, opt => opt.MapFrom(src => src.CreatedUtcMs))
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.RequestType, opt => opt.MapFrom(src => src.RequestType))
            .ForMember(dest => dest.ContentHash, opt => opt.MapFrom(src => src.ContentHash))
            .ForMember(dest => dest.OriginHash, opt => opt.MapFrom(src => src.OriginHash))
            .ForMember(dest => dest.OeId, opt => opt.MapFrom(src => src.OeId)).ReverseMap();

        CreateMap<Core.Models.PieMessageSpecification, Message.Data.PieOrderMetadata>()
            .ForMember(dest => dest.MixNumber, opt => opt.MapFrom(src => src.MixNumber))
            .ForMember(dest => dest.ModifiedUtcMs, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedUtcMs, opt => opt.Ignore())
            .ForMember(dest => dest.FileName, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsPriority, opt => opt.Ignore())
            .ForMember(dest => dest.ContentHash, opt => opt.Ignore())
            .ForMember(dest => dest.OriginHash, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.RequestType, opt => opt.Ignore())
            .ForMember(dest => dest.OeId, opt => opt.MapFrom(src => src.OeIdentifier)).ReverseMap();

        CreateMap<Core.Models.BroadcastContextMessage, Message.Data.BroadcastMetadata>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.MixNumber, opt => opt.MapFrom(src => src.MixNumber))
            .ForMember(dest => dest.IsPriority, opt => opt.MapFrom(src => src.IsPriority))
            .ForMember(dest => dest.ModifiedUtcMs, opt => opt.MapFrom(src => src.ModifiedUtcMs))
            .ForMember(dest => dest.CreatedUtcMs, opt => opt.MapFrom(src => src.CreatedUtcMs))
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.RequestType, opt => opt.MapFrom(src => src.RequestType))
            .ForMember(dest => dest.ContentHash, opt => opt.MapFrom(src => src.ContentHash))
            .ForMember(dest => dest.OriginHash, opt => opt.MapFrom(src => src.OriginHash))
            .ForMember(dest => dest.OeId, opt => opt.MapFrom(src => src.OeId)).ReverseMap();

        CreateMap<Core.Models.BroadcastMessageSpecification, Message.Data.BroadcastMetadata>()
            .ForMember(dest => dest.MixNumber, opt => opt.MapFrom(src => src.MixNumber))
            .ForMember(dest => dest.ModifiedUtcMs, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedUtcMs, opt => opt.Ignore())
            .ForMember(dest => dest.FileName, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsPriority, opt => opt.Ignore())
            .ForMember(dest => dest.ContentHash, opt => opt.Ignore())
            .ForMember(dest => dest.OriginHash, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.RequestType, opt => opt.Ignore())
            .ForMember(dest => dest.OeId, opt => opt.MapFrom(src => src.OeIdentifier)).ReverseMap();
    }
}