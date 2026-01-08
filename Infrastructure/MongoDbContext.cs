using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using RailwayTicketBooking.Domain.Entities;

namespace RailwayTicketBooking.Infrastructure
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration config)
        {
            try
            {
                var connectionString = config.GetConnectionString("MongoDb") ?? config["MongoDb:ConnectionString"] ?? "mongodb+srv://c59933290:SCAOuac9FtJyapjA@demo-drive-nexus.ekntl.mongodb.net/";
                var databaseName = config["MongoDb:Database"] ?? "RailwayTicket";
                
                var client = new MongoClient(connectionString);
                _database = client.GetDatabase(databaseName);
            }
            catch (Exception ex)
            {
                // Log the error but don't crash the application
                Console.WriteLine($"MongoDB connection failed: {ex.Message}");
                // You might want to use a fallback or in-memory database here
            }
        }


        // Core entities
        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<Train> Trains => _database.GetCollection<Train>("Trains");
        public IMongoCollection<Station> Stations => _database.GetCollection<Station>("Stations");
        public IMongoCollection<RailwayTicketBooking.Domain.Entities.Route> Routes => _database.GetCollection<RailwayTicketBooking.Domain.Entities.Route>("Routes");
        public IMongoCollection<TrainSchedule> TrainSchedules => _database.GetCollection<TrainSchedule>("TrainSchedules");
        public IMongoCollection<Seat> Seats => _database.GetCollection<Seat>("Seats");
        public IMongoCollection<Booking> Bookings => _database.GetCollection<Booking>("Bookings");
        public IMongoCollection<Payment> Payments => _database.GetCollection<Payment>("Payments");
        public IMongoCollection<Notification> Notifications => _database.GetCollection<Notification>("Notifications");
    }
}
