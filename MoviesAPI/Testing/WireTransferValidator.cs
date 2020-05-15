namespace MoviesAPI.Testing
{
    public class WireTransferValidator : IValidateWireTransfer
    {
        public OperationResult Validate(Account origin, Account destination, decimal amount)
        {
            if (amount > origin.Funds)
            {
                return new OperationResult(false, "The origin account does not have enough funds available");
            }
            
            //Other Validation
            return new OperationResult(true);
        }
    }
}