using System.Collections.Generic;
using BlueprintCqrs.Dto;
using JHipsterNet.Core.Pagination;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BlueprintCqrs.Application.Queries.User;

public class UserGetAllQuery : IRequest<(IHeaderDictionary, IEnumerable<UserDto>)>
{
    public IPageable Page { get; set; }
}
