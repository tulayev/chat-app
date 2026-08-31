using ChatApp.Application.Common.Interfaces.Images;
using ChatApp.Application.CQRS.Profile.Commands;
using ChatApp.Application.DTOs.User;
using ChatApp.Application.Helpers;
using ChatApp.Domain.Models;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.CQRS.Profile.Handlers
{
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, ApiResponse<UserDto>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IImageStoreService _imageStoreService;

        public UpdateProfileCommandHandler(UserManager<AppUser> userManager, IImageStoreService imageStoreService)
        {
            _userManager = userManager;
            _imageStoreService = imageStoreService;
        }

        public async Task<ApiResponse<UserDto>> Handle(UpdateProfileCommand command, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(command.UserId.ToString())
                ?? throw new Exception("User not found");

            var request = command.Request;

            if (!string.Equals(user.UserName, request.Username, StringComparison.OrdinalIgnoreCase))
            {
                if (await _userManager.Users.AnyAsync(x => x.UserName == request.Username && x.Id != command.UserId, cancellationToken))
                {
                    return ApiResponse<UserDto>.Fail("Username is already taken.");
                }

                user.UserName = request.Username;
            }

            if (request.Avatar is not null && request.Avatar.Length > 0)
            {
                var oldAvatarPublicId = user.AvatarPublicId;

                using var stream = request.Avatar.OpenReadStream();
                var uploaded = await _imageStoreService.UploadAsync(stream, request.Avatar.FileName);
                user.AvatarUrl = uploaded.Url;
                user.AvatarPublicId = uploaded.PublicId;

                if (!string.IsNullOrWhiteSpace(oldAvatarPublicId))
                {
                    await _imageStoreService.DeleteAsync(oldAvatarPublicId);
                }
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var error = string.Join("; ", result.Errors.Select(e => e.Description));
                return ApiResponse<UserDto>.Fail($"Profile update failed: {error}");
            }

            return ApiResponse<UserDto>.Ok(user.Adapt<UserDto>());
        }
    }
}
