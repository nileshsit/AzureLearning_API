using AzureLearning.Data.DBRepository.Account;

namespace AzureLearning.Service.Account
{
    public class AccountService : IAccountService
    {
        #region Fields
        private readonly IAccountRepository _repository;
        #endregion

        #region Construtor
        public AccountService(IAccountRepository repository)
        {
            _repository = repository;
        }
        #endregion

        public async Task<long> ValidateUserTokenData(long UserId, string jwtToken, DateTime TokenValidDate)
        {
            return await _repository.ValidateUserTokenData(UserId, jwtToken, TokenValidDate);
        }
    }
}
