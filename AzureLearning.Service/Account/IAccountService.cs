namespace AzureLearning.Service.Account
{
    public interface IAccountService 
    {
        Task<long> ValidateUserTokenData(long UserId, string jwtToken, DateTime TokenValidDate);
    }
}
