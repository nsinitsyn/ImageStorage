namespace ImageStorage.Application.Common;

public class OperationError
{
    public OperationError(string message)
    {
        Message = message;
    }

    public string Message { get; }

    public override string ToString() => Message;
}
