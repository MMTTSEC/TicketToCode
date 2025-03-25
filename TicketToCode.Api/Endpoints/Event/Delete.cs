
namespace TicketToCode.Api.Endpoints;

public class Delete : IEndpoint
{
    //Mapping
    public static void MapEndpoint(IEndpointRouteBuilder app) => app
        .MapDelete("/events/delete/{Id}", Handle)
        .WithTags("Event EndPoints")
        .WithSummary("Delete event");

    // Request and Response types
    public record Request(int Id);
    public record Response(int Id);

    //Logic
    private static Response Handle([AsParameters] Request request, IDatabase db)
    {
        var ev = db.Events.Find(ev => ev.Id == request.Id);
        db.Events.Remove(ev);
        return new Response(ev.Id);
    }
}
