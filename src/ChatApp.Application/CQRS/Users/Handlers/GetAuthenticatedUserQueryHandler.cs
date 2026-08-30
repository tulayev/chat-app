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
    public class GetAuthenticatedUserQueryHandler : IRequestHandler<GetAuthenticatedUserQuery, ApiResponse<UserDto>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public GetAuthenticatedUserQueryHandler(UserManager<AppUser> userManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<ApiResponse<UserDto>> Handle(GetAuthenticatedUserQuery query, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users
                .Where(x => x.Id == query.CurrentUserId)
                .ProjectToType<UserDto>(_mapper.Config)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
            {
                return ApiResponse<UserDto>.Fail("User is not found!");
            }

            return ApiResponse<UserDto>.Ok(user);
        }
    }
}
