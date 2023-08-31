namespace CleanArchitecture.Blazor.Application.Common.Models;

public class UploadRequest
{
    public UploadRequest(string fileName, string? uploadType, byte[] data)
    {
        FileName   = fileName;
        UploadType = uploadType;
        Data       = data;
    }

    public string  FileName  { get; set; }
    public string? Extension { get; set; }
    public string? UploadType { get; set; }
    public byte[]     Data       { get; set; }
}