namespace EventHandler.Api.Dtos.User;

public record class UpdateUserDto
(
    string Username,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber
);