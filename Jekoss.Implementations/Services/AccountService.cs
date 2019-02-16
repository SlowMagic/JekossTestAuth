using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Jekoss.Implementations.Common;
using Jekoss.Implementations.Helpers;
using Jekoss.Interfaces;
using JekossTest.Dal;
using JekossTest.Dal.Entities;
using JekossTest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Jekoss.Implementations.Services
{
    public class AccountService : BaseService<AccountService>, IAccountService
    {
        private const string PasswordError = "Passwords don't match";
        private const string UserExistError = "User with this email exist";
        private const string LoginError = "Invalid login or password";
        private const string Success = "Success";
        private const string RefreshTokenError = "Expiration date is out";
        
        private TestDbContext _context;

        public AccountService(TestDbContext context, IConfigurationRoot root, ILogger<AccountService> logger,
            IMemoryCache memoryCache) : base(
            root, logger, memoryCache)
        {
            _context = context;
        }

        public async Task<BaseResponse<TokenModel>> Login(LoginModel model)
        {
            return await ProcessRequestAsync(async () =>
            {
                var user = await _context.Users.FirstOrDefaultAsync(x =>
                    x.Password == ComputeHash(model.Password, new SHA256CryptoServiceProvider()) &&
                    x.Email == model.Login);
                if (user != null)
                {
                    var tokenModel = await GetTokenModel(user);
                    return new BaseResponse<TokenModel>(true,Success,tokenModel);
                }
                return new BaseResponse<TokenModel>(false,LoginError);
            });
        }
               
        public async Task<BaseResponse<TokenModel>> RegisterUser(RegisterUserModel model)
        {
            return await ProcessRequestAsync(async () =>
            {
                if (model.Password != model.ConfirmPassword)
                {
                    return new BaseResponse<TokenModel>(false, PasswordError);
                }

                var registerUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
                if (registerUser != null)
                {
                    return new BaseResponse<TokenModel>(false, UserExistError);
                }
                
                registerUser = new User()
                {
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Phone = model.Phone,
                    RoleId = 1,
                    Password = ComputeHash(model.Password, new SHA256CryptoServiceProvider())
                };
                await _context.Users.AddAsync(registerUser);
                await _context.SaveChangesAsync();
                var tokenModel = await GetTokenModel(registerUser);
                return new BaseResponse<TokenModel>(true,Success,tokenModel);                            
            });
        }

        public async Task<BaseResponse<TokenModel>> GetTokenByRefreshToken(TokenModel model)
        {
            return await ProcessRequestAsync(async () =>
            {
                var oldClaims =  GetPrincipalFromExpiredToken(model.Token);
                var userId = int.Parse(oldClaims.First(x => x.Type == UserBaseClaims.Id).Value);
                var refreshToken = await _context.AccountRefreshTokens.FirstOrDefaultAsync(x => x.UserId == userId && x.RefreshToken == model.RefreshToken);
                if (refreshToken == null || refreshToken.ExpiredDate < DateTime.Now)
                {
                    return new BaseResponse<TokenModel>(false,RefreshTokenError){ErrorCode = 401};
                }
                refreshToken.Salt = Guid.NewGuid().ToString("N");        //update salt to remove  token collisions             
                refreshToken.GenerateToken();
                _context.AccountRefreshTokens.Update(refreshToken);
                await _context.SaveChangesAsync();
                var jwt = GenerateToken(oldClaims);
                
                model = new TokenModel(new JwtSecurityTokenHandler().WriteToken(jwt), jwt.ValidTo.ToUniversalTime(), refreshToken.RefreshToken);
                
                return new BaseResponse<TokenModel>(true,Success,model);
            });
        }
        
        
        private async Task<TokenModel> GetTokenModel(User user)
        {
            var refreshToken = await _context.AccountRefreshTokens.FirstOrDefaultAsync(x => x.Id == user.Id);
            if (refreshToken != null)
            {
                _context.AccountRefreshTokens.Remove(refreshToken);
                await _context.SaveChangesAsync();
            }

            refreshToken = new AccountRefreshToken()
            {
                Salt = Guid.NewGuid().ToString("N"),
                ExpiredDate = DateTime.UtcNow.AddDays(7),
                PrivateToken = Guid.NewGuid().ToString("N")
            };
            await _context.AccountRefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
            var claims = await GetIdentityClaim(user);
            var encodedJwt = GenerateToken(claims.Claims);
            var token = new JwtSecurityTokenHandler().WriteToken(encodedJwt);
            TokenModel tokenModel = new TokenModel(token, encodedJwt.ValidTo.ToUniversalTime(), refreshToken.RefreshToken);
            return tokenModel;
        }

        private string ComputeHash(string password, HashAlgorithm algorithm)
        {
            Byte[] inputBytes = Encoding.UTF8.GetBytes(password);

            Byte[] hashedBytes = algorithm.ComputeHash(inputBytes);

            return BitConverter.ToString(hashedBytes);
        }

        private JwtSecurityToken GenerateToken(IEnumerable<Claim> claims)
        {
          
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: claims,
                expires: now.Add(TimeSpan.FromDays(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));          
            return jwt;
        }

        private async Task<ClaimsIdentity> GetIdentityClaim(User user)
        {
            var userRole = await _context.Roles.FirstOrDefaultAsync(x => x.Id == user.RoleId);
            if (user != null && userRole != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, userRole.RoleName),
                    new Claim(UserBaseClaims.Id, user.Id.ToString()),
                    new Claim(UserBaseClaims.FirstName, user.FirstName),
                    new Claim(UserBaseClaims.LastName, user.LastName)
                };
                ClaimsIdentity claimsIdentity =
                    new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                        ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
            return null;
        }
        
        private List<Claim> GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = AuthOptions.ISSUER,
                ValidateAudience = true,
                ValidAudience = AuthOptions.AUDIENCE,
                ValidateLifetime = false,
                IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                ValidateIssuerSigningKey = true,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            var claims = principal.Claims.ToList();
            claims.RemoveAll(x => UserBaseClaims.SystemClaimsToDrop.Contains(x.Type));    
            return claims;
        }
    }
}