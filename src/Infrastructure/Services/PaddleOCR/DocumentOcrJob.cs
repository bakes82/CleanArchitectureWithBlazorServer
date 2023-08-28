using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Runtime.Versioning;
using System.Text.Json;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Serialization;
/*using CleanArchitecture.Blazor.Application.Features.Documents.Caching;*/
using CleanArchitecture.Blazor.Application.Services.PaddleOCR;
using CleanArchitecture.Blazor.Domain.Enums;
using CleanArchitecture.Blazor.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CleanArchitecture.Blazor.Infrastructure.Services.PaddleOCR;

[SupportedOSPlatform("windows")]
public class DocumentOcrJob : IDocumentOcrJob
{
    private readonly IApplicationDbContext                _context;
    private readonly IHttpClientFactory                   _httpClientFactory;
    private readonly IHubContext<SignalRHub, ISignalRHub> _hubContext;
    private readonly ILogger<DocumentOcrJob>              _logger;
    private readonly ISerializer                          _serializer;
    private readonly Stopwatch                            _timer;

    public DocumentOcrJob(IHubContext<SignalRHub, ISignalRHub> hubContext, IApplicationDbContext context, IHttpClientFactory httpClientFactory, ISerializer serializer, ILogger<DocumentOcrJob> logger)
    {
        _hubContext        = hubContext;
        _context           = context;
        _httpClientFactory = httpClientFactory;
        _serializer        = serializer;
        _logger            = logger;
        _timer             = new Stopwatch();
    }

    public void Do(int id)
    {
        Recognition(id, CancellationToken.None)
            .Wait();
    }

    public async Task Recognition(int id, CancellationToken cancellationToken)
    {
        try
        {
            using (HttpClient client = _httpClientFactory.CreateClient("ocr"))
            {
                _timer.Start();
                Document? doc = await _context.Documents.FindAsync(id);
                if (doc == null)
                {
                    return;
                }

                await _hubContext.Clients.All.Start(doc.Title!);
                /*DocumentCacheKey.SharedExpiryTokenSource()
                                .Cancel();*/
                if (string.IsNullOrEmpty(doc.Url))
                {
                    return;
                }

                string imgFile = Path.Combine(Directory.GetCurrentDirectory(), doc.Url);
                if (!File.Exists(imgFile))
                {
                    return;
                }

                // Create multipart/form-data content
                using MultipartFormDataContent form        = new MultipartFormDataContent();
                using FileStream               fileStream  = new FileStream(imgFile, FileMode.Open);
                using StreamContent            fileContent = new StreamContent(fileStream);

                form.Add(fileContent, "file", Path.GetFileName(imgFile)); // "image" is the form parameter name for the file

                HttpResponseMessage response = await client.PostAsync("", form);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string     result    = await response.Content.ReadAsStringAsync();
                    OcrResult? ocrResult = JsonSerializer.Deserialize<OcrResult>(result);
                    doc.Status = JobStatus.Done;

                    if (ocrResult is not null)
                    {
                        string content = string.Join(',', ocrResult.Data);
                        doc.Description = "recognize the result: success";
                        doc.Content     = content;
                    }

                    await _context.SaveChangesAsync(cancellationToken);
                    await _hubContext.Clients.All.Completed(doc.Title!);
                    /*DocumentCacheKey.SharedExpiryTokenSource()
                                    .Cancel()*/;
                    _timer.Stop();
                    long elapsedMilliseconds = _timer.ElapsedMilliseconds;
                    _logger.LogInformation("Id: {Id}, elapsed: {ElapsedMilliseconds}, recognize the result: {@Result},{@Content}", id, elapsedMilliseconds, ocrResult, doc.Content);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Id}: recognize error {ExMessage}", id, ex.Message);
        }
    }

    private string ReadBase64String(string path)
    {
        using (Image image = Image.FromFile(path))
        {
            using (MemoryStream m = new MemoryStream())
            {
                image.Save(m, image.RawFormat);
                byte[] imageBytes = m.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }
    }
}
#pragma warning disable CS8981
internal class OcrResult
{
    public string[] Data { get; set; } = Array.Empty<string>();
}
#pragma warning restore CS8981