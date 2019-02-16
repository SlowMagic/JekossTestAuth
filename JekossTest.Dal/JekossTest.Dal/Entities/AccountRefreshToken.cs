using System;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace JekossTest.Dal.Entities
{
    public class AccountRefreshToken
    {
        public int Id { get; set; }
        public string PrivateToken { get; set; }
        public string Salt { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string RefreshToken { get; set; }
        public int UserId { get; set; }

        public virtual void GenerateToken()
        {
            var valuesByte = KeyDerivation.Pbkdf2(PrivateToken, Encoding.UTF8.GetBytes(Salt), KeyDerivationPrf.HMACSHA512, 10000, 252);
            RefreshToken = Convert.ToBase64String(valuesByte);
        }
        
        public virtual User User { get; set; }
    }
}