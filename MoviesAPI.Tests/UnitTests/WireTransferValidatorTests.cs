using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesAPI.Testing;

namespace MoviesAPI.Tests.UnitTests
{
    [TestClass]
    public class WireTransferValidatorTests
    {
        [TestMethod]
        public void ValidatorReturnsErrorWithInsufficientFunds()
        {
            //Preparation
            Account origin = new Account() {Funds = 0};
            Account destination = new Account() {Funds = 0};
            decimal amountToTransfer = 5m;

            //Testing
            var service = new WireTransferValidator();
            var result = service.Validate(origin, destination, amountToTransfer);

            //Verification
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("The origin account does not have enough funds available", result.ErrorMessage);
        }

        [TestMethod]
        public void ValidatorReturnsSuccessfulOperationWithSufficientFunds()
        {
            //Preparation
            Account origin = new Account() {Funds = 7};
            Account destination = new Account() {Funds = 0};
            decimal amountToTransfer = 5m;

            //Testing
            var service = new WireTransferValidator();
            var result = service.Validate(origin, destination, amountToTransfer);

            //Verification
            Assert.IsTrue(result.IsSuccessful);
        }
    }
}