namespace Organi.Server.Application.Common.Models;

public sealed record PaginationRequest(int Page = 1, int PageSize = 10);
