using AutoMapper;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

namespace Liberty.Reservation.Employee.WebAPI.Application.Mappings;

public class EmployeeMapperProfile : Profile
{
    public EmployeeMapperProfile()
    {
        CreateMap<Employee.Application.Contexts.Entities.Employee, CreateEmployeeRequest>()
            .ReverseMap();

        CreateMap<Employee.Application.Contexts.Entities.Employee, DataGetEmployeeManyResponse>()
            .ForMember(
                des => des.Id,
                opt => opt.MapFrom(
                    src => src.Id
                )
            )
            .ForMember(
                des => des.Name,
                opt => opt.MapFrom(
                    src => src.UserName
                )
            )
            // .ForMember(
            //     des => des.Kana,
            //     opt => opt.MapFrom(
            //         src => src.EmployeeMeta!.Kana
            //     )
            // )
            .ForMember(
                des => des.EMail,
                opt => opt.MapFrom(
                    src => src.Email
                )
            )
            .ForMember(
                des => des.Meta,
                opt => opt.MapFrom(
                    src => src.EmployeeMeta
                )
            )
            .ReverseMap();
    }
}
