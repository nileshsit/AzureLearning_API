using AzureLearning.Data.DBRepository.Account;

namespace AzureLearning.Data
{
    public static class DataRegister
    {
        public static Dictionary<Type, Type> GetTypes()
        {
            var dataDictionary = new Dictionary<Type, Type>
            {
                {typeof(IAccountRepository),typeof(AccountRepository) }
            };
            return dataDictionary;
        }
    }
}
