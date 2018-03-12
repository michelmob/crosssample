using System;
using System.Linq;
using Gravity.Runtime.Serialization;
using Microsoft.AspNetCore.Http;

namespace Gravity.Manager.Web.Application
{
    public class UserStateWrapper : IUserStateWrapper
    {
        public const string UserStateClaimKey = "UserStateClaim";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISerializer _serializer;

        public UserStateWrapper(
            IHttpContextAccessor httpContextAccessor
            , ISerializer serializer
        )
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public UserState GetUserState()
        {
            UserState userState = null;
            var principal = _httpContextAccessor?.HttpContext.User;
            if (principal == null) return null;

            var userStateClaim = principal.Claims.FirstOrDefault(c => c.Type == UserStateClaimKey);
            if (userStateClaim != null)
            {
                userState = _serializer.Deserialize<UserState>(userStateClaim.Value);
            }

            return userState;
        }

    }
}