using ChatApp.Application.Common.Interfaces.Repositories;
using ChatApp.Application.Common.Interfaces.Security;
using ChatApp.Application.CQRS.Login.Queries;
using ChatApp.Application.Helpers;
using ChatApp.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.CQRS.Login.Handlers
{
    public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, ApiResponse<string>>
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenService _jwtTokenService;

        public LoginUserQueryHandler(
            SignInManager<AppUser> signInManager, 
            IUnitOfWork unitOfWork, 
            IJwtTokenService jwtTokenService)
        {
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<ApiResponse<string>> Handle(LoginUserQuery query, CancellationToken cancellationToken)
        {
            var request = query.LoginRequestDto;

            var user = await _unitOfWork.GetQueryable<AppUser>()
                .FirstOrDefaultAsync(x => x.Email == request.UsernameOrEmail || x.UserName == request.UsernameOrEmail, 
                    cancellationToken: cancellationToken);

            if (user == null)
            {
                return ApiResponse<string>.Fail("User is not found");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return ApiResponse<string>.Fail("Login or password is incorrect.");
            }

            var token = _jwtTokenService.CreateToken(user);

            if (string.IsNullOrWhiteSpace(token))
            {
                return ApiResponse<string>.Fail("Something went wrong");
            }

            return ApiResponse<string>.Ok(token);
        }
    }
}
