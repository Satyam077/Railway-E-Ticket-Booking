using MediatR;
using RailwayTicketBooking.Domain.Entities;

namespace RailwayTicketBooking.Application.Routes.Queries
{
    public class GetAllStationsQuery : IRequest<List<Station>> { }
}
