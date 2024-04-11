using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;


namespace BlazorTamagotchi.Pages
{
    public partial class Home
    {
        private const string GoogleClientId = "794918693940-j1kb0o1gi3utki6th2u6nmoc2i40kqbm.apps.googleusercontent.com";
        private const string GoogleClientSecret = "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";
        private const string GoogleRedirectUri = "http://localhost:7117/GoogleCallback";

        public string BuildGoogleOAuthUrl()
        {
            return new StringBuilder("https://accounts.google.com/o/oauth2/v2/auth?")
                .Append("response_type=code")
                .Append($"&client_id={Uri.EscapeDataString(GoogleClientId)}")
                .Append($"&redirect_uri={Uri.EscapeDataString(GoogleRedirectUri)}")
                .Append($"&scope={Uri.EscapeDataString("openid email profile")}")
                .Append("&access_type=offline")
                .ToString();

        }
    }
}


