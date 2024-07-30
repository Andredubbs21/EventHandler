using System.ComponentModel.DataAnnotations;
namespace EventHandler.Api.Dtos.Booking;

public record class CreateBookingDto
(
    [Required][StringLength(50)]string Username,
    [Required] int EventId,
    [Required] int Amount
);