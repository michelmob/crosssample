using System.Threading.Tasks;
using Gravity.Data;

namespace Gravity.Manager.Domain.Organizations
{
    public interface IUserRepository: IGenericRepository<User, long>
    {
        Task<User> GetUserByUsernameAsync(string userName);
        Task<int> RegisterNewUserAsync(string userName);
        Task<bool> CheckUserByUsernameAsync(string userName);
    }
}