using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.User.WebAPI.Application.Models.Requests;

namespace Liberty.Reservation.User.WebAPI.Application.Mappings;

public class GMOPaymentMapperProfile : Profile
{
    public GMOPaymentMapperProfile()
    {
        CreateMap<GmoPaymentRequest, GmoPaymentResultRequest>()
            .ForMember(
                dest => dest.ShopId,
                opt => opt.MapFrom(x => x.ShopId)
            )
            .ForMember(
                dest => dest.ShopPass,
                opt => opt.MapFrom(x => x.ShopPass)
            )
            .ForMember(
                dest => dest.OrderId,
                opt => opt.MapFrom(x => x.OrderId)
            )
            .ForMember(
                dest => dest.AccessId,
                opt => opt.MapFrom(x => x.AccessId)
            )
            .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(x => x.Status)
            )
            .ForMember(
                dest => dest.JobCd,
                opt => opt.MapFrom(x => x.JobCd)
            )
            .ForMember(
                dest => dest.Amount,
                opt => opt.MapFrom(x => x.Amount)
            )
            .ForMember(
                dest => dest.Tax,
                opt => opt.MapFrom(x => x.Tax)
            )
            .ForMember(
                dest => dest.Currency,
                opt => opt.MapFrom(x => x.Currency)
            )
            .ForMember(
                dest => dest.Forward,
                opt => opt.MapFrom(x => x.Forward)
            )
            .ForMember(
                dest => dest.Method,
                opt => opt.MapFrom(x => x.Method)
            )
            .ForMember(
                dest => dest.PayTimes,
                opt => opt.MapFrom(x => x.PayTimes)
            )
            .ForMember(
                dest => dest.TranId,
                opt => opt.MapFrom(x => x.TranId)
            )
            .ForMember(
                dest => dest.Approve,
                opt => opt.MapFrom(x => x.Approve)
            )
            .ForMember(
                dest => dest.TranDate,
                opt => opt.MapFrom(x => x.TranDate)
            )
            .ForMember(
                dest => dest.ErrCode,
                opt => opt.MapFrom(x => x.ErrCode)
            )
            .ForMember(
                dest => dest.ErrInfo,
                opt => opt.MapFrom(x => x.ErrInfo)
            )
            .ForMember(
                dest => dest.PayType,
                opt => opt.MapFrom(x => x.PayType)
            );
    }
}
