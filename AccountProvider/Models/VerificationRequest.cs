
namespace AccountProvider.Models;

public class VerificationRequest
{
    public string Email { get; set; } = null!;
    public string VerficationCode { get; set; } = null!;
}
