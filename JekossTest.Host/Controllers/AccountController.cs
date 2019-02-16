using System.Threading.Tasks;
using Jekoss.Interfaces;
using JekossTest.Models;
using Microsoft.AspNetCore.Mvc;

namespace JekossTest.Controllers
{
    [ApiController]
    [Route("api/account/")]
    public class AccountController : Controller
    {
        private IAccountService _account;

        public AccountController(IAccountService account)
        {
            _account = account;
        }

        [HttpPost("login")]        
        public async Task<BaseResponse<TokenModel>> Login([FromBody] LoginModel login)
        {
            var tokenModel = await _account.Login(login);
            return tokenModel;
        }

        [HttpPost("register")]
        public async Task<BaseResponse<TokenModel>> Register([FromBody] RegisterUserModel model)
        {
            var tokenModel = await _account.RegisterUser(model);
            return tokenModel;
        }

        [HttpPost("refreshtoken")]
        public async Task<BaseResponse<TokenModel>> UpdateToken([FromBody] TokenModel token)
        {
            var tokenModel = await _account.GetTokenByRefreshToken(token);
            return tokenModel;
        }
    }
}