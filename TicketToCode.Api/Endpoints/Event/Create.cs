namespace TicketToCode.Api.Endpoints;
public class CreateEvent : IEndpoint
{
    // Mapping
    public static void MapEndpoint(IEndpointRouteBuilder app) => app
        .MapPost("/events/create", Handle)
        .WithSummary("Create event");

    // Request and Response types
    // Why do we need these? check this video if you are not sure
    // https://youtu.be/xtpPspNdX58?si=GJBUxMzeR2ZJ5Fg_
    public record Request(
        string Name,
        string Description,
        EventType Type,
        DateTime Start,
        DateTime End,
        int MaxAttendees,
        int Bookings
        );
    public record Response(int id);

    //Logic
    private static Ok<Response> Handle(Request request, IDatabase db)
    {
        // Todo, use a better constructor that enforces setting all necessary properties
        var ev = new Event()
        {
            Id = db.Events.Count + 1,
            Name = request.Name,
            Description = request.Description,
            Type = request.Type,
            StartTime = request.Start,
            EndTime = request.End,
            MaxAttendees = request.MaxAttendees,
            Bookings = request.Bookings
        };


        // Todo: does this set id on ev-object?
        db.Events.Add(ev); 

        return TypedResults.Ok(new Response(ev.Id));
    }
}

