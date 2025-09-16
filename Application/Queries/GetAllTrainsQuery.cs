using MediatR;
using Railway_Ticket_Booking.Domain.Entities;
namespace Railway_Ticket_Booking.Application.Queries
{
    public class GetAllTrainsQuery : IRequest<List<Train>> { }
}
