using System.ComponentModel.DataAnnotations;
namespace EventHandler.Api.Dtos.Event;

    public record class CreateEventDto
    (
        [Required][StringLength(50)]string Name,
        [StringLength(250)]string Description,
        [Required]DateTime Date,
        [Required]int MaxCapacity
    );