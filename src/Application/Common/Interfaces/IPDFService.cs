namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

public interface IPdfService
{
    Task<byte[]> ExportAsync<TData>(IEnumerable<TData> data, Dictionary<string, Func<TData, object?>> mappers, string title, bool landscape);
}