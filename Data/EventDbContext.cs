using EventHandler.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventHandler.Api.Data;

public class EventDbContext(DbContextOptions<EventDbContext> options) : DbContext(options)
{

    public DbSet<Event> Events => Set<Event>();

}
