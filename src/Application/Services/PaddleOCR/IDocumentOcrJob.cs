namespace CleanArchitecture.Blazor.Application.Services.PaddleOCR;

public interface IDocumentOcrJob
{
    void Do(int id);
    Task Recognition(int id, CancellationToken cancellationToken);
}