using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Commands.AddEdit;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Commands.Delete;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Enums;
using FluentAssertions;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.IntegrationTests.KeyValues.Commands;

using static Testing;

public class DeleteKeyValueTests : TestBase
{
    [Test]
    public void ShouldRequireValidKeyValueId()
    {
        DeleteKeyValueCommand command = new DeleteKeyValueCommand(new[]
                                                                  {
                                                                      99
                                                                  });

        FluentActions.Invoking(() => SendAsync(command))
                     .Should()
                     .ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldDeleteKeyValue()
    {
        AddEditKeyValueCommand addCommand = new AddEditKeyValueCommand { Name = Picklist.Brand, Text = "Word", Value = "Word", Description = "For Test" };
        Result<int>            result     = await SendAsync(addCommand);

        await SendAsync(new DeleteKeyValueCommand(new[]
                                                  {
                                                      result.Data
                                                  }));

        Document item = await FindAsync<Document>(result.Data);

        item.Should()
            .BeNull();
    }
}