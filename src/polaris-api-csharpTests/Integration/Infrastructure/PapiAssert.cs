using Clc.Polaris.Api.Models;
using Clc.Rest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration.Infrastructure;

public static class PapiAssert
{
    public static T HasData<T>(IRestResponse<T> response)
    {
        Assert.IsNotNull(response, "Expected a REST response object.");
        Assert.IsNotNull(response.Response, "Expected an HTTP response object.");
        Assert.IsNotNull(response.Data, "Expected the PAPI response body to deserialize.");
        return response.Data;
    }

    public static T Success<T>(IRestResponse<T> response) where T : PapiResponseCommon
    {
        var data = HasData(response);
        Assert.AreEqual(0, data.PAPIErrorCode, $"Expected PAPI success but received {data.PAPIErrorCode}: {data.ErrorMessage}");
        return data;
    }

    public static T PapiError<T>(IRestResponse<T> response, int expectedCode) where T : PapiResponseCommon
    {
        var data = HasData(response);
        Assert.AreEqual(expectedCode, data.PAPIErrorCode, $"Unexpected PAPIErrorCode. Message: {data.ErrorMessage}");
        return data;
    }

    public static T RowCountMatchesPapiCode<T>(IRestResponse<T> response, int rowCount) where T : PapiResponseCommon
    {
        var data = HasData(response);
        Assert.AreEqual(rowCount, data.PAPIErrorCode, "Expected PAPIErrorCode to equal the returned row count for this list endpoint.");
        return data;
    }

    public static void RequestUriContains<T>(IRestResponse<T> response, string expectedSegment)
    {
        var uri = response.Response?.RequestMessage?.RequestUri?.ToString();
        Assert.IsFalse(string.IsNullOrWhiteSpace(uri), "Expected response to expose the request URI.");
        StringAssert.Contains(uri!, expectedSegment);
    }
}
