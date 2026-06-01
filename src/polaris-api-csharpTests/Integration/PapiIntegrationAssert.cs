using Clc.Polaris.Api.Models;
using Clc.Rest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api;

public static class PapiIntegrationAssert
{
    public static T HasData<T>(IRestResponse<T> response)
    {
        Assert.IsNotNull(response, "The REST response wrapper should not be null.");
        Assert.IsNotNull(response.Response, "The underlying HTTP response should not be null.");
        Assert.IsNotNull(response.Data, "The response body should deserialize into the expected model.");
        return response.Data!;
    }

    public static T Success<T>(IRestResponse<T> response) where T : PapiResponseCommon
    {
        var data = HasData(response);
        Assert.AreEqual(0, data.PAPIErrorCode, data.ErrorMessage);
        return data;
    }

    public static T PapiError<T>(IRestResponse<T> response, int expectedCode, string? messageSubstring = null) where T : PapiResponseCommon
    {
        var data = HasData(response);
        Assert.AreEqual(expectedCode, data.PAPIErrorCode, data.ErrorMessage);
        if (!string.IsNullOrWhiteSpace(messageSubstring))
        {
            StringAssert.Contains(data.ErrorMessage ?? string.Empty, messageSubstring);
        }

        return data;
    }

    public static T PapiCode<T>(IRestResponse<T> response, int expectedCode) where T : PapiResponseCommon
    {
        var data = HasData(response);
        Assert.AreEqual(expectedCode, data.PAPIErrorCode, data.ErrorMessage);
        return data;
    }

    public static void RowCountEqualsPapiCode<T>(IRestResponse<T> response, int rowCount) where T : PapiResponseCommon
    {
        var data = HasData(response);
        Assert.AreEqual(rowCount, data.PAPIErrorCode, data.ErrorMessage);
    }

    public static void RequestUriContains<T>(IRestResponse<T> response, string expectedRouteSegment)
    {
        Assert.IsNotNull(response.Response?.RequestMessage?.RequestUri, "The HTTP request URI should be available for route assertions.");
        StringAssert.Contains(response.Response!.RequestMessage!.RequestUri!.ToString(), expectedRouteSegment);
    }
}
