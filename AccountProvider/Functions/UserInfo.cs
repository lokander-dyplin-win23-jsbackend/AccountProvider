using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Data.Entities;
using Microsoft.Azure.Functions.Worker;

public class UserInfo
{
    private readonly ILogger<UserInfo> _logger;
    private readonly UserManager<UserAccount> _userManager;

    public UserInfo(ILogger<UserInfo> logger, UserManager<UserAccount> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    [Function("UserInfo")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        // Får den inloggade användaren
        var user = await _userManager.GetUserAsync(req.HttpContext.User);

        if (user == null)
        {
            return new UnauthorizedResult();
        }

        // skapar ett användare objekt som vi sedan kan mappa till account
        var userInfo = new
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
        };

        return new OkObjectResult(userInfo);
    }
}
