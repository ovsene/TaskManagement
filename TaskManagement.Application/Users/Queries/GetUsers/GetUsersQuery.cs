using MediatR;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Users.DTOs;

namespace TaskManagement.Application.Users.Queries.GetUsers
{
    public class GetUsersQuery : IRequest<BaseResponse<List<UserDto>>>
    {
    }
} 