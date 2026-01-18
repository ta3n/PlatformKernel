using AutoMapper;
using Liberty.Reservation.Employee.Application.Contexts.Entities;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

namespace Liberty.Reservation.Employee.WebAPI.Application.Mappings;

public class AddressMapperProfile : Profile
{
    public AddressMapperProfile()
    {
        CreateMap<Address, CreateEmployeeRequest>()
            .ForMember(
                des => des.Tel,
                opt => opt.MapFrom(
                    src => src.Tel
                )
            )
            .ForMember(
                des => des.Mobile,
                opt => opt.MapFrom(
                    src => src.Mobile
                )
            )
            .ReverseMap();
    }
}
