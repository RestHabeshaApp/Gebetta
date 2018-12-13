using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StandardMVC.JWT;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StandardMVC.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly JWTSettings _options;

        public AccountController(
      UserManager<IdentityUser> userManager,
      SignInManager<IdentityUser> signInManager,
      IOptions<JWTSettings> optionsAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _options = optionsAccessor.Value;
        }


        public async Task<IActionResult> Register(Credentials Credentials){

            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Credentials.Email, Email = Credentials.Email };
                var result = await _userManager.CreateAsync(user, Credentials.Password);



                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return new JsonResult(new Dictionary<string, object>
                    {
                        {"access_token", GetAccessToken(Credentials.Email)},

                        { "id_token", GetIdToken(user) }
                    });
                }

                return Errors(result);
                }

            return Error("Unexpected error");

            }


                private string GetIdToken(IdentityUser user)
                {
                    var payload = new Dictionary<string, object>
                      {
                        { "id", user.Id },
                        { "sub", user.Email },
                        { "email", user.Email },
                        { "emailConfirmed", user.EmailConfirmed },
                      };
                    return GetToken(payload);
                }
        

            private string GetAccessToken(string Email)
            {
                var payload = new Dictionary<string, object>
                  {
                    { "sub", Email },
                    { "email", Email }
                  };
                return GetToken(payload);
            }


            private string GetToken(Dictionary<string, object> payload)
            {
                var secret = _options.SecretKey;

                payload.Add("iss", _options.Issuer);
                payload.Add("aud", _options.Audience);
                payload.Add("nbf", ConvertToUnixTimestamp(DateTime.Now));
                payload.Add("iat", ConvertToUnixTimestamp(DateTime.Now));
                payload.Add("exp", ConvertToUnixTimestamp(DateTime.Now.AddDays(7)));
                IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
                IJsonSerializer serializer = new JsonNetSerializer();
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

                return encoder.Encode(payload, secret);
            }

        private JsonResult Errors(IdentityResult result)
        {
            var items = result.Errors
                .Select(x => x.Description)
                .ToArray();
            return new JsonResult(items) { StatusCode = 400 };
        }

        private JsonResult Error(string message)
        {
            return new JsonResult(message) { StatusCode = 400 };
        }

        private static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }


        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
    }
}
