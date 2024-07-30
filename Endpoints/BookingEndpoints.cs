using EventHandler.Api.Data;
using EventHandler.Api.Entities;
using EventHandler.Api.Mapping;
using EventHandler.Api.Dtos.Booking;
using Microsoft.EntityFrameworkCore;
namespace EventHandler.Api.Endpoints;

public static class BookingEndpoints
{
    public static RouteGroupBuilder MapBookingEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("bookings").WithParameterValidation();
        const string getBookingEndpoint = "GetBookingById"; 
        // Get Bookings
        group.MapGet("/", async (BookingDbContext dbContext) =>
            await dbContext.Bookings
                // Transforms bookings to summarized dto versions
                .Select(booking => booking.ToBookingSummaryDto())
                // Removes tracking for better performance (optional)
                .AsNoTracking()
                // Makes list out of the query 
                .ToListAsync());

        // Get bookings by id
        group.MapGet("/{id}", async (int id, BookingDbContext dbContext) =>
        {
            // Search for bookings by id. result may be null 
            Booking? booking = await dbContext.Bookings.FindAsync(id);
            // Return either null or a detailed version of the booking
            return booking is null ? Results.NotFound() : Results.Ok(booking.ToBookingDetailsDto());
        }).WithName(getBookingEndpoint);

        // Get bookings by username 
        group.MapGet("/user_name/{username}", async (string username, BookingDbContext dbContext) =>
        {
            // Search for user by username. result may be null
            Booking? booking = await dbContext.Bookings
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Username == username);
            // Return either null or a detailed version of the booking
            return booking is null ? Results.NotFound() : Results.Ok(booking.ToBookingDetailsDto());
        }).WithName("GetBookingByUsername");

        // Create Bookings
        group.MapPost("/", async (CreateBookingDto newBooking, BookingDbContext dbContext, UserDbContext userDbContext,
        EventDbContext eventDbContext) => 
        { 
            // Search for user by username. result may be null
            User? user = await userDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == newBooking.Username);     
            if (user is null){
                return Results.NotFound();
            };
            // Search for event by id, result may be null 
            Event? evt = await eventDbContext.Events.FindAsync(newBooking.EventId);
            if (evt is null){
                return Results.NotFound();
            }
            // CHECK EVENT DATE HAS NOT PASSED
            var today = DateTime.Now;
            if (today >= evt.Date){
                Console.WriteLine("CONCLUDED EVENT");
                return Results.NoContent();
            }
            //MISSING: CHECK EVENT HAS REMAINING TICKETS 

            //MISSING: CHECK IF USER HAS NOT ISSUED A TICKET 

            // Convert dto to entity 
            Booking booking = newBooking.ToEntity();
            // Add user
            dbContext.Bookings.Add(booking);
            // Commit changes
            await dbContext.SaveChangesAsync();
            return Results.CreatedAtRoute(getBookingEndpoint, new { id = booking.Id }, booking.ToBookingDetailsDto());
        });


        // Update booking by id 
        group.MapPut("/{id}", async(int id, UpdateBookingDto updatedBooking, BookingDbContext dbContext) =>{
            var existingBooking = await dbContext.Bookings.FindAsync(id);
            if (existingBooking is null){
                return Results.NotFound();
            }
            dbContext.Entry(existingBooking)
            .CurrentValues
            .SetValues(updatedBooking.ToEntity(id));
            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        });

        //  Delete booking by id
        group.MapDelete("/{id}", async(int id, BookingDbContext dbContext) =>{
            await dbContext.Bookings.Where(booking => booking.Id == id).ExecuteDeleteAsync();
            return Results.NoContent();
        });

        return group;
    }

}
