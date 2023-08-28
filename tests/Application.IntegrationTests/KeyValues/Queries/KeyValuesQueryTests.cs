using CleanArchitecture.Blazor.Application.Features.KeyValues.Queries.ByName;
using CleanArchitecture.Blazor.Domain.Enums;
using FluentAssertions;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.IntegrationTests.KeyValues.Queries;

using static Testing;

public class KeyValuesQueryTests : TestBase
{
    [Test]
    public void ShouldNotNullKeyValuesQueryByName()
    {
        KeyValuesQueryByName query  = new KeyValuesQueryByName(Picklist.Brand);
        var                  result = SendAsync(query);
        FluentActions.Invoking(() => SendAsync(query))
                     .Should()
                     .NotBeNull();
    }
}