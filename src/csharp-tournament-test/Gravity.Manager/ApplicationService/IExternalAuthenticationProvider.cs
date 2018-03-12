namespace Gravity.Manager.ApplicationService
{
    public interface IExternalAuthenticationProvider
    {
        bool Authenticate(string logonName, string logonPassword);
    }
}