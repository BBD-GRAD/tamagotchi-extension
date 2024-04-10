using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text;

namespace WebTamagotchi.Controllers
{
    [AllowAnonymous, Route("account")]
    public class AccountControllercs : Controller
    {
        private const string GoogleClientId = "794918693940-j1kb0o1gi3utki6th2u6nmoc2i40kqbm.apps.googleusercontent.com";
        private const string GoogleClientSecret = "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";
        private const string GoogleRedirectUri = "https://localhost:32813/api/auth/GoogleCallback";

        [Route("google-login")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value,
                claim.Properties
            });

            return Json(claims);
        }

        //private static string BuildGoogleOAuthUrl()
        //{
        //    return new StringBuilder("https://accounts.google.com/o/oauth2/v2/auth?")
        //        .Append("response_type=code")
        //        .Append($"&client_id={Uri.EscapeDataString(GoogleClientId)}")
        //        .Append($"&redirect_uri={Uri.EscapeDataString(GoogleRedirectUri)}")
        //        .Append($"&scope={Uri.EscapeDataString("openid email profile")}")
        //        .Append("&access_type=offline")
        //        .ToString();
        //}

        //[HttpGet("GoogleCallback")]
        //public async Task<IActionResult> GoogleCallback(string code)
        //{
        //    var tokenResponse = await ExchangeCodeForToken(code);
        //    if (!tokenResponse.IsSuccessStatusCode)
        //    {
        //        var errorResponse = await tokenResponse.Content.ReadAsStringAsync();
        //        return BadRequest(errorResponse);
        //    }

        //    var responseContent = await tokenResponse.Content.ReadAsStringAsync();
        //    var jsonResponse = JObject.Parse(responseContent);
        //    var idToken = jsonResponse["id_token"].ToString();
        //    var refreshToken = jsonResponse["refresh_token"].ToString(); // Capture the refresh token

        //    return Ok(new { Jwt = idToken, RefreshToken = refreshToken });
        //}

        //private static async Task<HttpResponseMessage> ExchangeCodeForToken(string code)
        //{
        //    var contentString = new StringBuilder()
        //        .Append("code=").Append(Uri.EscapeDataString(code))
        //        .Append("&client_id=").Append(Uri.EscapeDataString(GoogleClientId))
        //        .Append("&client_secret=").Append(Uri.EscapeDataString(GoogleClientSecret))
        //        .Append("&redirect_uri=").Append(Uri.EscapeDataString(GoogleRedirectUri))
        //        .Append("&grant_type=authorization_code")
        //        .ToString();

        //    using (var client = new HttpClient())
        //    {
        //        var content = new StringContent(contentString, Encoding.UTF8, "application/x-www-form-urlencoded");
        //        return await client.PostAsync("https://oauth2.googleapis.com/token", content);
        //    }
        //}
    }
}
