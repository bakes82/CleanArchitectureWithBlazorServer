using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Example.Features.Products.Commands.AddEdit;
using Application.Example.Features.Products.Commands.Delete;
using Application.Example.Features.Products.DTOs;
using Application.Example.Features.Products.Queries.GetAll;
using CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.IntegrationTests.Products.Commands;

using static Testing;

internal class DeleteProductCommandTests : TestBase
{
    [Test]
    public void ShouldRequireValidKeyValueId()
    {
        DeleteProductCommand command = new DeleteProductCommand(new[]
                                                                {
                                                                    99
                                                                });

        FluentActions.Invoking(() => SendAsync(command))
                     .Should()
                     .ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldDeleteOne()
    {
        AddEditProductCommand addCommand = new AddEditProductCommand
                                           {
                                               Name        = "Test",
                                               Brand       = "Brand",
                                               Price       = 100m,
                                               Unit        = "EA",
                                               Description = "Description"
                                           };
        Result<int> result = await SendAsync(addCommand);

        await SendAsync(new DeleteProductCommand(new[]
                                                 {
                                                     result.Data
                                                 }));

        Product item = await FindAsync<Product>(result.Data);

        item.Should()
            .BeNull();
    }

    [SetUp]
    public async Task InitData()
    {
        await AddAsync(new Product { Name = "Test1" });
        await AddAsync(new Product { Name = "Test2" });
        await AddAsync(new Product { Name = "Test3" });
        await AddAsync(new Product { Name = "Test4" });
    }

    [Test]
    public async Task ShouldDeleteAll()
    {
        GetAllProductsQuery     query  = new GetAllProductsQuery();
        IEnumerable<ProductDto> result = await SendAsync(query);
        result.Count()
              .Should()
              .Be(4);
        int[] id = result.Select(x => x.Id)
                         .ToArray();
        Result<int> deleted = await SendAsync(new DeleteProductCommand(id));
        deleted.Succeeded.Should()
               .BeTrue();

        IEnumerable<ProductDto> deleteResult = await SendAsync(query);
        deleteResult.Should()
                    .BeNullOrEmpty();
    }
}