using System.Threading.Tasks;
using Gravity.Data.EF;
using Gravity.Manager.Domain.Organizations;
using Microsoft.EntityFrameworkCore;

namespace Gravity.Manager.Data.EF.Repositories
{
    public class UserRepository : EntityRepository<User>, IUserRepository
    {
        public UserRepository(DbContext context) : base(context)
        {
            // No-op.
        }

        public Task<User> GetUserByUsernameAsync(string userName)
        {
            return AsQueryable().FirstOrDefaultAsync(u => u.UserName == userName);
        }
        
        public Task<bool> CheckUserByUsernameAsync(string userName)
        {
            return AsQueryable().AnyAsync(u => u.UserName == userName);
        }

        public async Task<int> RegisterNewUserAsync(string userName)
        {
            var existingUser = await CheckUserByUsernameAsync(userName);
            
            if (existingUser) throw new GravityException("User with the same user name already exist.");
            
            var user = new User
            {
                UserName = userName
            };

            return await InsertAsync(user);
        }
        
    }
}