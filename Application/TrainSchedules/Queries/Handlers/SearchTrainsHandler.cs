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
                // 1️⃣ Load required collections once
                var trainsTask = _context.Trains.Find(_ => true).ToListAsync(cancellationToken);
                var routesTask = _context.Routes.Find(_ => true).ToListAsync(cancellationToken);
                var stationsTask = _context.Stations.Find(_ => true).ToListAsync(cancellationToken);

                // 2️⃣ Get all schedules valid for selected date
                var schedules = await _context.TrainSchedules
                    .Find(ts =>
                        ts.IsActive
                    )
                    .ToListAsync(cancellationToken);

                var trains = await trainsTask;
                var routes = await routesTask;
                var stations = await stationsTask;

                // 3️⃣ Resolve user-selected stations
                var fromStation = stations.FirstOrDefault(s => s.Id == request.FromStationId);
                var toStation = stations.FirstOrDefault(s => s.Id == request.ToStationId);
                if (fromStation == null || toStation == null)
                    return results;

                // Get the day of week for the travel date
                var travelDayOfWeek = request.TravelDate.DayOfWeek;

                foreach (var schedule in schedules)
                {
                    var train = trains.FirstOrDefault(t => t.Id == schedule.TrainId);
                    var route = routes.FirstOrDefault(r => r.Id == schedule.RouteId);

                    if (train == null || route == null)
                        continue;

                    // Check if train is active
                    if (!train.IsActive || !schedule.IsActive)
                        continue;

                    // Check if train runs on the selected travel date
                    // If RunsOn is empty or null, train runs daily
                    if (schedule.RunsOn != null && schedule.RunsOn.Count > 0)
                    {
                        if (!schedule.RunsOn.Contains(travelDayOfWeek))
                            continue;
                    }

                    var routeStations = route.Stations?.OrderBy(s => s.StationOrder).ToList();
                    if (routeStations == null || routeStations.Count == 0)
                        continue;

                    // 4️⃣ Check route contains FROM and TO
                    var fromRoute = routeStations.FirstOrDefault(s => s.StationId == request.FromStationId);
                    var toRoute = routeStations.FirstOrDefault(s => s.StationId == request.ToStationId);

                    if (fromRoute == null || toRoute == null)
                        continue;

                    // 5️⃣ Check correct travel direction using route station order
                    // (Schedule stations may be a subset, so we use route order for validation)
                    if (fromRoute.StationOrder >= toRoute.StationOrder)
                        continue;

                    // 6️⃣ Resolve time from schedule if available, otherwise from route timing
                    var scheduleStations = schedule.Stations?.OrderBy(s => s.StationOrder).ToList();
                    var fromSchedule = scheduleStations?.FirstOrDefault(s => s.StationId == request.FromStationId);
                    var toSchedule = scheduleStations?.FirstOrDefault(s => s.StationId == request.ToStationId);
                    
                    // Use schedule times if available, otherwise use route times
                    TimeSpan departureTimeSpan = fromSchedule?.DepartureTime ?? fromRoute.DepartureTime;
                    TimeSpan arrivalTimeSpan = toSchedule?.ArrivalTime ?? toRoute.ArrivalTime;

                    // Convert TimeSpan into DateTime combining travel date + time
                    DateTime departTime = request.TravelDate.Date.Add(departureTimeSpan);
                    DateTime arriveTime = request.TravelDate.Date.Add(arrivalTimeSpan);

                    // Handle day offsets for overnight trains
                    int fromDayOffset = fromSchedule?.DayOffset ?? fromRoute.DayOffset;
                    int toDayOffset = toSchedule?.DayOffset ?? toRoute.DayOffset;

                    // Adjust arrival time based on day offset
                    arriveTime = arriveTime.AddDays(toDayOffset - fromDayOffset);

                    // Sometimes arrival is next day (overnight trains) - check if arrival time is before departure
                    if (arriveTime < departTime)
                    {
                        arriveTime = arriveTime.AddDays(1);
                    }

                    var classesInfo = train.Classes?.Select(tc => new TrainClassInfo
                    {
                        ClassName = tc.ClassName,
                        Fare = tc.BasePrice,
                        AvailableSeats = tc.SeatCount,
                        Status = tc.SeatCount > 0 ? "Available" : "WL"
                    }).ToList() ?? new List<TrainClassInfo>();

                    // Filter class
                    if (request.Class != "All Classes")
                    {
                        classesInfo = classesInfo
                            .Where(c => c.ClassName.Contains(request.Class, StringComparison.OrdinalIgnoreCase))
                            .ToList();

                        if (!classesInfo.Any())
                            continue;
                    }

                    // Only available berth trains
                    if (request.TrainWithAvailableBerth && !classesInfo.Any(c => c.AvailableSeats > 0))
                        continue;

                    // 8️⃣ Build final result
                    results.Add(new TrainSearchResult
                    {
                        TrainId = train.Id,
                        ScheduleId = schedule.Id,
                        TrainNumber = train.TrainNumber,
                        TrainName = train.Name,
                        TrainType = train.TrainType,
                        FromStation = fromStation.StationName,
                        ToStation = toStation.StationName,
                        FromStationCode = fromStation.StationCode,
                        ToStationCode = toStation.StationCode,
                        DepartureTime = departTime,
                        ArrivalTime = arriveTime,
                        Duration = arriveTime - departTime,
                        Classes = classesInfo,
                        IsActive = train.IsActive && schedule.IsActive,
                        RunsOn = GetRunningDays(schedule)
                    });
                }
                results = results.OrderBy(r => r.DepartureTime).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SearchTrainsHandler: {ex}");
            }

            return results;
        }

        private List<string> GetRunningDays(TrainSchedule schedule)
        {
            if (schedule.RunsOn == null || schedule.RunsOn.Count == 0)
            {
                return new List<string> { "Daily" };
            }

            var dayAbbreviations = new Dictionary<DayOfWeek, string>
            {
                { DayOfWeek.Monday, "Mon" },
                { DayOfWeek.Tuesday, "Tue" },
                { DayOfWeek.Wednesday, "Wed" },
                { DayOfWeek.Thursday, "Thu" },
                { DayOfWeek.Friday, "Fri" },
                { DayOfWeek.Saturday, "Sat" },
                { DayOfWeek.Sunday, "Sun" }
            };

            return schedule.RunsOn
                .OrderBy(d => d)
                .Select(d => dayAbbreviations.ContainsKey(d) ? dayAbbreviations[d] : d.ToString())
                .ToList();
        }
    }
}
