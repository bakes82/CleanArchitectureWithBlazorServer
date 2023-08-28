namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

public interface IResult
{
    string[] Errors { get; init; }

    bool Succeeded { get; init; }
}

public interface IResult<out T> : IResult
{
    T? Data { get; }
}