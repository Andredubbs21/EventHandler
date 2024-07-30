using Microsoft.EntityFrameworkCore;

namespace EventHandler.Api.Data
{
    public static class DataExtensions
    {
        public static async Task MigrateDbAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var userContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
            await userContext.Database.MigrateAsync();

            var eventContext = scope.ServiceProvider.GetRequiredService<EventDbContext>();
            await eventContext.Database.MigrateAsync();

            var bookingContext = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
            await bookingContext.Database.MigrateAsync();
        }
    }
}
