using System.Threading.Tasks;
using Application.Example.Features.Products.Queries.Export;
using CleanArchitecture.Blazor.Application.Common.Models;
using FluentAssertions;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.IntegrationTests.Products.Queries;

using static Testing;

internal class ExportProductsQueryTests : TestBase
{
    [Test]
    public async Task ShouldNotEmptyExportQuery()
    {
        ExportProductsQuery query  = new ExportProductsQuery { OrderBy = "Id", SortDirection = "Ascending" };
        Result<byte[]>      result = await SendAsync(query);
        result.Should()
              .NotBeNull();
    }

    [Test]
    public async Task ShouldNotEmptyExportQueryWithFilter()
    {
        ExportProductsQuery query  = new ExportProductsQuery { Keyword = "1", OrderBy = "Id", SortDirection = "Ascending" };
        Result<byte[]>      result = await SendAsync(query);
        result.Should()
              .NotBeNull();
    }
}