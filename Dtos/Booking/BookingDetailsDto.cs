namespace EventHandler.Api.Dtos.Booking;

public record class BookingDetailsDto
(
    int Id, 
    string Username,
    int EventId,
    int Amount
);
