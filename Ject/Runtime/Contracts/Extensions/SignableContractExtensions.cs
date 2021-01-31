namespace Ject.Contracts.Extensions
{
    public static class SignableContractExtensions
    {
        public static ISignableContract NewSubContract(this ISignableContract contract)
        {
            ISignableContract newSubContract = new SignableContract();
            contract.AddSubContract(newSubContract);
            return newSubContract;
        }
    }
}