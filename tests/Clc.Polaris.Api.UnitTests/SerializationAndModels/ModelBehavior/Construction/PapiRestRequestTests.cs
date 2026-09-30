namespace Clc.Polaris.Api.UnitTests.SerializationAndModels.ModelBehavior.Construction
{
    [TestClass]
    [UnitTest]
    public class PapiRestRequestTests
    {
        [TestMethod]
        public void PapiRestRequest_StringConstructor_DefaultsToGetAndSetsPath()
        {
            var request = new PapiRestRequest("/public/foo");

            Assert.AreEqual(HttpMethod.Get, request.Method);
            Assert.AreEqual("/public/foo", request.Path);
            Assert.AreEqual("", request.Password);
            Assert.IsNull(request.Body);
        }

        [TestMethod]
        public void PapiRestRequest_HttpMethodConstructor_PreservesMethodPathPasswordAndBody()
        {
            var body = new { Name = "Example" };

            var request = new PapiRestRequest(HttpMethod.Put, "/public/foo", "pin", body);

            Assert.AreEqual(HttpMethod.Put, request.Method);
            Assert.AreEqual("/public/foo", request.Path);
            Assert.AreEqual("pin", request.Password);
            Assert.AreSame(body, request.Body);
        }

        [TestMethod]
        public void PapiRestRequest_CopyConstructor_PreservesValuesWithoutSharingMutableCollections()
        {
            var request = PapiRestRequest.Post("/protected/v1/1033/100/1/test", new { Value = 1 }, "password");

            request.AuthRequired = false;
            request.JsonSerializerIgnoreNulls = false;
            request.BlockStaffOverride = true;
            request.HashString = "hash-string";
            request.Headers["X-Test"] = "header-value";
            request.QueryParameters["query"] = "query-value";

            var copy = new PapiRestRequest(request);

            Assert.AreEqual(request.Method, copy.Method);
            Assert.AreEqual(request.Path, copy.Path);
            Assert.AreSame(request.Body, copy.Body);
            Assert.AreEqual(request.Password, copy.Password);
            Assert.AreEqual(request.AuthRequired, copy.AuthRequired);
            Assert.AreEqual(request.JsonSerializerIgnoreNulls, copy.JsonSerializerIgnoreNulls);
            Assert.AreEqual(request.BlockStaffOverride, copy.BlockStaffOverride);
            Assert.AreEqual(request.HashString, copy.HashString);

            Assert.AreNotSame(request.Headers, copy.Headers);
            Assert.AreNotSame(request.QueryParameters, copy.QueryParameters);

            Assert.AreEqual("header-value", copy.Headers["X-Test"]);
            Assert.AreEqual("query-value", copy.QueryParameters["query"]);

            request.Headers["X-Test"] = "changed-header-value";
            request.QueryParameters["query"] = "changed-query-value";
            request.Headers["X-New"] = "new-header-value";
            request.QueryParameters["new"] = "new-query-value";

            Assert.AreEqual("header-value", copy.Headers["X-Test"]);
            Assert.AreEqual("query-value", copy.QueryParameters["query"]);
            Assert.IsFalse(copy.Headers.ContainsKey("X-New"));
            Assert.IsFalse(copy.QueryParameters.ContainsKey("new"));
        }
    }
}
