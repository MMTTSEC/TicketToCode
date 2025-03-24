namespace TicketToCode.Api.Endpoints;
public class GetAllEvents : IEndpoint
{
    // Mapping
    public static void MapEndpoint(IEndpointRouteBuilder app) => app
        .MapGet("/events", Handle)
        .WithTags("Event EndPoints")
        .WithSummary("Get all events");

    // Request and Response types
    public record Response(
        int Id,
        string Name,
        string Description,
        EventType Type,
        DateTime Start,
        DateTime End,
        int MaxAttendees
    );

    //Logic
    private static List<Response> Handle(IDatabase db)
    {
        return db.Events
            .Select(item => new Response(
                Id: item.Id,
                Name: item.Name,
                Description: item.Description,
                Type: item.Type,
                Start: item.StartTime,
                End: item.EndTime,
                MaxAttendees: item.MaxAttendees
            )).ToList();
    }
}