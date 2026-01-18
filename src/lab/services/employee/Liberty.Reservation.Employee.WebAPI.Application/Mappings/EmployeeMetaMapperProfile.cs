using AutoMapper;
using Liberty.Reservation.Employee.Application.Contexts.Entities;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

namespace Liberty.Reservation.Employee.WebAPI.Application.Mappings;

public class EmployeeMetaMapperProfile : Profile
{
    public EmployeeMetaMapperProfile()
    {
        CreateMap<EmployeeMeta, CreateEmployeeRequest>()
            .ForMember(
                des => des.Name,
                opt => opt.MapFrom(
                    src => src.Name
                )
            )
            .ForMember(
                des => des.Kana,
                opt => opt.MapFrom(
                    src => src.Kana
                )
            )
            .ForMember(
                des => des.Gender,
                opt => opt.MapFrom(
                    src => src.Gender
                )
            )
            .ForMember(
                des => des.BirthDay,
                opt => opt.MapFrom(
                    src => src.Birthday
                )
            )
            .ReverseMap()
            .ForMember(
                des => des.Birthday,
                opt => opt.MapFrom(
                    src => src.BirthDay!.Value.ToDateTime(TimeOnly.MinValue)
                )
            );

        CreateMap<EmployeeMeta, DataEmployeeMeta>()
            .ForMember(
                des => des.Kana,
                opt => opt.MapFrom(
                    src => src.Kana
                )
            )
            .ForMember(
                des => des.BirthDay,
                opt => opt.MapFrom(
                    src => src.Birthday!.Value
                )
            );
    }
}
