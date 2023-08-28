using System.Threading.Tasks;
using Application.Example.Features.Products.DTOs;
using Application.Example.Features.Products.Queries.Pagination;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Entities;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.IntegrationTests.Products.Queries;

using static Testing;

internal class ProductsPaginationQueryTests : TestBase
{
    [SetUp]
    public async Task InitData()
    {
        await AddAsync(new Product
                       {
                           Name        = "Test1",
                           Price       = 19,
                           Brand       = "Test1",
                           Unit        = "EA",
                           Description = "Test1"
                       });
        await AddAsync(new Product
                       {
                           Name        = "Test2",
                           Price       = 19,
                           Brand       = "Test2",
                           Unit        = "EA",
                           Description = "Test1"
                       });
        await AddAsync(new Product
                       {
                           Name        = "Test3",
                           Price       = 19,
                           Brand       = "Test3",
                           Unit        = "EA",
                           Description = "Test1"
                       });
        await AddAsync(new Product
                       {
                           Name        = "Test4",
                           Price       = 19,
                           Brand       = "Test4",
                           Unit        = "EA",
                           Description = "Test1"
                       });
        await AddAsync(new Product
                       {
                           Name        = "Test5",
                           Price       = 19,
                           Brand       = "Test5",
                           Unit        = "EA",
                           Description = "Test1"
                       });
    }

    [Test]
    public async Task ShouldNotEmptyQuery()
    {
        ProductsWithPaginationQuery query  = new ProductsWithPaginationQuery();
        PaginatedData<ProductDto>   result = await SendAsync(query);
        Assert.AreEqual(5, result.TotalItems);
    }

    [Test]
    public async Task ShouldNotEmptyKeywordQuery()
    {
        ProductsWithPaginationQuery query  = new ProductsWithPaginationQuery { Keyword = "1" };
        PaginatedData<ProductDto>   result = await SendAsync(query);
        Assert.AreEqual(5, result.TotalItems);
    }

    [Test]
    public async Task ShouldNotEmptySpecificationQuery()
    {
        ProductsWithPaginationQuery query  = new ProductsWithPaginationQuery { Keyword = "1", Brand = "Test1", Unit = "EA", Name = "Test1" };
        PaginatedData<ProductDto>   result = await SendAsync(query);
        Assert.AreEqual(1, result.TotalItems);
    }
}