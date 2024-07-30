using Microsoft.EntityFrameworkCore;
using EventHandler.Api.Data;
using EventHandler.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var userConnString = builder.Configuration.GetConnectionString("User");
var eventConnString = builder.Configuration.GetConnectionString("Event");
var bookingConnString = builder.Configuration.GetConnectionString("Booking");

// Register DbContexts with their respective connection strings
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlite(userConnString));

builder.Services.AddDbContext<EventDbContext>(options =>
    options.UseSqlite(eventConnString));

builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseSqlite(bookingConnString));

// build app 
var app = builder.Build();
// Map endpoints
app.MapUserEndpoints();
app.MapEventEndpoints();
app.MapBookingEndpoints();
await app.MigrateDbAsync();
// run app
app.Run();
