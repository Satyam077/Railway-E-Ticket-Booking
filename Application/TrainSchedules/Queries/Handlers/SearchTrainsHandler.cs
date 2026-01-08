using MediatR;
using MongoDB.Driver;
using RailwayTicketBooking.Domain.Entities;
using RailwayTicketBooking.Domain.Enums;
using RailwayTicketBooking.Infrastructure;

namespace RailwayTicketBooking.Application.TrainSchedules.Queries.Handlers
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
                // 1?? Load required collections once
                var trainsTask = _context.Trains.Find(_ => true).ToListAsync(cancellationToken);
                var routesTask = _context.Routes.Find(_ => true).ToListAsync(cancellationToken);
                var stationsTask = _context.Stations.Find(_ => true).ToListAsync(cancellationToken);

                // 2?? Get all schedules valid for selected date
                var schedules = await _context.TrainSchedules
                    .Find(ts =>
                        ts.IsActive
                    )
                    .ToListAsync(cancellationToken);

                var trains = await trainsTask;
                var routes = await routesTask;
                var stations = await stationsTask;

                // 3?? Resolve user-selected stations
                var fromStation = stations.FirstOrDefault(s => s.Id == request.FromStationId);
                var toStation = stations.FirstOrDefault(s => s.Id == request.ToStationId);
                if (fromStation == null || toStation == null)
                    return results;

                // Get the day of week for the travel date
                var travelDayOfWeek = request.TravelDate.DayOfWeek;

                // 4?? Load all bookings for the travel date once (for performance)
                var journeyDateStart = request.TravelDate.Date;
                var journeyDateEnd = journeyDateStart.AddDays(1).AddTicks(-1);
                
                var allBookingsForDate = await _context.Bookings
                    .Find(b => b.JourneyDate >= journeyDateStart &&
                               b.JourneyDate <= journeyDateEnd &&
                               b.Status == BookingStatus.Confirmed &&
                               b.IsActive)
                    .ToListAsync(cancellationToken);

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

                    // 4?? Check route contains FROM and TO
                    var fromRoute = routeStations.FirstOrDefault(s => s.StationId == request.FromStationId);
                    var toRoute = routeStations.FirstOrDefault(s => s.StationId == request.ToStationId);

                    if (fromRoute == null || toRoute == null)
                        continue;

                    // 5?? Check correct travel direction using route station order
                    // (Schedule stations may be a subset, so we use route order for validation)
                    if (fromRoute.StationOrder >= toRoute.StationOrder)
                        continue;

                    // 6?? Resolve time from schedule if available, otherwise from route timing
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

                    // Filter bookings for this specific train
                    var bookingsForThisTrain = allBookingsForDate
                        .Where(b => b.TrainId == train.Id)
                        .ToList();

                    var classesInfo = train.Classes?.Select(tc => 
                    {
                        // Map class name to SeatClass enum
                        var seatClass = MapClassNameToSeatClass(tc.ClassName);
                        
                        // Count booked seats for this class
                        var bookedSeatsCount = bookingsForThisTrain
                            .SelectMany(b => b.Seats ?? new List<BookedSeat>())
                            .Count(s => s.Class == seatClass);
                        
                        // Calculate available seats
                        var availableSeats = Math.Max(0, tc.SeatCount - bookedSeatsCount);
                        
                        // Determine status based on available seats
                        string status;
                        if (availableSeats > 0)
                            status = "Available";
                        else if (tc.SeatCount > 0)
                            status = "WL"; // Waiting List
                        else
                            status = "WL";
                        
                        return new TrainClassInfo
                        {
                            ClassName = tc.ClassName,
                            Fare = tc.BasePrice,
                            AvailableSeats = availableSeats,
                            Status = status
                        };
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

                    // 8?? Build final result
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

        /// <summary>
        /// Maps TrainClass.ClassName to SeatClass enum
        /// </summary>
        private SeatClass MapClassNameToSeatClass(string className)
        {
            if (string.IsNullOrEmpty(className))
                return SeatClass.SleeperClass;

            var classLower = className.ToLower().Trim();
            
            // Try exact enum match first
            if (Enum.TryParse<SeatClass>(className, true, out var parsedClass))
                return parsedClass;
            
            // Handle common class name formats
            if (classLower.Contains("sleeper") || classLower.Contains("sl"))
                return SeatClass.SleeperClass;
            if (classLower.Contains("ac 3") || classLower.Contains("3a") || classLower.Contains("3 tier") || 
                classLower.Contains("ac3") || classLower.Contains("ac iii"))
                return SeatClass.ACThreeTier;
            if (classLower.Contains("ac 2") || classLower.Contains("2a") || classLower.Contains("2 tier") ||
                classLower.Contains("ac2") || classLower.Contains("ac ii"))
                return SeatClass.ACTwoTier;
            if (classLower.Contains("ac 1") || classLower.Contains("1a") || classLower.Contains("first") ||
                classLower.Contains("ac1") || classLower.Contains("ac i") || classLower.Contains("ac first"))
                return SeatClass.ACFirstClass;
            if (classLower.Contains("chair") || classLower.Contains("cc"))
                return SeatClass.ACChairCar;
            if (classLower.Contains("second") || classLower.Contains("2s") ||
                classLower.Contains("second sitting"))
                return SeatClass.SecondSitting;
            if (classLower.Contains("executive") || classLower.Contains("ec"))
                return SeatClass.ExecutiveClass;

            // Default to Sleeper
            return SeatClass.SleeperClass;
        }
    }
}
