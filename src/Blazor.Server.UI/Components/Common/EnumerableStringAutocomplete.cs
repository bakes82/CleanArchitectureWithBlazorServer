namespace Blazor.Server.UI.Components.Common;

public class EnumerableStringsAutocomplete : MudAutocomplete<string>
{
    [Parameter]
    public IEnumerable<string> ListOfValues { get; set; }

    public override Task SetParametersAsync(ParameterView parameters)
    {
        SearchFunc            = SearchKeyValues;
        Clearable             = true;
        Dense                 = true;
        return base.SetParametersAsync(parameters);
    }
    private Task<IEnumerable<string>> SearchKeyValues(string value)
    {
        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
        {
            return Task.FromResult(ListOfValues);
        }
        return Task.FromResult(ListOfValues.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase)));
    }
}