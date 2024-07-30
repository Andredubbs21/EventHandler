namespace EventHandler.Api.Dtos.Event;

   public record EventSummaryDto(
        int Id,
        string Name,
        DateTime Date
    );