using ChatApp.Application.CQRS.Users.Queries;
using ChatApp.Application.DTOs.User;
using ChatApp.Application.Helpers;
using ChatApp.Domain.Models;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.CQRS.Users.Handlers
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, ApiResponse<IReadOnlyCollection<UserDto>>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public GetUsersQueryHandler(UserManager<AppUser> userManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IReadOnlyCollection<UserDto>>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
        {
            var users = await _userManager.Users
                .Where(x => x.Id != query.CurrentUserId)
                .ProjectToType<UserDto>(_mapper.Config)
                .ToListAsync(cancellationToken);

            return ApiResponse<IReadOnlyCollection<UserDto>>.Ok(users);
        }
    }
}
