using MediatR;
using Microsoft.AspNetCore.Components.Authorization;
using Railway_Ticket_Booking.EmailServices;
using Railway_Ticket_Booking.Infrastructure;
using Railway_Ticket_Booking.Infrastructure.Services;
using Railway_Ticket_Booking.Logging;
using Railway_Ticket_Booking.WebSettings;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient(); // Add HttpClient for API calls

// Register MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddSingleton<MongoDbContext>();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<PayuService>();
builder.Services.Configure<PayuOptions>(
    builder.Configuration.GetSection("PayU")
);

// Register Seat Allocation Service
builder.Services.AddScoped<SeatAllocationService>();

// Register Cancellation Service
builder.Services.AddScoped<CancellationService>();

// Register Booking Email Service
builder.Services.AddScoped<BookingEmailService>();



// Register JWT Service
builder.Services.AddScoped<IJwtService, JwtService>();

// Configure Authentication and Authorization
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore(options =>
{
    // Define role-based policies
    options.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("SuperAdmin"));
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin", "SuperAdmin"));
    options.AddPolicy("ZonalManagerOnly", policy => policy.RequireRole("ZonalManager", "Admin", "SuperAdmin"));
    options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
    options.AddPolicy("AuthenticatedUsers", policy => policy.RequireAuthenticatedUser());
});

// Register Custom Authentication State Provider
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllers(); // Map API controllers
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
