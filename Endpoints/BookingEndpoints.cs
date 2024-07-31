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
        group.MapGet("/{id}", async (int id, BookingDbContext dbContext, EventDbContext eventDbContext) =>
        {
            // Search for bookings by id. result may be null 
            Booking? booking = await dbContext.Bookings.FindAsync(id);
            if (booking is null){
                return Results.NotFound();
            }
            BookingDetailsDto? detailedBooking = booking!.ToBookingDetailsDto();
            // Search for event by id, result may be null 
            Event? evt = await eventDbContext.Events.FindAsync(booking.EventId);
            if (evt is null){
                return Results.NotFound();
            }
            detailedBooking.EventName = evt.Name;
            // Return either null or a detailed version of the booking
            return booking is null ? Results.NotFound() : Results.Ok(detailedBooking);
        }).WithName(getBookingEndpoint);

        // Get bookings by username 
        group.MapGet("/user_name/{username}", async (string username, BookingDbContext dbContext, EventDbContext eventDbContext) =>
        {
            // Search for bookings by event id. result will be a list
            var bookings = await dbContext.Bookings
                .AsNoTracking()
                .Where(b => b.Username == username)
                .Select(b => b.ToBookingDetailsDto())
                .ToListAsync();
            foreach (var booking in bookings)
            {
                Event? evt = await eventDbContext.Events.FindAsync(booking.EventId);
                if (evt is null){
                    booking.EventName = "";
                }else{
                    booking.EventName = evt!.Name;
                }   
            }
            // Return the list of bookings
            return bookings.Count == 0 ? Results.NotFound() : Results.Ok(bookings);
        }).WithName("GetBookingByUsername");

        // Get bookings by event id 
        group.MapGet("/event_id/{id}", async (int id, BookingDbContext dbContext, EventDbContext eventDbContext) =>
        {
            // Search for bookings by event id. result will be a list
            var bookings = await dbContext.Bookings
                .AsNoTracking()
                .Where(b => b.EventId == id)
                .Select(b => b.ToBookingDetailsDto())
                .ToListAsync();
            foreach (var booking in bookings)
            {
                Event? evt = await eventDbContext.Events.FindAsync(booking.EventId);
                if (evt is null){
                    booking.EventName = "";
                }else{
                    booking.EventName = evt!.Name;
                }   
            }
            // Return the list of bookings
            return bookings.Count == 0 ? Results.NotFound() : Results.Ok(bookings);
        }).WithName("GetBookingByEventId");
                
        // Create Bookings
        group.MapPost("/", async (CreateBookingDto newBooking, BookingDbContext dbContext, UserDbContext userDbContext,
        EventDbContext eventDbContext) => 
        { 
            // Search for user by username. result may be null
            User? user = await userDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == newBooking.Username);     
            if (user is null){
                Console.WriteLine("User not found");
                return Results.NotFound();
            };
            // Search for event by id, result may be null 
            Event? evt = await eventDbContext.Events.FindAsync(newBooking.EventId);
            if (evt is null){
                Console.WriteLine("Event not found");
                return Results.NotFound();
            }
            // CHECK EVENT DATE HAS NOT PASSED
            var today = DateTime.Now;
            if (today >= evt.Date){
                Console.WriteLine("CONCLUDED EVENT");
                return Results.NoContent();
            }
            //MISSING: CHECK EVENT HAS REMAINING TICKETS 
            var eventBookings = await dbContext.Bookings
                .AsNoTracking()
                .Where(b => b.EventId == evt.Id)
                .Select(b => b.ToBookingDetailsDto())
                .ToListAsync();
             var totalAmount = eventBookings.Sum(b => b.Amount);
             totalAmount += newBooking.Amount;
             if (totalAmount > evt.MaxCapacity){
                Console.WriteLine("EVENT MAXED OUT");
                return Results.NoContent();
             } 
            //MISSING: CHECK IF USER HAS NOT ISSUED A TICKET 
            var userBookings = await dbContext.Bookings
                .AsNoTracking()
                .Where(b => b.Username == user.Username && b.EventId == newBooking.EventId)
                .Select(b => b.ToBookingDetailsDto())
                .ToListAsync();
            if (userBookings.Count > 0){
                Console.WriteLine("USER HAS BOOKED ALREADY");
                return Results.NoContent();
            };
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

        // Delete booking by username and event id 
        group.MapDelete("user_event/{username}/{id}", async(int id, string username, BookingDbContext dbContext, UserDbContext userDbContext, EventDbContext eventDbContext) =>{
            //check if booking exists
            var booking = await dbContext.Bookings
            .FirstOrDefaultAsync(b => b.Username == username && b.EventId == id);
            if(booking is null){
                Console.WriteLine("Booking not found");
                return Results.NotFound();
            }
            //Validate user exsits
             // Search for user by username. result may be null
            User? user = await userDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);     
            if (user is null){
                Console.WriteLine("User not found");
                return Results.NotFound();
            };
            // Search for event by id, result may be null 
            Event? evt = await eventDbContext.Events.FindAsync(id);
            if (evt is null){
                Console.WriteLine("Event not found");
                return Results.NotFound();
            }
            // CHECK EVENT DATE HAS NOT PASSED
            var today = DateTime.Now;
            if (today >= evt.Date){
                Console.WriteLine("CONCLUDED EVENT");
                return Results.NoContent();
            }
            dbContext.Bookings.Remove(booking);
            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        }); 
        return group;
    }

}
