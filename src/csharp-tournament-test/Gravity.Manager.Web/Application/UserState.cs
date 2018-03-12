using System;
using Gravity.Manager.Domain.Organizations;

namespace Gravity.Manager.Web.Application
{
    public class UserState
    {
        // ReSharper disable once UnusedMember.Global (serialization).
        public UserState()
        {
        }

        public UserState(User user)
        {
            Id = user.Id;
            OrganizationId = user.OrganizationId;
            UserName = user.UserName;
            Name = user.Name;
            EMail = user.EMail;
            Role = user.Role;
        }

        public static UserState FromUser(User user)
        {
            return new UserState(user ?? throw new ArgumentNullException(nameof(user)));
        }

        public static User ToUser(UserState userState)
        {
            if (userState == null)
            {
                throw new ArgumentNullException(nameof(userState));
            }
            
            return new User
            {
                Id = userState.Id,
                OrganizationId = userState.OrganizationId,
                UserName = userState.UserName,
                Name = userState.Name,
                EMail = userState.EMail,
                Role = userState.Role
            };
        }
        
        public long Id { get; set; }
        
        public long? OrganizationId { get; set; }
        
        public string UserName { get; set; }
        
        public string Name { get; set; }
        
        public string EMail { get; set; }
        
        public RoleType Role { get; set; }
    }
}