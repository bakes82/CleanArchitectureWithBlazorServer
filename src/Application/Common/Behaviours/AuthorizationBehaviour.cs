using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Application.Common.Security;

namespace CleanArchitecture.Blazor.Application.Common.Behaviours;

public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService    _identityService;

    public AuthorizationBehaviour(ICurrentUserService currentUserService, IIdentityService identityService)
    {
        _currentUserService = currentUserService;
        _identityService    = identityService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        IEnumerable<RequestAuthorizeAttribute> authorizeAttributes = request.GetType()
                                                                            .GetCustomAttributes<RequestAuthorizeAttribute>();
        IEnumerable<RequestAuthorizeAttribute> requestAuthorizeAttributes = authorizeAttributes.ToList();
        if (requestAuthorizeAttributes.Any())
        {
            // Must be authenticated user
            string? userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException();
            }

            // DefaultRole-based authorization
            IEnumerable<RequestAuthorizeAttribute> authorizeAttributesWithRoles = requestAuthorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));

            IEnumerable<RequestAuthorizeAttribute> attributesWithRoles = authorizeAttributesWithRoles.ToList();
            if (attributesWithRoles.Any())
            {
                bool authorized = false;

                foreach (string[] roles in attributesWithRoles.Select(a => a.Roles.Split(',')))
                {
                    foreach (string role in roles)
                    {
                        bool isInRole = await _identityService.IsInRoleAsync(userId, role.Trim());
                        if (isInRole)
                        {
                            authorized = true;
                            break;
                        }
                    }
                }

                // Must be a member of at least one role in roles
                if (!authorized)
                {
                    throw new ForbiddenException("You are not authorized to access this resource.");
                }
            }

            // Policy-based authorization
            IEnumerable<RequestAuthorizeAttribute> authorizeAttributesWithPolicies = requestAuthorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy));
            IEnumerable<RequestAuthorizeAttribute> attributesWithPolicies          = authorizeAttributesWithPolicies.ToList();
            if (attributesWithPolicies.Any())
            {
                foreach (string? policy in attributesWithPolicies.Select(a => a.Policy))
                {
                    bool authorized = await _identityService.AuthorizeAsync(userId, policy);

                    if (!authorized)
                    {
                        throw new ForbiddenException("You are not authorized to access this resource.");
                    }
                }
            }
        }

        // User is authorized / authorization not required
        return await next()
            .ConfigureAwait(false);
    }
}