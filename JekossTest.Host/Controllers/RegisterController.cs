using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jekoss.Interfaces;
using JekossTest.Models;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;

namespace JekossTest.Controllers
{
    [Route("api/register/")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private IAccountService _accountService;

        public RegisterController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public JsonResult Get()
        {
            return new JsonResult("Hellow");            
        }
        
        
        
        [HttpPost]
        public async Task<JsonResult> Post([FromBody]RegisterUserModel model)
        {
            return new JsonResult(await _accountService.RegisterUser(model));
        }
       
    }
}