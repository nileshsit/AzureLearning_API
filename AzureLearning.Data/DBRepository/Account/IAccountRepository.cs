namespace AzureLearning.Data.DBRepository.Account
{
    public interface IAccountRepository
    {     
        Task<long> ValidateUserTokenData(long UserId, string jwtToken, DateTime TokenValidDate);
    }
}
