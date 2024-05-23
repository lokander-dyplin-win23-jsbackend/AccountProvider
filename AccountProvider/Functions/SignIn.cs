using AccountProvider.Models;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace AccountProvider.Functions
{
    public class SignIn(ILogger<SignIn> logger, SignInManager<UserAccount> signInManager, UserManager<UserAccount> userManager )
    {
        private readonly ILogger<SignIn> _logger = logger;
        private readonly UserManager<UserAccount> _userManager = userManager;
        private readonly SignInManager<UserAccount> _signInManager = signInManager;

        [Function("SignIn")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            string body = null!;
            try
            {
                body = await new StreamReader(req.Body).ReadToEndAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"StreamReader :: {ex.Message}");
            }

            if (body != null)
            {
                UserLoginRequest ulr = null!;
                try
                {
                    ulr = JsonConvert.DeserializeObject<UserLoginRequest>(body)!;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"JsonConvert :: {ex.Message}");
                }

                if (ulr != null && !string.IsNullOrEmpty(ulr.Email) && !string.IsNullOrEmpty(ulr.Password))
                {
                    try
                    {
                        var userAccount = await _userManager.FindByEmailAsync(ulr.Email);
                        var result = await _signInManager.CheckPasswordSignInAsync(userAccount!, ulr.Password, false);
                        if (result.Succeeded)
                        {
                            if (userAccount!.Email != null && userAccount.Id != null)
                            {
                                TokenRequest trm = new TokenRequest
                                {
                                    Email = userAccount.Email!,
                                    UserId = userAccount.Id,
                                };

                                try
                                {
                                    trm = JsonConvert.DeserializeObject<TokenRequest>(body)!;
                                }

                                catch (Exception ex)
                                {
                                    _logger.LogError($"JsonConvert TRM :: {ex.Message}");
                                }


                                using var http = new HttpClient();
                                StringContent content = new StringContent(JsonConvert.SerializeObject(trm), Encoding.UTF8, "application/json");
                                var response = await http.PostAsync("https://tokenprovider-silicons.azurewebsites.net/api/token/generate?code=wuBVxONg8p6LBOAPV_MQ8E2Q4YpHZ1yZqBai51eSCNEAAzFut47-Uw%3D%3D", content);
                                return new OkObjectResult("accessToken");
                            }

                            return new UnauthorizedResult();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"await _signInManager.PasswordSignInAsync :: {ex.Message}");

                    }
                }
            }


            return new BadRequestResult();
        }
    }
}
