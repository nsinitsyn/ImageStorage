namespace ImageStorage.Application.Dependencies;

public interface IHashCalculator
{
    byte[] GetHash(string str);
}
