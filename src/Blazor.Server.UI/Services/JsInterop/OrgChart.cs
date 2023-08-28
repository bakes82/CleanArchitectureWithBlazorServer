using Blazor.Server.UI.Pages.Identity.Users;
using Microsoft.JSInterop;

namespace Blazor.Server.UI.Services.JsInterop;

public class OrgChart
{
    private readonly IJSRuntime _jsRuntime;

    public OrgChart(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public ValueTask Create(List<OrgItem> data)
    {
        return _jsRuntime.InvokeVoidAsync(JsInteropConstants.CreateOrgChart, data);
    }
}