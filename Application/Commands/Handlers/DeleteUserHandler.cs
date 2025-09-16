using MediatR;
using MongoDB.Driver;
using Railway_Ticket_Booking.Infrastructure;

namespace Railway_Ticket_Booking.Application.Commands.Handlers
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly MongoDbContext _context;

        public DeleteUserHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            // Check if user has any active bookings before deletion
            var userBookings = await _context.Bookings
                .Find(b => b.UserId == request.Id)
                .CountDocumentsAsync(cancellationToken);

            if (userBookings > 0)
            {
                throw new InvalidOperationException("Cannot delete user with existing bookings. Please cancel all bookings first.");
            }

            var result = await _context.Users.DeleteOneAsync(
                u => u.Id == request.Id,
                cancellationToken: cancellationToken);

            return result.DeletedCount > 0;
        }
    }
}
