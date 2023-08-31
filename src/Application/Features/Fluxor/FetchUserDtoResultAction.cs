using CleanArchitecture.Blazor.Application.Features.Identity.Dto;

namespace CleanArchitecture.Blazor.Application.Features.Fluxor;

public class FetchUserDtoResultAction
{
    public FetchUserDtoResultAction(ApplicationUserDto dto)
    {
        UserProfile = new UserProfile
                      {
                          UserId                = dto.Id,
                          ProfilePictureDataUrl = dto.ProfilePictureDataUrl,
                          Email                 = dto.Email,
                          PhoneNumber           = dto.PhoneNumber,
                          DisplayName           = dto.DisplayName,
                          Provider              = dto.Provider,
                          UserName              = dto.UserName,
                          SuperiorId            = dto.SuperiorId,
                          SuperiorName          = dto.SuperiorName,
                          AssignedRoles         = dto.AssignedRoles
                      };
    }

    public UserProfile UserProfile { get; }
}