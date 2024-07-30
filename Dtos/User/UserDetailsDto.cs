namespace EventHandler.Api.Dtos.User;

public record class UserDetailsDto
(
    int Id,
    string Username,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber
);
