using Clc.Polaris.Api.Models;
using Clc.Rest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

public static class PapiIntegrationAssert
{
    public static TData HasData<TData>(IRestResponse<TData> response)
    {
        Assert.IsNotNull(response, "The REST response wrapper should not be null.");
        Assert.IsNotNull(response.Response, "The underlying HTTP response should not be null.");
        Assert.IsNotNull(response.Data, "The response body should deserialize into the expected model.");
        return response.Data!;
    }

    public static TData Success<TData>(IRestResponse<TData> response) where TData : PapiResponseCommon
    {
        var data = HasData(response);
        Assert.AreEqual(0, data.PAPIErrorCode, $"Expected PAPI success, but received {data.PAPIErrorCode}: {data.ErrorMessage}");
        return data;
    }

    public static TData ErrorCode<TData>(IRestResponse<TData> response, int expectedCode) where TData : PapiResponseCommon
    {
        var data = HasData(response);
        Assert.AreEqual(expectedCode, data.PAPIErrorCode, $"Expected PAPI error code {expectedCode}, but received {data.PAPIErrorCode}: {data.ErrorMessage}");
        return data;
    }

    public static TData ErrorCodeIn<TData>(IRestResponse<TData> response, params int[] expectedCodes) where TData : PapiResponseCommon
    {
        var data = HasData(response);
        CollectionAssert.Contains(expectedCodes, data.PAPIErrorCode, $"Expected one of [{string.Join(", ", expectedCodes)}], but received {data.PAPIErrorCode}: {data.ErrorMessage}");
        return data;
    }

    public static void PapiCodeEqualsRowCount(int papiErrorCode, int rowCount, string rowName)
    {
        Assert.AreEqual(rowCount, papiErrorCode, $"Expected PAPIErrorCode to equal {rowName} row count.");
    }

    public static void RequestUriContains<TData>(IRestResponse<TData> response, string expectedSegment)
    {
        Assert.IsNotNull(response.Response?.RequestMessage?.RequestUri, "The request URI should be available for route assertions.");
        StringAssert.Contains(response.Response!.RequestMessage!.RequestUri!.ToString(), expectedSegment);
    }
}
