using System;

namespace JekossTest.Models
{
    public class TokenModel
    {
        public TokenModel()
        {

        }
        public TokenModel(string encodedJwt, DateTime expired,string refreshToken)
        {
            Token = encodedJwt;
            Expired = expired;
            RefreshToken = refreshToken;
        }

        public string Token { get; set; }
        public DateTime? Expired { get; set; }
        public string RefreshToken { get; set; }
    }
}