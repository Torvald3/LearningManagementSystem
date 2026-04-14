using System.Net.Http.Json;

namespace LMS.Tests.Infrastructure;

public static class HttpResponseMessageExtensions
{
    public static async Task<T> ReadAsJsonAsync<T>(this HttpResponseMessage response)
    {
        var value = await response.Content.ReadFromJsonAsync<T>();
        if (value is null)
            throw new InvalidOperationException("Response body was null.");

        return value;
    }
}