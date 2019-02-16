using System.Text;
using Microsoft.IdentityModel.Tokens;


namespace Jekoss.Implementations.Helpers
{
    public class AuthOptions
    {
        public const string ISSUER = "JekossAuthServer";
        public const string AUDIENCE = "http://localhost:5000";
        const string KEY = "mysupersecret_secretkey!123";
        public const int LIFETIME = 60; 
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}