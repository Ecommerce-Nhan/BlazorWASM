using System.Text.Json.Serialization;
using System.Text.Json;
using SharedLibrary.Wrappers;

namespace ECommerce.Infrastructure.Extensions;

public static class ResponseExtensions
{
    internal static async Task<IResponse<T>> ToResponse<T>(this HttpResponseMessage response)
    {
        var responseAsString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<Response<T>>(responseAsString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.Preserve
        });
        if (responseObject is null)
        {
            throw new InvalidOperationException("Unable to deserialize response content.");
        }
        return responseObject;
    }

    internal static async Task<IResponse> ToResponse(this HttpResponseMessage response)
    {
        var responseAsString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<Response>(responseAsString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.Preserve
        });
        if (responseObject is null)
        {
            throw new InvalidOperationException("Unable to deserialize response content.");
        }
        return responseObject;
    }

    internal static async Task<PagedResponse<T>> ToPaginatedResponse<T>(this HttpResponseMessage response)
    {
        var responseAsString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<PagedResponse<T>>(responseAsString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        if (responseObject is null)
        {
            throw new InvalidOperationException("Unable to deserialize response content.");
        }
        return responseObject;
    }

    internal static async Task<T> ToSelfResponse<T>(this HttpResponseMessage response)
    {
        var responseAsString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<T>(responseAsString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.Preserve
        });
        if (responseObject is null)
        {
            throw new InvalidOperationException("Unable to deserialize response content.");
        }
        return responseObject;
    }
}