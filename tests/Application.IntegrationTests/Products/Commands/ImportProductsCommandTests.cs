using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Application.Example.Features.Products.Commands.Import;
using CleanArchitecture.Blazor.Application.Common.Models;
using FluentAssertions;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.IntegrationTests.Products.Commands;

using static Testing;

internal class ImportProductsCommandTests : TestBase
{
    [Test]
    public async Task DownloadTemplate()
    {
        CreateProductsTemplateCommand cmd    = new CreateProductsTemplateCommand();
        Result<byte[]>                result = await SendAsync(cmd);
        result.Succeeded.Should()
              .BeTrue();
    }

    [Test]
    public async Task ImportDataFromExcel()
    {
        string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly()
                                                   .Location);
        string                excelFile = Path.Combine(dir, "../../../", "Products", "ImportExcel", "Products.xlsx");
        byte[]                data      = File.ReadAllBytes(excelFile);
        ImportProductsCommand cmd       = new ImportProductsCommand("Products.xlsx", data);
        Result<int>           result    = await SendAsync(cmd);
        result.Succeeded.Should()
              .BeTrue();
    }
}