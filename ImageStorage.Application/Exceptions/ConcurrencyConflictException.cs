namespace ImageStorage.Application.Exceptions;

public class ConcurrencyConflictException : Exception
{
    public string PropertyName { get; private set; }

    public ConcurrencyConflictException(string propertyName)
    {
        PropertyName = propertyName;
    }
}
