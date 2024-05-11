﻿namespace AccountProvider.Models;

public class UserLoginRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    public bool IsPersistant { get; set; }
}
