using Blazor.Server.UI.Models.SideMenu;

namespace Blazor.Server.UI.Services.Navigation;

public interface IMenuService
{
    public List<MenuSectionModel> GetMenu();
}