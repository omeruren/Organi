using Organi.Server.Application.Common.Models;

namespace Organi.Server.WebAPI.Extensions;

public static class ResultExtensions
{
    public static IResult ToApiResult<T>(this Result<T> result) =>
        result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Unauthorized",
                detail: result.Error!.Message);
}
