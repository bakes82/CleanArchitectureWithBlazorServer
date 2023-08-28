using Microsoft.JSInterop;

namespace Blazor.Server.UI.Services.JsInterop;

public class InputClear
{
    private readonly IJSRuntime _jsRuntime;

    public InputClear(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public ValueTask Clear(string targetId)
    {
        return _jsRuntime.InvokeVoidAsync(JsInteropConstants.ClearInput, targetId);
    }
}