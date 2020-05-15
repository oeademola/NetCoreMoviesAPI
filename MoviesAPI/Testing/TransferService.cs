using System;

namespace MoviesAPI.Testing 
{
    public class TransferService 
    {
        private readonly IValidateWireTransfer validateWireTransfer;
        public TransferService (IValidateWireTransfer validateWireTransfer) 
        {
            this.validateWireTransfer = validateWireTransfer;

        }


        public void WireTransfer (Account origin, Account destination, decimal amount) 
        {
            var state = validateWireTransfer.Validate(origin, destination, amount);

            if (!state.IsSuccessful) 
            {
                throw new ApplicationException (state.ErrorMessage);
            }

            origin.Funds -= amount;
            destination.Funds += amount;
        }
    }
}