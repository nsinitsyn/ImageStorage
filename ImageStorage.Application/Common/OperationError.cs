namespace ImageStorage.Application.Common;

public class OperationError
{
    public OperationError(OperationErrorCode code, string message)
    {
        Code = code;
        Message = message;
    }

    public OperationError(OperationErrorCode code) : this(code, string.Empty) { }

    public OperationError(string message) : this(OperationErrorCode.None, message) { }

    public OperationErrorCode Code { get; }

    public string Message { get; }

    public override string ToString() => Message;
}