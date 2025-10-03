using Core.CQRS.Login.Queries;
using Core.Data.Repositories.User;
using Core.Models;
using Core.Models.DTOs.Auth;
using Core.Services.JwtToken;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Core.CQRS.Login.Handlers
{
    public class LoginUserHandler : IRequestHandler<LoginUserQuery, AuthResponseDto>
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public LoginUserHandler(
            SignInManager<AppUser> signInManager, 
            IUserRepository userRepository, 
            IJwtTokenService jwtTokenService)
        {
            _signInManager = signInManager;
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<AuthResponseDto> Handle(LoginUserQuery query, CancellationToken cancellationToken)
        {
            var request = query.LoginRequestDto;

            var user = await _userRepository.FindByNameOrEmailAsync(request.UsernameOrEmail)
                ?? throw new InvalidOperationException("User is not found.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Login or password is incorrect.");
            }

            var token = _jwtTokenService.CreateToken(user);

            return new AuthResponseDto(token, user.UserName!, user.Email!, user.AvatarUrl);
        }
    }
}
