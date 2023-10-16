namespace ImageStorage.Application.Common;

public enum OperationErrorCode
{
    None = 0,
    NotFound,
    AccessDenied,
    NotAuthorized,
    ServerError
}