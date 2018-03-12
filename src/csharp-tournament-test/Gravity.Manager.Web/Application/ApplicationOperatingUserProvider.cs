using System;
using Gravity.Manager.Data.Entities;
using Gravity.Manager.Service;

namespace Gravity.Manager.Web.Application
{
    public class ApplicationOperatingUserProvider: IOperatingUserProvider
    {
        private readonly IUserStateWrapper _userStateWrapper;

        public ApplicationOperatingUserProvider(IUserStateWrapper userStateWrapper)
        {
            _userStateWrapper = userStateWrapper ?? throw new ArgumentNullException(nameof(userStateWrapper));
        }
        
        public User GetOperatingUser()
        {
            var userState = _userStateWrapper.GetUserState();
            return userState == null ? null : UserState.ToUser(userState);
        }
    }
}