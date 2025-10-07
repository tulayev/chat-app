using Core.CQRS.Register.Commands;
using Core.Helpers;
using Core.Models;
using Core.Models.DTOs.Auth;
using Core.Services.Image;
using Core.Services.JwtToken;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Core.CQRS.Register.Handlers
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ApiResponse<AuthResponseDto>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IImageStoreService _imageStoreService;

        public RegisterUserCommandHandler(
            UserManager<AppUser> userManager,
            IJwtTokenService jwtTokenService,
            IImageStoreService imageStoreService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _imageStoreService = imageStoreService;
        }

        public async Task<ApiResponse<AuthResponseDto>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            var request = command.RegisterRequestDto;

            if (await _userManager.Users.AnyAsync(x => x.UserName == request.Username))
            {
                return ApiResponse<AuthResponseDto>.Fail("UserName is already taken.");
            }

            if (await _userManager.Users.AnyAsync(x => x.Email == request.Email))
            {
                return ApiResponse<AuthResponseDto>.Fail("Email is already in use.");
            }

            var user = new AppUser
            {
                UserName = request.Username,
                Email = request.Email
            };

            if (request.Avatar is not null && request.Avatar.Length > 0)
            {
                using var stream = request.Avatar.OpenReadStream();
                var uploaded = await _imageStoreService.UploadAsync(stream, request.Avatar.FileName);
                user.AvatarUrl = uploaded.Url;
                user.AvatarPublicId = uploaded.PublicId;
            }

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var error = string.Join("; ", result.Errors.Select(e => e.Description));
                return ApiResponse<AuthResponseDto>.Fail($"Registration failed: {error}");
            }

            var token = _jwtTokenService.CreateToken(user);

            var response = new AuthResponseDto(token, user.UserName!, user.Email!, user.AvatarUrl);
            return ApiResponse<AuthResponseDto>.Ok(response);
        }
    }
}
