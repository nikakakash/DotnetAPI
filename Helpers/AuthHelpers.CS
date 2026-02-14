using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Helpers
{
    public class AuthHelpers
    {

        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _config;
        public AuthHelpers(IConfiguration config)
        {
            _config = config;
            _dapper = new DataContextDapper(config);
        }

        public byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            string passwordSaltplusString = _config.GetSection("ApiSettings:PasswordKey").Value +
                Convert.ToBase64String(passwordSalt);

            byte[] passwordHash = KeyDerivation.Pbkdf2(
                password: password,
                salt: System.Text.Encoding.ASCII.GetBytes(passwordSaltplusString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8
                );
            return passwordHash;
        }



        public string CreateToken(int userId)
        {
            Claim[] claims = new Claim[]
            {
                new Claim("UserId", userId.ToString())

            };

            string? tokenKeyString = _config.GetSection("ApiSettings:TokenKey").Value;

            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    tokenKeyString != null ? tokenKeyString : "")
                );

            SigningCredentials credentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Expires = DateTime.Now.AddDays(1)
            };


            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken token = tokenHandler.CreateToken(descriptor);

            return tokenHandler.WriteToken(token);
        }


        public bool SetPassword(UserForLoginDTO userForSetPassword)
        {

            byte[] passwordSalt = new byte[128 / 8];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(passwordSalt);
            }
            byte[] passwordHash = GetPasswordHash(userForSetPassword.Password, passwordSalt);


            string sqlAddAuth = @"EXEC TutorialAppSchema.spRegistration_Upsert  
                        @Email= @EmailParam, 
                        @PasswordHash = @PasswordHashParam, 
                        @PasswordSalt = @PasswordSaltParam";




            DynamicParameters sqlParameters = new DynamicParameters();

            sqlParameters.Add("@EmailParam", userForSetPassword.Email, DbType.String);
            sqlParameters.Add("@PasswordHashParam", passwordHash, DbType.Binary);
            sqlParameters.Add("@PasswordSaltParam", passwordSalt, DbType.Binary);

            // List<SqlParameter> sqlParameters = new List<SqlParameter>();

            // SqlParameter emailParam = new SqlParameter("@EmailParam", SqlDbType.VarChar);
            // emailParam.Value = userForSetPassword.Email;
            // sqlParameters.Add(emailParam);


            // SqlParameter passwordSaltParam = new SqlParameter("@PasswordSaltParam", SqlDbType.VarBinary);
            // passwordSaltParam.Value = passwordSalt;
            // sqlParameters.Add(passwordSaltParam);

            // SqlParameter passwordHashParam = new SqlParameter("@PasswordHashParam", SqlDbType.VarBinary);
            // passwordHashParam.Value = passwordHash;
            // sqlParameters.Add(passwordHashParam);

            return _dapper.ExecuteSqlWithParameterx(sqlAddAuth, sqlParameters);


        }


        
    }
}