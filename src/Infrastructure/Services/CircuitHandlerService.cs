using Microsoft.AspNetCore.Components.Server.Circuits;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class CircuitHandlerService : CircuitHandler
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    private readonly IUsersStateContainer _usersStateContainer;

    public CircuitHandlerService(IUsersStateContainer usersStateContainer, AuthenticationStateProvider authenticationStateProvider)
    {
        _usersStateContainer         = usersStateContainer;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public override async Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        AuthenticationState? state = await _authenticationStateProvider.GetAuthenticationStateAsync();
        if (state is not null && state.User.Identity is not null && state.User.Identity.IsAuthenticated)
        {
            _usersStateContainer.Update(circuit.Id, state.User.Identity.Name);
        }
    }

    public override Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        _usersStateContainer.Remove(circuit.Id);
        return Task.CompletedTask;
    }
}