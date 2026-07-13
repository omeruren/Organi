namespace Organi.Server.Domain.Exceptions;

public sealed class NotFoundException(string entityName, object key)
    : Exception($"{entityName} with ID '{key}' was not found.");
