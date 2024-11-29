using AutoMapper;
using PieHandlerService.Core.Models;



namespace PieHandlerService.Infrastructure.Services.Pie;

public sealed class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<EndOfLineOrderRequest, Contracts.EndOfLineOrderRequest>()
            .ForMember(dest => dest.CertificateChain, opt => opt.MapFrom(src => src.CertificateChain))
            .ForMember(dest => dest.MixNumber, opt => opt.MapFrom(src => src.MixNumber))
            .ForMember(dest => dest.VehicleObject, opt => opt.MapFrom(src => src.VehicleObject))
            .ForMember(dest => dest.VinUnique, opt => opt.MapFrom(src => src.EndOfLine.VinUnique))
            .ForMember(dest => dest.EndOfLine, opt => opt.MapFrom(src => src.EndOfLine));

        CreateMap<EndOfLine, Contracts.EndOfLineData>()
            .ForMember(dest => dest.Orderer, opt => opt.MapFrom(src => src.Orderer))
            .ForMember(dest => dest.Pno12, opt => opt.MapFrom(src => src.Pno12))
            .ForMember(dest => dest.Ecus, opt => opt.MapFrom(src => src.Ecus));


        CreateMap<PreFlashOrderRequest, Contracts.PreFlashOrderRequest>()
            .ForMember(dest => dest.CertificateChain, opt => opt.MapFrom(src => src.CertificateChain))
            .ForMember(dest => dest.MixNumber, opt => opt.MapFrom(src => src.MixNumber))
            .ForMember(dest => dest.VehicleObject, opt => opt.MapFrom(src => src.VehicleObject))
            .ForMember(dest => dest.PreFlash, opt => opt.MapFrom(src => src.PreFlash));


        CreateMap<PreFlash, Contracts.PreFlashData>()
            .ForMember(dest => dest.Ecus, opt => opt.MapFrom(src => src.Ecus));

        CreateMap<EcuSoftwareDetail, Contracts.EcuSoftware>()
            .ForMember(dest => dest.EcuName, opt => opt.MapFrom(src => src.EcuName))
            .ForMember(dest => dest.Software, opt => opt.MapFrom(src => src.Software))
            .ForMember(dest => dest.AuthenticationTemplate, opt => opt.MapFrom(src => src.AuthenticationTemplate));

        CreateMap<EcuStaticSoftware, string>().ConvertUsing(x => (!string.IsNullOrEmpty(x.PartNumber) && x.PartNumber.Length >= 8) ?
            x.PartNumber.Substring(0, 8) : string.Empty);

        CreateMap<VehicleCodesRequest, Contracts.VehicleCodesRequest>()
            .ForMember(dest => dest.CertificateChain, opt => opt.MapFrom(src => src.CertificateChain))
            .ForMember(dest => dest.MixNumber, opt => opt.MapFrom(src => src.MixNumber))
            .ForMember(dest => dest.VehicleObject, opt => opt.MapFrom(src => src.VehicleObject));

        CreateMap<Contracts.EndOfLineOrderResponse, EndOfLineOrderResponse>()
            .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.Order))
            .ForMember(dest => dest.PieKeyManifest, opt => opt.MapFrom(src => src.PieKeyManifest))
            .ForMember(dest => dest.OeIdentifier, opt => opt.Ignore())
            .ForMember(dest => dest.OriginHash, opt => opt.Ignore())
            .ForMember(dest => dest.ProblemDetails, opt => opt.Ignore())
            .ForMember(dest => dest.MixNumber, opt => opt.Ignore());

        CreateMap<Contracts.PreFlashOrderResponse, PreFlashOrderResponse?>()
            .ConvertUsing((preFlashResponse, _) =>
            {
                if (preFlashResponse == null)
                {
                    return null;
                }
                return new PreFlashOrderResponse
                (
                    GetOrderFromPreFlashResponse(preFlashResponse),
                    preFlashResponse.KeyManifest
                );
            });


        CreateMap<Contracts.VehicleCodesResponse, VehicleCodesResponse>()
            .ConvertUsing((vehicleCodeResponse, _) =>
            {
                if (vehicleCodeResponse == null)
                {
                    return null;
                }
                return new VehicleCodesResponse
                (
                    GetOrderFromVehicleCodesResponse(vehicleCodeResponse),
                    vehicleCodeResponse.KeyManifest,
                    vehicleCodeResponse.PackageIdentity
                );
            });
    }

    private static Order GetOrderFromVehicleCodesResponse(Contracts.VehicleCodesResponse vehicleObjectResponse)
    {
        var ecuSet = new EcuSet();
        var order = new Order();
        order.LoadEcuSet = ecuSet;
        order.VehicleCodesSessionKeyId = vehicleObjectResponse.VehicleCodesSessionKeyId;
        order.VbfSessionKeyId = vehicleObjectResponse.VbfSessionKeyId;
        order.SecureOnBoardCodes = vehicleObjectResponse.SecureOnBoardCodes;
        order.VehicleCodesV2 = vehicleObjectResponse.VehicleCodesV2;
        return order;
    }

    private static Order GetOrderFromPreFlashResponse(Contracts.PreFlashOrderResponse preFlashOrderResponse)
    {
        var ecuSet = new EcuSet();
        ecuSet.Ecus = preFlashOrderResponse.Ecus;
        ecuSet.PackageIdentity = preFlashOrderResponse.PackageIdentity;
        var order = new Order();
        order.LoadEcuSet = ecuSet;
        order.VbfSessionKeyId = preFlashOrderResponse.VbfSessionKeyId;
        order.EncryptionData = preFlashOrderResponse.EncryptionData;
        order.VehicleCodesSessionKeyId = preFlashOrderResponse.VehicleCodesSessionKeyId;
        order.Version = preFlashOrderResponse.Version;
        return order;
    }

}