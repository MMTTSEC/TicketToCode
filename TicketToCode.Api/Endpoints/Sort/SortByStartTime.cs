using TicketToCode.Core.Interface;

namespace TicketToCode.Api.Endpoints.Sort
{
    public class SortByStartTime : IEndpoint
    {
        public static void MapEndpoint(IEndpointRouteBuilder app) => app
            .MapGet("/sort/starttime", Handle)
            .WithTags("Sort EndPoints")
            .WithSummary("Sort by Start Time");
        public record Response(
            int Id, 
            string Name, 
            string Description,
            EventType Type,
            DateTime StartTime,
            DateTime EndTime,
            int MaxAttendance
            );

        private static IResult Handle (ISort SortService)
        {
            var SortedByStartTime = SortService.SortByStartTime();

            var response = SortedByStartTime.Select(x => new Response(
                x.Id,
                x.Name,
                x.Description,
                x.Type,
                x.StartTime,
                x.EndTime,
                x.MaxAttendees

                ));
            return TypedResults.Ok( response );
        }
    }
}
