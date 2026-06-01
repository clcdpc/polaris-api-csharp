using Clc.Rest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Reflection;

namespace Clc.Polaris.Api.Tests.Integration;

internal static class PapiIntegrationAssert
{
    public static T HasData<T>(IRestResponse<T> response)
    {
        Assert.IsNotNull(response, "The REST response should not be null.");
        Assert.IsNotNull(response.Response, "The underlying HTTP response should not be null.");
        Assert.IsNotNull(response.Data, "The response body should deserialize into the expected model.");
        return response.Data;
    }

    public static T HasPapiData<T>(IRestResponse<T> response)
    {
        var data = HasData(response);
        _ = GetPapiErrorCode(data);
        return data;
    }

    public static T Success<T>(IRestResponse<T> response)
    {
        var data = HasPapiData(response);
        var errorCode = GetPapiErrorCode(data);
        Assert.AreEqual(0, errorCode, $"Expected PAPI success but received {errorCode}: {GetErrorMessage(data)}");
        return data;
    }

    public static T PapiError<T>(IRestResponse<T> response, int expectedErrorCode)
    {
        var data = HasPapiData(response);
        var errorCode = GetPapiErrorCode(data);
        Assert.AreEqual(expectedErrorCode, errorCode, $"Unexpected PAPI error code. ErrorMessage: {GetErrorMessage(data)}");
        return data;
    }

    public static void RowCountMatchesPapiCode<T>(IRestResponse<T> response, int rowCount)
    {
        var data = HasPapiData(response);
        Assert.AreEqual(rowCount, GetPapiErrorCode(data), "For list endpoints, the Polaris guide documents the PAPIErrorCode as the returned row count on success.");
    }

    public static void RequestUriContains<T>(IRestResponse<T> response, string expectedSegment)
    {
        Assert.IsNotNull(response.Response?.RequestMessage?.RequestUri, "The request URI should be available for route assertions.");
        var requestUri = response.Response!.RequestMessage!.RequestUri!.ToString();
        StringAssert.Contains(requestUri, expectedSegment);
    }

    public static T HttpUnauthorized<T>(IRestResponse<T> response)
    {
        var data = HasData(response);
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.Response!.StatusCode);
        return data;
    }

    public static int GetPapiErrorCode<T>(T data)
    {
        Assert.IsNotNull(data, "The response body should not be null.");
        var property = data!.GetType().GetProperty("PAPIErrorCode", BindingFlags.Public | BindingFlags.Instance);
        Assert.IsNotNull(property, $"{data.GetType().Name} should expose a PAPIErrorCode property.");
        return (int)(property!.GetValue(data) ?? 0);
    }

    public static object? GetErrorMessage<T>(T data)
    {
        if (data == null)
        {
            return null;
        }

        return data.GetType().GetProperty("ErrorMessage", BindingFlags.Public | BindingFlags.Instance)?.GetValue(data);
    }
}
