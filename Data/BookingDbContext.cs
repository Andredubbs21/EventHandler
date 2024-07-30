using EventHandler.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventHandler.Api.Data;

public class BookingDbContext(DbContextOptions<BookingDbContext> options) : DbContext(options)
{
    public DbSet<Booking> Bookings => Set<Booking>();

}
