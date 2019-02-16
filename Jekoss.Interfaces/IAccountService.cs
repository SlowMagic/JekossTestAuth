using System.Threading.Tasks;
using JekossTest.Models;

namespace Jekoss.Interfaces
{
    public interface IAccountService
    {
        Task<BaseResponse<TokenModel>> Login(LoginModel model);
        Task<BaseResponse<TokenModel>> RegisterUser(RegisterUserModel model);
        Task<BaseResponse<TokenModel>> GetTokenByRefreshToken(TokenModel model);
    }
}