using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Example.Features.Products.DTOs;
using Application.Example.Features.Products.Queries.GetAll;
using CleanArchitecture.Blazor.Domain.Entities;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.IntegrationTests.Products.Queries;

using static Testing;

internal class GetAllProductsQueryTests : TestBase
{
    [SetUp]
    public async Task InitData()
    {
        await AddAsync(new Product { Name = "Test1" });
        await AddAsync(new Product { Name = "Test2" });
        await AddAsync(new Product { Name = "Test3" });
        await AddAsync(new Product { Name = "Test4" });
    }

    [Test]
    public async Task ShouldQueryAll()
    {
        GetAllProductsQuery     query  = new GetAllProductsQuery();
        IEnumerable<ProductDto> result = await SendAsync(query);
        Assert.AreEqual(4, result.Count());
    }

    [Test]
    public async Task ShouldQueryById()
    {
        GetAllProductsQuery     query  = new GetAllProductsQuery();
        IEnumerable<ProductDto> result = await SendAsync(query);
        int id = result.Last()
                       .Id;
        GetProductQuery getProductQuery = new GetProductQuery { Id = id };
        ProductDto      product         = await SendAsync(getProductQuery);
        Assert.AreEqual(id, product.Id);
    }
}