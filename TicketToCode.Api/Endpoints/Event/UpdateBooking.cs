namespace TicketToCode.Api.Endpoints;

public class UpdateBooking : IEndpoint
{
    // Mapping
    public static void MapEndpoint(IEndpointRouteBuilder app) => app
        .MapPut("/events/updatebooking/{Id}", Handle)
        .WithTags("Event EndPoints")
        .WithSummary("Update an existing events booking");

    // Request and Response types
    public record Request(
        int Bookings
    );

    public record Response(
        int Bookings
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
        if(request.Bookings < ev.MaxAttendees)
        {
            ev.Bookings = request.Bookings;
        } else
        {
            return TypedResults.NotFound(new { Message = "No available tickets" });
        }

        // Map the updated event to the response DTO
        var response = new Response(
            Bookings: ev.Bookings
        );

        return TypedResults.Ok(response);
    }
}
