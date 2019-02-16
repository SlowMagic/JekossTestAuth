using System.Net;
using JekossTest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JekossTest.Controllers
{
    [Route("api/profile")]
    
    [ApiController]
    public class UserController : BaseController
    {
        
        public BaseResponse<CurrentUserModel> Get()
        {
            var user = CurrentUser;
            return  new BaseResponse<CurrentUserModel>(true,"",CurrentUser);
        }
        
    }
}