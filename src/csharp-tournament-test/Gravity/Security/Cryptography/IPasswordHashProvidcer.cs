namespace Gravity.Security.Cryptography
{
    public interface IPasswordHashProvidcer
    {
        string Hash(string clearText);
    }
}