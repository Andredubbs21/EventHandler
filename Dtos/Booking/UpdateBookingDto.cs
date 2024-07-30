namespace EventHandler.Api.Dtos.Booking;

public record class UpdateBookingDto
(
    string Username,
    int EventId,
    int Amount
);