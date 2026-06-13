namespace Clc.Polaris.Api.Tests.Models
{
    [TestClass]
    [UnitTest]
    public sealed class HoldRequestCreateParamsTests
    {
        [TestMethod]
        public void HoldRequestCreateParams_DefaultContextIds_AreNull()
        {
            var holdParams = new HoldRequestCreateParams();

            Assert.IsNull(holdParams.WorkstationID);
            Assert.IsNull(holdParams.UserID);
            Assert.IsNull(holdParams.RequestingOrgID);
        }

        [TestMethod]
        public void HoldRequestCreateParams_DefaultPickupOrgId_IsZero()
        {
            var holdParams = new HoldRequestCreateParams();

            Assert.AreEqual(0, holdParams.PickupOrgID);
        }

        [TestMethod]
        public void HoldRequestCreateParams_Constructor_AssignsProvidedValues()
        {
            var holdParams = new HoldRequestCreateParams(1, 2, 3, 4);

            Assert.AreEqual(1, holdParams.PatronID);
            Assert.AreEqual(2, holdParams.BibID);
            Assert.AreEqual(3, holdParams.PickupOrgID);
            Assert.AreEqual(4, holdParams.RequestingOrgID);
            Assert.IsNull(holdParams.WorkstationID);
            Assert.IsNull(holdParams.UserID);
        }
    }
}
