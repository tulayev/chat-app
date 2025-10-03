using Core.CQRS.Register.Commands;
using Core.Data.Repositories.User;
using Core.Models;
using Core.Models.DTOs.Auth;
using Core.Services.Image;
using Core.Services.JwtToken;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Core.CQRS.Register.Handlers
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, AuthResponse>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IImageStoreService _imageStoreService;
        private readonly IUserRepository _userRepository;

        public RegisterUserHandler(
            UserManager<AppUser> userManager,
            IJwtTokenService jwtTokenService,
            IImageStoreService imageStoreService,
            IUserRepository userRepository)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _imageStoreService = imageStoreService;
            _userRepository = userRepository;
        }

        public async Task<AuthResponse> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            var request = command.RegisterRequestDto;

            if (await _userRepository.IsUserNameTakenAsync(request.Username))
            {
                throw new InvalidOperationException("UserName is already taken.");
            }

            if (await _userRepository.IsEmailTakenAsync(request.Email))
            {
                throw new InvalidOperationException("Email is already in use.");
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
                throw new InvalidOperationException($"Registration failed: {error}");
            }

            var token = _jwtTokenService.CreateToken(user);

            return new AuthResponse(token, user.UserName!, user.Email!, user.AvatarUrl);
        }
    }
}
