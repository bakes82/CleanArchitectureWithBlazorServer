using System.ComponentModel;

namespace CleanArchitecture.Blazor.Domain.Enums;

public enum UploadType : byte
{
    [Description(@"Products")]        Product,
    [Description(@"ProfilePictures")] ProfilePicture,
    [Description(@"Documents")]       Document
}