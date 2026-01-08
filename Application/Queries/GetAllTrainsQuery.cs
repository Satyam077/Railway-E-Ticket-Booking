using MediatR;
using RailwayTicketBooking.Domain.Entities;
namespace RailwayTicketBooking.Application.Queries
{
    public class GetAllTrainsQuery : IRequest<List<Train>> { }
}
