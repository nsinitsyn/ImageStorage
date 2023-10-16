namespace ImageStorage.Application.Common;

public class OperationResult<T> : OperationResult
{
    public T? Value { get; set; }
}
