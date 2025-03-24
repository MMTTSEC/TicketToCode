using TicketToCode.Api.Endpoints;
using TicketToCode.Core.Interface;

namespace TicketTCode.Api.Endpoints.Filter
{
    public class SearchBar : IEndpoint
    {
        public static void MapEndpoint(IEndpointRouteBuilder app) => app
            .MapGet("/filter", Handle)
            .WithTags("Filter EndPoints")
            .WithSummary("Search bar result");

        public record Request(string query);

        public record Response(
            int Id,
            string Name,
            string Description,
            EventType Type,
            DateTime StartTime,
            DateTime EndTime,
            int MaxAttendance
            );

        private static IResult Handle([AsParameters] Request request, IFilter FilterService)
        {
            var searchResult = FilterService.SearchBar(request.query);

            var response = searchResult.Select(x => new Response(
                x.Id,
                x.Name,
                x.Description,
                x.Type,
                x.StartTime,
                x.EndTime,
                x.MaxAttendees

                ));
            return TypedResults.Ok(response);
        }
    }
}
