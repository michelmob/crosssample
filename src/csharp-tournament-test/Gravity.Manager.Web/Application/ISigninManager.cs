using System.Threading.Tasks;

namespace Gravity.Manager.Web.Application
{
    public interface ISigninManager
    {
        Task SignInAsync(UserState userState);
        Task SignOffAsync();
    }
}