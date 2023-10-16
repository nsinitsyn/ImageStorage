namespace ImageStorage.Application.Common;

public class OperationResult
{
    private List<OperationError>? _errors;

    public bool IsSucceeded { get; private set; } = true;

    public void AddError(OperationError error)
    {
        IsSucceeded = false;
        Errors.Add(error);
    }

    public List<OperationError> Errors => _errors ??= new List<OperationError>();

    public override string ToString()
    {
        if(IsSucceeded)
        {
            return "Ok";
        }

        return string.Join(Environment.NewLine, Errors);
    }
}
