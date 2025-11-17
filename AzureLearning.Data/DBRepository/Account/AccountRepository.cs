using AzureLearning.Model.Config;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Data;

namespace AzureLearning.Data.DBRepository.Account
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        #region Fields
        private IConfiguration _config;
        #endregion

        #region Constructor
        public AccountRepository(IConfiguration config, IOptions<DataConfig> dataConfig) : base(dataConfig)
        {
            _config = config;
        }
        #endregion

        public async Task<long> ValidateUserTokenData(long UserId, string jwtToken, DateTime TokenValidDate)
        {
            var param = new DynamicParameters();
            param.Add("@UserId", UserId);
            param.Add("@jwtToken", jwtToken);
            param.Add("@TokenValidDate", TokenValidDate);
            return await QueryFirstOrDefaultAsync<long>("ValidateToken", param, commandType: CommandType.StoredProcedure);
        }
    }
}
