using ImageStorage.Application.Dependencies;
using System.Security.Cryptography;
using System.Text;

namespace ImageStorage.WebApi.Helpers;

public class HashCalculator : IHashCalculator
{
    public byte[] GetHash(string str)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(str);

        using var sha = SHA256.Create();
        return sha.ComputeHash(buffer);
    }
}
