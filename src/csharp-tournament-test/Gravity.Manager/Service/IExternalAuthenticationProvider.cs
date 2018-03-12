namespace Gravity.Manager.Service
{
    public interface IExternalAuthenticationProvider
    {
        bool Authenticate(string logonName, string logonPassword);
    }
}