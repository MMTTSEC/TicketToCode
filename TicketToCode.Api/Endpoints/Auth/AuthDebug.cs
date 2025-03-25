using System.Reflection.Metadata;
using System.Linq;
using System.Collections.Generic;
using TicketToCode.Api.Services;

namespace TicketToCode.Api.Endpoints.Auth
{
    public class AuthDebug : IEndpoint //can be deleted
    {
        //Mapping
        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app
            .MapGet("/auth/Debug", Handle)
            .WithTags("Auth EndPoints")
            .WithSummary("View user Database");
        }

    public record request();
    public record Results(
        int Id,
        string Username, 
        string PasswordHash,
        string Role
        );

    private static List<Results> Handle(IDatabase db)
    {
        return db.Users
            .Select(u => new Results(
                u.Id,
                u.Username,
                u.PasswordHash,
                u.Role
                )).ToList();
    }
    }
}
