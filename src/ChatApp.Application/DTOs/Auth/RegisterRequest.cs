﻿namespace ChatApp.Application.DTOs.Auth;

public record RegisterRequest(string Email, string Password, string FirstName, string LastName, string PhoneNumber);