using MediatR;

namespace Railway_Ticket_Booking.Application.Routes.Queries
{
    public class GetAllRoutesQuery : IRequest<List<Railway_Ticket_Booking.Domain.Entities.Route>> { }
}
