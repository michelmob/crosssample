namespace Gravity.Security.Cryptography
{
    public interface IShortHashProvider
    {
        string Encode(int id);

        int Decode(string shortHash);
    }
}