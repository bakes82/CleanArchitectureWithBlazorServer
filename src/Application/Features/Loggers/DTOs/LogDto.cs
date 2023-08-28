using CleanArchitecture.Blazor.Domain.Entities.Logger;

namespace CleanArchitecture.Blazor.Application.Features.Loggers.DTOs;

public class LogDto
{
    [Description("Id")]
    public int Id { get; set; }

    [Description("Message")]
    public string? Message { get; set; }

    [Description("Message Template")]
    public string? MessageTemplate { get; set; }

    [Description("Level")]
    public string Level { get; set; } = default!;

    [Description("Timestamp")]
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

    [Description("Exception")]
    public string? Exception { get; set; }

    [Description("Username")]
    public string? UserName { get; set; }

    [Description("Client IP")]
    public string? ClientIp { get; set; }

    [Description("Client Agent")]
    public string? ClientAgent { get; set; }

    [Description("Properties")]
    public string? Properties { get; set; }

    [Description("Log Event")]
    public string? LogEvent { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Logger, LogDto>()
                .ReverseMap();
        }
    }
}