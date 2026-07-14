namespace Organi.Server.Application.Common.Utilities;

public static class OrderNumberGenerator
{
    public static string Generate() =>
        $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";

    public static async Task<string> GenerateUniqueAsync(Func<string, Task<bool>> orderNumberExistsAsync)
    {
        var candidate = Generate();

        while (await orderNumberExistsAsync(candidate))
        {
            candidate = Generate();
        }

        return candidate;
    }
}
