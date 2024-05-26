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

        // Get the currently logged-in user
        var user = await _userManager.GetUserAsync(req.HttpContext.User);

        if (user == null)
        {
            return new UnauthorizedResult();
        }

        // Construct the user info object
        var userInfo = new
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
        };

        // Return the user info as JSON
        return new JsonResult(userInfo)
        {
            StatusCode = StatusCodes.Status200OK
        };
    }
}
