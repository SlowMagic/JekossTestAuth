using System.Linq;
using System.Security.Claims;
using JekossTest.Dal.Entities;
using JekossTest.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JekossTest.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BaseController : ControllerBase
    {
        public CurrentUserModel CurrentUser
        {
            get
            {
                if (User.Identity.IsAuthenticated)
                {
                    return new CurrentUserModel
                    {    
                        Email =  User.Claims.First(x=>x.Type == ClaimsIdentity.DefaultNameClaimType).Value,
                        FirstName = User.Claims.First(x => x.Type == UserBaseClaims.FirstName).Value,
                        LastName = User.Claims.First(x => x.Type == UserBaseClaims.LastName).Value,
                        Id = int.Parse(User.Claims.First(x => x.Type == UserBaseClaims.Id).Value),
                        RoleId = int.Parse(User.Claims.First(x=>x.Type == ClaimsIdentity.DefaultRoleClaimType).Value)
                    };
                }
                return null;
            }           
        }
    }
}