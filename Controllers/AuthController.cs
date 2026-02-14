using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly AuthHelpers _authHelpers;
        private readonly IMapper _mapper;   
        private readonly ReusableSQL _reusableSQL;
        public AuthController(IConfiguration config)
        {
            _authHelpers = new AuthHelpers(config);
            _dapper = new DataContextDapper(config);
            _reusableSQL = new ReusableSQL(config);
            _mapper = new Mapper(new MapperConfiguration(cfg => {
                cfg.CreateMap<UserForRegistrationDTO, UserComplete>();
            }));
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDTO userForRegistration)
        {
            if(userForRegistration.Password == userForRegistration.PasswordConfirm)
            {
                string sqlCheckIfUserExists = @"select Email from TutorialAppSchema.Users Where Email = '"
                 + userForRegistration.Email + "'";

                 IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckIfUserExists);
                if(existingUsers.Count() == 0 )
                {
                    UserForLoginDTO userForSetPassword = new UserForLoginDTO()
                    {
                        Email = userForRegistration.Email,
                        Password = userForRegistration.Password
                    };
                    if(_authHelpers.SetPassword(userForSetPassword))
                    {
                        UserComplete userComplete = _mapper.Map<UserComplete>(userForRegistration);
                        userComplete.Active = true;

                        if (_reusableSQL.UpsertUser(userComplete))
                        {
                            return Ok();
                        }
                        throw new Exception("Failed to add user to Users table");
                    }
                    throw new Exception("Failed to register user");
                } 
                
                throw new Exception("User with this email already exists");
            }
            throw new Exception("Passwords do not match");
        }


        [HttpPut("ResetPassword")]
        public IActionResult ResetPassword(UserForLoginDTO userForSetPassword)
        {
            if(_authHelpers.SetPassword(userForSetPassword))
            {
                return Ok();
            }
            throw new Exception("Failed to reset password");
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDTO userForLogin)
        {
            string sqlForHashAndSalt = @"EXEC TutorialAppSchema.spLoginConfirmation_Get @Email = @EmailParam";

            DynamicParameters sqlParameters = new DynamicParameters();
            // SqlParameter emailParam = new SqlParameter("@EmailParam", SqlDbType.VarChar);
            // emailParam.Value = userForLogin.Email;
            // sqlParameters.Add(emailParam);

            sqlParameters.Add("@EmailParam", userForLogin.Email, DbType.String);

            UserForLoginConfirmationDTO userForConfirmation = _dapper.LoadDataSingleWithParameter<UserForLoginConfirmationDTO>(sqlForHashAndSalt,sqlParameters);

            byte[] computedHash = _authHelpers.GetPasswordHash(userForLogin.Password, userForConfirmation.passwordSalt);

            if (!computedHash.SequenceEqual(userForConfirmation.passwordHash))
            {
                return StatusCode(401,"Incorrect password");
            }
            
            string userIdSQL = @"EXEC TutorialAppSchema.spGetUsers_UserId_Email @Email = @EmailParam";
            int userId = _dapper.LoadDataSingleWithParameter<int>(userIdSQL,sqlParameters);


            return Ok(new Dictionary<string, string>
            {
                {"token", _authHelpers.CreateToken(userId)}, 
            });
        }   

        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken()
        {
            string userIdSQL = "EXEC TutorialAppSchema.spGetUsers_UserId_Email  @UserId=@UserIdParam"; 
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParam", User.FindFirst("UserId")?.Value, dbType: System.Data.DbType.Int32);   

            int userIdFromDB = _dapper.LoadDataSingleWithParameter<int>(userIdSQL,sqlParameters);

            return Ok(new Dictionary<string, string>
            {
                {"token", _authHelpers.CreateToken(userIdFromDB)},  
            });
        }

  
        
        
    }   
}