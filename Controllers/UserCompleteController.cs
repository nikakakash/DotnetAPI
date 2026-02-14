using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using DotnetAPI.Data;
using Dapper;
using DotnetAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
namespace DotnetAPI.Controllers;


[Authorize]
[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{
     private readonly DataContextDapper _dapper;
     private readonly ReusableSQL _reusableSQL;
    public UserCompleteController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        _reusableSQL = new ReusableSQL(config);   
    }



    [HttpGet("GetUsers/{userId}/{IsActive}")]
    public IEnumerable<UserComplete> GetUser(int userId,bool IsActive)
    {
        string sql = @"EXEC TutorialAppSchema.spUsers_Get";
        string parameters = "";
        DynamicParameters sqlParameters = new DynamicParameters();


        if(userId != 0)
        {
            parameters += ", @UserId=@UserIdParam ";
            sqlParameters.Add("@UserIdParam", userId, dbType: System.Data.DbType.Int32);
        }
        if(IsActive)
        {
            parameters += ", @Active=@ActiveParam";
            sqlParameters.Add("@ActiveParam", IsActive, dbType: System.Data.DbType.Boolean);
        }

        if(parameters.Length > 0)
        {
            sql += parameters.Substring(1);
        }

        IEnumerable<UserComplete> users= _dapper.LoadDataWithParameter<UserComplete>(sql,sqlParameters);
        return users;
    }

   

    [HttpPut("UpsertUser")]
    public IActionResult EditUser(UserComplete user)
    {

        if(_reusableSQL.UpsertUser(user))
        {
            return Ok();
        }

        throw new Exception("Failed to update user");
    }

   

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"EXEC TutorialAppSchema.spUsers_Delete @UserId = @UserIdParam";

        DynamicParameters sqlParameters = new DynamicParameters();
        sqlParameters.Add("@UserIdParam", userId, dbType: System.Data.DbType.Int32);

        if(_dapper.ExecuteSqlWithParameterx(sql,sqlParameters))
        {
            return Ok();
        }

        throw new Exception("Failed to delete user");
    }
}