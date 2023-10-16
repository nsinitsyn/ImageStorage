namespace ImageStorage.Infrastructure.DbAccess;

/// <summary>
/// https://www.postgresql.org/docs/current/errcodes-appendix.html
/// </summary>
internal static class PostgreSqlErrorCodes
{
    public const string UniqueViolation = "23505";
}
