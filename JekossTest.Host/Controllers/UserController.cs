using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JekossTest.Controllers
{
    [Route("api/profile")]
    
    [ApiController]
    public class UserController : BaseController
    {
        
        public JsonResult Get()
        {
            dynamic MyDynamic = new System.Dynamic.ExpandoObject();
            MyDynamic.A = "A";
            MyDynamic.B = "B";
            MyDynamic.C = "C";
            return  new JsonResult( MyDynamic);
        }
        
    }
}