namespace TicketToCode.Api.Endpoints;
public class CreateEvent : IEndpoint
{
    // Mapping
    public static void MapEndpoint(IEndpointRouteBuilder app) => app
        .MapPost("/events/create", Handle)
        .WithTags("Event EndPoints")
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
    public record Response(int Id);

    //Logic
    private static Ok<Response> Handle(Request request, IDatabase db)
    {
        //makeing sure there are no duplicite id's.
        var possilbeId = db.Events.Count + 1;
        while (db.Events.Any(e => e.Id == possilbeId)) 
        {
            possilbeId++;
        }

        // Todo, use a better constructor that enforces setting all necessary properties
        var ev = new Event()
        {
            
            Id = possilbeId,
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

