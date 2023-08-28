namespace CleanArchitecture.Blazor.Application.Constants.Permission;

public static class PermissionModules
{
    public const string Users     = "Users";
    public const string Roles     = "Roles";
    public const string Products  = "Products";
    public const string Brands    = "Brands";
    public const string Companies = "Companies";

    public static List<string> GeneratePermissionsForModule(string module)
    {
        return new List<string> { $"Permissions.{module}.Create", $"Permissions.{module}.View", $"Permissions.{module}.Edit", $"Permissions.{module}.Delete" };
    }

    public static List<string> GetAllPermissionsModules()
    {
        return new List<string>
               {
                   Users,
                   Roles,
                   Products,
                   Brands,
                   Companies
               };
    }
}