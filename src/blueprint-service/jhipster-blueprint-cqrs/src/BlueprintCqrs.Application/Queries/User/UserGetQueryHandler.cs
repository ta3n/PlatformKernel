using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BlueprintCqrs.Dto;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlueprintCqrs.Application.Queries.User;

public class UserGetQueryHandler(
    UserManager<Domain.Entities.User> userManager,
    IMapper mapper
) : IRequestHandler<UserGetQuery, UserDto>
{
    private readonly UserManager<Domain.Entities.User> _userManager = userManager;
    private readonly IMapper _mapper = mapper;

    public async Task<UserDto> Handle(
        UserGetQuery request,
        CancellationToken cancellationToken
    )
    {
        var result = await _userManager.Users
            .Where(user => user.Login == request.Login)
            .Include(it => it.UserRoles)
            .ThenInclude(r => r.Role)
            .SingleOrDefaultAsync();
        return _mapper.Map<UserDto>(result);
    }
}
