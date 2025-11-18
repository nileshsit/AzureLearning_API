using AzureLearning.Service.Account;
using AzureLearning.Service.BlobStorage;

namespace AzureLearning.Service
{
    public static class ServiceRegister
    {
        public static Dictionary<Type, Type> GetTypes()
        {
            var serviceDictonary = new Dictionary<Type, Type>
            {
                { typeof(IAccountService), typeof(AccountService) },
                { typeof(IBlobService), typeof(BlobService) },
            };
            return serviceDictonary;

        }
    }
}
