using MediatR;
using MongoDB.Driver;
using Railway_Ticket_Booking.Domain.Entities;
using Railway_Ticket_Booking.Infrastructure;

namespace Railway_Ticket_Booking.Application.TrainSchedules.Queries.Handlers
{
    public class SearchTrainsHandler : IRequestHandler<SearchTrainsQuery, List<TrainSearchResult>>
    {
        private readonly MongoDbContext _context;

        public SearchTrainsHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<List<TrainSearchResult>> Handle(SearchTrainsQuery request, CancellationToken cancellationToken)
        {
            var results = new List<TrainSearchResult>();

            try
            {
                // Get all active train schedules for the requested date
                var schedules = await _context.TrainSchedules
                    .Find(ts => ts.IsActive && 
                               ts.DepartureDate.Date <= request.TravelDate.Date &&
                               ts.ArrivalDate.Date >= request.TravelDate.Date)
                    .ToListAsync(cancellationToken);

                // Get all trains, routes, and stations for reference
                var trains = await _context.Trains.Find(_ => true).ToListAsync(cancellationToken);
                var routes = await _context.Routes.Find(_ => true).ToListAsync(cancellationToken);
                var stations = await _context.Stations.Find(_ => true).ToListAsync(cancellationToken);

                foreach (var schedule in schedules)
                {
                    var train = trains.FirstOrDefault(t => t.Id == schedule.TrainId);
                    var route = routes.FirstOrDefault(r => r.Id == schedule.RouteId);

                    var routeStations = route.Stations?.ToList();
                    var routeStation = route.Stations?.FirstOrDefault(_ => _.StationId == schedule.Id);


                    if (train == null || route == null) continue;

                    // Check if route contains both from and to stations
                    var fromStationInRoute = schedule.Stations?.FirstOrDefault(s => s.StationId == request.FromStationId);
                    var toStationInRoute = schedule.Stations?.FirstOrDefault(s => s.StationId == request.ToStationId);

                    // Check if route contains both from and to stations
                    var fromStationRoute = routeStations?.FirstOrDefault(s => s.StationId == request.FromStationId);
                    var toStationRoute = routeStations?.FirstOrDefault(s => s.StationId == request.ToStationId);


                    // If specific stations not found in schedule, check route stations
                    if (fromStationInRoute == null || toStationInRoute == null)
                    {
                        fromStationInRoute = route.Stations?.FirstOrDefault(s => s.StationId == request.FromStationId) != null ? 
                            new ScheduleStation { StationId = request.FromStationId } : null;
                        toStationInRoute = route.Stations?.FirstOrDefault(s => s.StationId == request.ToStationId) != null ? 
                            new ScheduleStation { StationId = request.ToStationId } : null;
                    }

                    if (fromStationInRoute == null || toStationInRoute == null) continue;

                    // Ensure from station comes before to station in the route
                    var fromOrder = schedule.Stations?.FirstOrDefault(s => s.StationId == request.FromStationId)?.StationOrder ??
                                   route.Stations?.FirstOrDefault(s => s.StationId == request.FromStationId)?.StationOrder ?? 0;
                    var toOrder = schedule.Stations?.FirstOrDefault(s => s.StationId == request.ToStationId)?.StationOrder ??
                                 route.Stations?.FirstOrDefault(s => s.StationId == request.ToStationId)?.StationOrder ?? 0;

                    if (fromOrder >= toOrder) continue;

                    var fromStation = stations.FirstOrDefault(s => s.Id == request.FromStationId);
                    var toStation = stations.FirstOrDefault(s => s.Id == request.ToStationId);

                    if (fromStation == null || toStation == null) continue;

                    //// Calculate departure and arrival times
                    var departureTime = fromStationInRoute.ScheduledDeparture != DateTime.MinValue ?
                        fromStationInRoute.ScheduledDeparture : schedule.DepartureDate;
                    var arrivalTime = toStationInRoute.ScheduledArrival != DateTime.MinValue ?
                        toStationInRoute.ScheduledArrival : schedule.ArrivalDate;

                    // Calculate departure and arrival times in Routes
                    //var departureTime = fromStationRoute.DepartureTime;
                    //var arrivalTime = toStationRoute.ArrivalTime;

                    // Create train classes info
                    var classesInfo = train.Classes?.Select(tc => new TrainClassInfo
                    {
                        ClassName = tc.ClassName,
                        Fare = tc.BasePrice,
                        AvailableSeats = tc.SeatCount,
                        Status = tc.SeatCount > 0 ? "Available" : "Waiting List"
                    }).ToList() ?? new List<TrainClassInfo>();

                    // Filter by class if specified
                    if (request.Class != "All Classes")
                    {
                        classesInfo = classesInfo.Where(c => c.ClassName.Contains(request.Class, StringComparison.OrdinalIgnoreCase)).ToList();
                        if (!classesInfo.Any()) continue;
                    }

                    // Filter by available berth if requested
                    if (request.TrainWithAvailableBerth)
                    {
                        if (!classesInfo.Any(c => c.AvailableSeats > 0)) continue;
                    }

                    var searchResult = new TrainSearchResult
                    {
                        TrainId = train.Id,
                        ScheduleId = schedule.Id,
                        TrainNumber = train.TrainNumber,
                        TrainName = train.Name,
                        TrainType = train.TrainType,
                        DepartureTime = departureTime,
                        ArrivalTime = arrivalTime,
                        Duration = arrivalTime - departureTime,
                        FromStation = fromStation.StationName,
                        ToStation = toStation.StationName,
                        FromStationCode = fromStation.StationCode,
                        ToStationCode = toStation.StationCode,
                        Classes = classesInfo,
                        IsActive = train.IsActive && schedule.IsActive,
                        RunsOn = GetRunningDays(schedule) // You can implement this based on your requirements
                    };

                    results.Add(searchResult);
                }

                // Sort by departure time
                results = results.OrderBy(r => r.DepartureTime).ToList();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in SearchTrainsHandler: {ex.Message}");
            }

            return results;
        }

        private List<string> GetRunningDays(TrainSchedule schedule)
        {
            // This is a simplified implementation
            // You can enhance this based on your business logic
            var days = new List<string>();
            
            // For now, assume all trains run daily
            days.AddRange(new[] { "M", "T", "W", "T", "F", "S", "S" });
            
            return days;
        }
    }
}
