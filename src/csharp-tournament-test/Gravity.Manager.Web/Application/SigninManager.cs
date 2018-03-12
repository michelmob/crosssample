using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Gravity.Configuration;
using Gravity.Diagnostics;
using Gravity.Runtime.Serialization;
using Gravity.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace Gravity.Manager.Web.Application
{
    public class SigninManager : ISigninManager
    {
        private readonly ILogger _logger;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ISerializer _serializer;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISettingsProvider _settingsProvider;

        public SigninManager(
            ILogger logger,
            IDateTimeProvider dateTimeProvider
            , ISerializer serializer
            , IHttpContextAccessor httpContextAccessor
            , ISettingsProvider settingsProvider
            )
        {
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
            _serializer = serializer;
            _httpContextAccessor = httpContextAccessor;
            _settingsProvider = settingsProvider;
        }

        public async Task SignInAsync(UserState userState)
        {
            var data = (string)_serializer.Serialize(userState);
            var userStateClaim = new Claim(UserStateWrapper.UserStateClaimKey, data);

            var claims = new List<Claim>
            {
                userStateClaim
            };

//            var properties = new AuthenticationProperties()
//            {
//                AllowRefresh = true,
//                ExpiresUtc = _dateTimeProvider.Now().AddHours(3),
//                IsPersistent = false,
//                IssuedUtc = _dateTimeProvider.Now()                    
//            };
            
            claims.Add(new Claim(ClaimTypes.Name, userState.Name));
                        
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);
            
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        public async Task SignOffAsync()
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

    }
}