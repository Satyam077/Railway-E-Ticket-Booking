using MediatR;
using Railway_Ticket_Booking.Domain.Entities;

namespace Railway_Ticket_Booking.Application.Routes.Queries
{
    public class GetAllStationsQuery : IRequest<List<Station>> { }
}
