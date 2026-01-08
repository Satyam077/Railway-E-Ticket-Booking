using MediatR;
using Microsoft.AspNetCore.Components.Authorization;
using RailwayTicketBooking.EmailServices;
using RailwayTicketBooking.Infrastructure;
using RailwayTicketBooking.Infrastructure.Services;
using RailwayTicketBooking.Logging;
using RailwayTicketBooking.WebSettings;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();

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

builder.Services.AddScoped<SeatAllocationService>();
builder.Services.AddScoped<CancellationService>();
builder.Services.AddScoped<BookingEmailService>();

// Register JWT Service
builder.Services.AddScoped<IJwtService, JwtService>();

// Configure Authentication and Authorization
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("SuperAdmin"));
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin", "SuperAdmin"));
    options.AddPolicy("ZonalManagerOnly", policy => policy.RequireRole("ZonalManager", "Admin", "SuperAdmin"));
    options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
    options.AddPolicy("AuthenticatedUsers", policy => policy.RequireAuthenticatedUser());
});

// Register Custom Authentication State Provider
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
