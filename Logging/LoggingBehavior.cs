using MediatR;

namespace RailwayTicketBooking.Logging
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Handling {typeof(TRequest).Name}");
            var response = await next();
            Console.WriteLine($"Handled {typeof(TResponse).Name}");
            return response;
        }
    }
}
