using MediatR;
using RailwayTicketBooking.Domain.Entities;

namespace RailwayTicketBooking.Application.Stations.Queries
{
    public class GetAllStationsQuery : IRequest<List<Station>> { }
}