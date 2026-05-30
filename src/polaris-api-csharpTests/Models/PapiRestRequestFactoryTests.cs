using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;

namespace Clc.Polaris.Api.Tests.Models
{
    [TestClass]
    [TestCategory("Unit")]
    [TestCategory("RestClientMigration")]
    public class PapiRestRequestFactoryTests
    {
        [TestMethod]
        public void Get_SetsMethodPathPasswordAndBody()
        {
            var body = new { Value = "test" };

            var request = PapiRestRequest.Get("/public/factory-get", password: "pin", body: body);

            Assert.AreEqual(HttpMethod.Get, request.Method);
            Assert.AreEqual("/public/factory-get", request.Path);
            Assert.AreEqual("pin", request.Password);
            Assert.AreSame(body, request.Body);
        }

        [TestMethod]
        public void Post_SetsMethodAndBody()
        {
            var body = new { Value = "test" };

            var request = PapiRestRequest.Post("/public/factory-post", body: body);

            Assert.AreEqual(HttpMethod.Post, request.Method);
            Assert.AreEqual("/public/factory-post", request.Path);
            Assert.AreEqual(string.Empty, request.Password);
            Assert.AreSame(body, request.Body);
        }

        [TestMethod]
        public void Put_SetsMethodBodyAndPassword()
        {
            var body = new { Value = "test" };

            var request = PapiRestRequest.Put("/public/factory-put", body: body, password: "pin");

            Assert.AreEqual(HttpMethod.Put, request.Method);
            Assert.AreEqual("/public/factory-put", request.Path);
            Assert.AreEqual("pin", request.Password);
            Assert.AreSame(body, request.Body);
        }

        [TestMethod]
        public void Delete_SetsMethod()
        {
            var request = PapiRestRequest.Delete("/public/factory-delete");

            Assert.AreEqual(HttpMethod.Delete, request.Method);
            Assert.AreEqual("/public/factory-delete", request.Path);
            Assert.AreEqual(string.Empty, request.Password);
            Assert.IsNull(request.Body);
        }

        [TestMethod]
        public void Create_PreservesSuppliedMethodPathBodyAndPassword()
        {
            var body = new { Value = "test" };
            var method = new HttpMethod("PATCH");

            var request = PapiRestRequest.Create(method, "/public/factory-create", body: body, password: "pin");

            Assert.AreEqual(method, request.Method);
            Assert.AreEqual("/public/factory-create", request.Path);
            Assert.AreEqual("pin", request.Password);
            Assert.AreSame(body, request.Body);
        }
    }
}
