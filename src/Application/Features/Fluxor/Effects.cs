using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Application.Features.Identity.Dto;

namespace CleanArchitecture.Blazor.Application.Features.Fluxor;

public class Effects
{
    private readonly IIdentityService _identityService;

    public Effects(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [EffectMethod]
    public async Task HandleFetchDataAction(FetchUserDtoAction action, IDispatcher dispatcher)
    {
        ApplicationUserDto? result = await _identityService.GetApplicationUserDto(action.UserId);
        if (result is not null)
        {
            dispatcher.Dispatch(new FetchUserDtoResultAction(result));
        }
    }
}