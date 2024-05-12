using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class UserAccount : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set;}
    public string ProfileImage { get; set; } = null!;

    public string? AddressId { get; set; }

    public UserAddress? Address { get; set; }

}
