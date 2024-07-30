﻿using System.ComponentModel.DataAnnotations;
namespace EventHandler.Api.Dtos.User;

public record class CreateUserDto(
    [Required][StringLength(50)] string Username,
    [Required][StringLength(25)] string FirstName,
    [Required][StringLength(25)] string LastName,
    [Required][EmailAddress]string Email,
    [StringLength(20)] string PhoneNumber
    
);