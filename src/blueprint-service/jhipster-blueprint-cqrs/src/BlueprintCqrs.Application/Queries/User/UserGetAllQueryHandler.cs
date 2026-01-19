using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BlueprintCqrs.Domain.Services.Interfaces;
using BlueprintCqrs.Dto;
using BlueprintCqrs.Infrastructure.Web.Rest.Utilities;
using JHipsterNet.Core.Pagination.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlueprintCqrs.Application.Queries.User;

public class UserGetAllQueryHandler(
    UserManager<Domain.Entities.User> userManager,
    IUserService userService,
    IMapper mapper,
    IMailService mailService
) : IRequestHandler<UserGetAllQuery, (IHeaderDictionary, IEnumerable<UserDto>)>
{
    private readonly UserManager<Domain.Entities.User> _userManager = userManager;
    private readonly IMapper _mapper = mapper;

    public async Task<(IHeaderDictionary, IEnumerable<UserDto>)> Handle(
        UserGetAllQuery request,
        CancellationToken cancellationToken
    )
    {
        var page = await _userManager.Users
            .Include(it => it.UserRoles)
            .ThenInclude(r => r.Role)
            .UsePageableAsync(request.Page);
        var userDtos = page.Content.Select(user => _mapper.Map<UserDto>(user));
        var headers = page.GeneratePaginationHttpHeaders();
        return (headers, userDtos);
    }
}
