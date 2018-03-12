using Gravity.Manager.ApplicationService;
using Gravity.Manager.Domain.Organizations;

namespace Gravity.Manager.Data.EF.Tests
{
    public class DefaultUserProvider : IOperatingUserProvider
    {
        public static long UserId { get; } = 0;

        public User GetOperatingUser() =>
            new User
            {
                Id = UserId,
                EMail = "default-user@domain.com",
                Name = "default-user",
                Role = RoleType.Admin,
                UserName = "Default User",
                Organization = new Organization
                {
                    Name = "acme"
                }
            };
    }
}