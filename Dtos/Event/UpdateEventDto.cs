namespace EventHandler.Api.Dtos.Event;

public record class UpdateEventDto
(
    string Name,
    string Description,
    DateTime Date,
    int MaxCapacity
);