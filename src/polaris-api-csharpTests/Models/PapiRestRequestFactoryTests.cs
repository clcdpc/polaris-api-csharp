namespace Clc.Polaris.Api.Tests.Models
{
    [TestClass]
    [UnitTest]
    public class PapiRestRequestFactoryTests
    {
        [TestMethod]
        public void Get_SetsMethodPathPasswordAndBody()
        {
            var body = new { Value = "example" };

            var request = PapiRestRequest.Get("/public/foo", password: "pin", body: body);

            Assert.AreEqual(HttpMethod.Get, request.Method);
            Assert.AreEqual("/public/foo", request.Path);
            Assert.AreEqual("pin", request.Password);
            Assert.AreSame(body, request.Body);
        }

        [TestMethod]
        public void Post_SetsMethodAndBody()
        {
            var body = new { Value = "example" };

            var request = PapiRestRequest.Post("/public/foo", body: body);

            Assert.AreEqual(HttpMethod.Post, request.Method);
            Assert.AreEqual("/public/foo", request.Path);
            Assert.AreSame(body, request.Body);
        }

        [TestMethod]
        public void Put_SetsMethodBodyAndPassword()
        {
            var body = new { Value = "example" };

            var request = PapiRestRequest.Put("/public/foo", body: body, password: "pin");

            Assert.AreEqual(HttpMethod.Put, request.Method);
            Assert.AreEqual("/public/foo", request.Path);
            Assert.AreSame(body, request.Body);
            Assert.AreEqual("pin", request.Password);
        }

        [TestMethod]
        public void Delete_SetsMethod()
        {
            var request = PapiRestRequest.Delete("/public/foo");

            Assert.AreEqual(HttpMethod.Delete, request.Method);
            Assert.AreEqual("/public/foo", request.Path);
        }

        [TestMethod]
        public void Create_PreservesSuppliedMethodPathBodyAndPassword()
        {
            var body = new { Value = "example" };

            var request = PapiRestRequest.Create(HttpMethod.Patch, "/public/foo", body: body, password: "pin");

            Assert.AreEqual(HttpMethod.Patch, request.Method);
            Assert.AreEqual("/public/foo", request.Path);
            Assert.AreSame(body, request.Body);
            Assert.AreEqual("pin", request.Password);
        }
    }
}
