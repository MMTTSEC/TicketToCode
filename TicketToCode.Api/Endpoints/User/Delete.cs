﻿namespace TicketToCode.Api.Endpoints.User
{
    public class Delete : IEndpoint
    {
        public static void MapEndpoint(IEndpointRouteBuilder app) => app
            .MapDelete("/users/{id}", Handle)
            .WithSummary("Delete User");

        public record Request(int Id);
        public record Response(int Id);

        private static Response Handle([AsParameters] Request request, IDatabase db)
        {
            var u = db.Users.Find(u => u.Id == request.Id);
            db.Users.Remove(u);
            return new Response(u.Id);
        }



    }
}
