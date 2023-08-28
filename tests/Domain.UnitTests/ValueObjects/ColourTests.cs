using CleanArchitecture.Blazor.Domain.Exceptions;
using CleanArchitecture.Blazor.Domain.ValueObjects;
using FluentAssertions;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Domain.UnitTests.ValueObjects;

public class ColourTests
{
    [Test]
    public void ShouldReturnCorrectColourCode()
    {
        const string code = "#FFFFFF";

        Colour colour = Colour.From(code);

        colour.Code.Should()
              .Be(code);
    }

    [Test]
    public void ToStringReturnsCode()
    {
        Colour colour = Colour.White;

        colour.ToString()
              .Should()
              .Be(colour.Code);
    }

    [Test]
    public void ShouldPerformImplicitConversionToColourCodeString()
    {
        string code = Colour.White;

        code.Should()
            .Be("#FFFFFF");
    }

    [Test]
    public void ShouldPerformExplicitConversionGivenSupportedColourCode()
    {
        Colour colour = (Colour)"#FFFFFF";

        colour.Should()
              .Be(Colour.White);
    }

    [Test]
    public void ShouldThrowUnsupportedColourExceptionGivenNotSupportedColourCode()
    {
        FluentActions.Invoking(() => Colour.From("##FF33CC"))
                     .Should()
                     .Throw<UnsupportedColourException>();
    }
}