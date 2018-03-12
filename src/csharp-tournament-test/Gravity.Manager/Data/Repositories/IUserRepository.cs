using System.Threading.Tasks;
using Gravity.Data;
using Gravity.Manager.Data.Entities;

namespace Gravity.Manager.Data.Repositories
{
    public interface IUserRepository: IGenericRepository<User, long>
    {
        Task<User> GetUserByUsernameAsync(string userName);
        Task<int> RegisterNewUserAsync(string userName);
        Task<bool> CheckUserByUsernameAsync(string userName);
    }
}