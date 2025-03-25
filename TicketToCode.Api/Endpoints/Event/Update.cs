namespace TicketToCode.Api.Endpoints;
public class Update : IEndpoint
{
    // Mapping
    public static void MapEndpoint(IEndpointRouteBuilder app) => app
        .MapPut("/events/update/{Id}", Handle)
        .WithTags("Event EndPoints")
        .WithSummary("Update an existing event");

    // Request and Response types
    public record Request(
        int Id,
        string Name,
        string Description,
        EventType Type,
        DateTime Start,
        DateTime End,
        int MaxAttendees
    );

    public record Response(
        int Id,
        string Name,
        string Description,
        EventType Type,
        DateTime Start,
        DateTime End,
        int MaxAttendees,
        int Bookings,
        int RemainingSeats
    );


    //Logic
    private static IResult Handle(int Id, Request request, IDatabase db)
    {
        // Find the event by Id
        var ev = db.Events.FirstOrDefault(ev => ev.Id == Id);

        if (ev == null)
        {
            return TypedResults.NotFound(new { Message = "Event not found" });
        }

        // Update the properties of the event
        ev.Name = request.Name;
        ev.Description = request.Description;
        ev.Type = request.Type;
        ev.StartTime = request.Start;
        ev.EndTime = request.End;
        ev.MaxAttendees = request.MaxAttendees;

        // Map the updated event to the response DTO
        var response = new Response(
            Id: ev.Id,
            Name: ev.Name,
            Description: ev.Description,
            Type: ev.Type,
            Start: ev.StartTime,
            End: ev.EndTime,
            MaxAttendees: ev.MaxAttendees,
            Bookings: ev.Bookings,
            RemainingSeats: ev.MaxAttendees - ev.Bookings
        );

        return TypedResults.Ok(response);
    }
}
