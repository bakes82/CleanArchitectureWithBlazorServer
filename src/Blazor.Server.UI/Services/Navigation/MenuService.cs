using Blazor.Server.UI.Models.SideMenu;
using CleanArchitecture.Blazor.Application.Common.Configurations;
using CleanArchitecture.Blazor.Application.Constants.Role;
using Microsoft.Extensions.Options;

namespace Blazor.Server.UI.Services.Navigation;

public class MenuService : IMenuService
{
    private static HangfireSettings? _hangfireSettings;
    
    public MenuService(IOptions<HangfireSettings> hangfireSettingsOptions)
    {
        _hangfireSettings = hangfireSettingsOptions.Value;
    }
    public List<MenuSectionModel> GetMenu()
    {
        return new List<MenuSectionModel>
                                                        {
                                                            new MenuSectionModel
                                                            {
                                                                Title = "Application",
                                                                SectionItems = new List<MenuSectionItemModel>
                                                                               {
                                                                                   new MenuSectionItemModel { Title = "Home", Icon = Icons.Material.Filled.Home, Href = "/" },
                                                                                   new MenuSectionItemModel
                                                                                   {
                                                                                       Title = "Analytics",
                                                                                       Roles = new[]
                                                                                               {
                                                                                                   RoleName.Admin,
                                                                                                   RoleName.Users
                                                                                               },
                                                                                       Icon       = Icons.Material.Filled.Analytics,
                                                                                       Href       = "/analytics",
                                                                                       PageStatus = PageStatus.ComingSoon
                                                                                   },
                                                                                   new MenuSectionItemModel
                                                                                   {
                                                                                       Title = "Banking",
                                                                                       Roles = new[]
                                                                                               {
                                                                                                   RoleName.Admin,
                                                                                                   RoleName.Users
                                                                                               },
                                                                                       Icon       = Icons.Material.Filled.Money,
                                                                                       Href       = "/banking",
                                                                                       PageStatus = PageStatus.ComingSoon
                                                                                   },
                                                                                   new MenuSectionItemModel
                                                                                   {
                                                                                       Title = "Booking",
                                                                                       Roles = new[]
                                                                                               {
                                                                                                   RoleName.Admin,
                                                                                                   RoleName.Users
                                                                                               },
                                                                                       Icon       = Icons.Material.Filled.CalendarToday,
                                                                                       Href       = "/booking",
                                                                                       PageStatus = PageStatus.ComingSoon
                                                                                   }
                                                                               }
                                                            },
                                                            new MenuSectionModel
                                                            {
                                                                Title = "MANAGEMENT",
                                                                Roles = new[]
                                                                        {
                                                                            RoleName.Admin
                                                                        },
                                                                SectionItems = new List<MenuSectionItemModel>
                                                                               {
                                                                                   new MenuSectionItemModel
                                                                                   {
                                                                                       IsParent = true,
                                                                                       Title    = "Authorization",
                                                                                       Icon     = Icons.Material.Filled.ManageAccounts,
                                                                                       MenuItems = new List<MenuSectionSubItemModel>
                                                                                                   {
                                                                                                       new MenuSectionSubItemModel { Title = "Multi-Tenant", Href = "/system/tenants", PageStatus = PageStatus.Completed },
                                                                                                       new MenuSectionSubItemModel { Title = "Users", Href        = "/identity/users", PageStatus = PageStatus.Completed },
                                                                                                       new MenuSectionSubItemModel { Title = "Roles", Href        = "/identity/roles", PageStatus = PageStatus.Completed },
                                                                                                       new MenuSectionSubItemModel { Title = "Profile", Href      = "/user/profile", PageStatus   = PageStatus.Completed }
                                                                                                   }
                                                                                   },
                                                                                   new MenuSectionItemModel
                                                                                   {
                                                                                       IsParent = true,
                                                                                       Title    = "System",
                                                                                       Icon     = Icons.Material.Filled.Devices,
                                                                                       MenuItems = new List<MenuSectionSubItemModel>
                                                                                                   {
                                                                                                       new MenuSectionSubItemModel { Title = "Audit Trails", Href = "/system/audittrails", PageStatus = PageStatus.Completed },
                                                                                                       new MenuSectionSubItemModel { Title = "Log", Href          = "/system/logs", PageStatus = PageStatus.Completed },
                                                                                                       new MenuSectionSubItemModel { Title = "Jobs", Href         = "/jobs", PageStatus = _hangfireSettings.Enabled ? PageStatus.Completed : PageStatus.Disabled, Target = "_blank" }
                                                                                                   }
                                                                                   }
                                                                               }
                                                            }
                                                        };
    }
    
}