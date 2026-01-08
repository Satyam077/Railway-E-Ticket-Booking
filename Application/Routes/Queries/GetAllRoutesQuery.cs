using MediatR;

namespace RailwayTicketBooking.Application.Routes.Queries
{
    public class GetAllRoutesQuery : IRequest<List<RailwayTicketBooking.Domain.Entities.Route>> { }
}
