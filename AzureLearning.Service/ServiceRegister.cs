using AzureLearning.Service.Account;

namespace AzureLearning.Service
{
    public static class ServiceRegister
    {
        public static Dictionary<Type, Type> GetTypes()
        {
            var serviceDictonary = new Dictionary<Type, Type>
            {
                { typeof(IAccountService), typeof(AccountService) }
            };
            return serviceDictonary;

        }
    }
}
