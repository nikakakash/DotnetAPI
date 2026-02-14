using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        public PostController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config); 
        }


        [HttpGet("Posts/{postId}/{userId}/{searchText}")]
        public IEnumerable<Post> GetPosts(int postId = 0,int userId = 0, string searchText="None")
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Get";
            string parameters = "";
            DynamicParameters sqlParameters = new DynamicParameters();

            if(postId != 0)
            {
                parameters += ", @PostId=@PostIdParam ";
                sqlParameters.Add("@PostIdParam", postId, dbType: System.Data.DbType.Int32);
            }
            if(userId != 0)
            {
                parameters += ", @UserId=@UserIdParam ";
                sqlParameters.Add("@UserIdParam", userId, dbType: System.Data.DbType.Int32);
            }
            if(searchText.ToLower() != "none")
            {
                parameters += ", @SearchValue=@SearchValueParam";
                sqlParameters.Add("@SearchValueParam", searchText, dbType: System.Data.DbType.String);
            }

            if(parameters.Length > 0)
            {
                sql += parameters.Substring(1);
            }

            return _dapper.LoadDataWithParameter<Post>(sql,sqlParameters);
        }

       


      

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPost()
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Get @UserId= @UserIdParam";
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParam", this.User.FindFirst("UserId")?.Value, dbType: System.Data.DbType.Int32);
            IEnumerable<Post> posts= _dapper.LoadDataWithParameter<Post>(sql,sqlParameters);
            return posts;
        }


       



        [HttpPut("UpsertPost")]
        public IActionResult UpsertPost(Post postToUpsert)
        {
            string sql= @"EXEC TutorialAppSchema.spPosts_Upsert @PostTitle=@PostTitleParam,
                    @PostContent=@PostContentParam,
                    @UserId=@UserIdParam" ;

            DynamicParameters sqlParameters = new DynamicParameters();

            sqlParameters.Add("@PostTitleParam", postToUpsert.PostTitle, dbType: System.Data.DbType.String);
            sqlParameters.Add("@PostContentParam", postToUpsert.PostContent, dbType: System.Data.DbType.String);
            sqlParameters.Add("@UserIdParam", this.User.FindFirst("UserId")?.Value, dbType: System.Data.DbType.Int32);
            sqlParameters.Add("@PostIdParam", postToUpsert.PostId, dbType: System.Data.DbType.Int32);

            
            if(postToUpsert.PostId > 0)
            {
                sql += ", @PostId=@PostIdParam";
            }

            if (_dapper.ExecuteSqlWithParameterx(sql,sqlParameters))
            {
                return Ok();
            }
            throw new Exception("Failed to Upsert post");
        }



   


        [HttpDelete("Post/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sql= @"EXEC TutorialAppSchema.spPosts_Delete
                @PostId =@PostIdParam,
                @UserId =@UserIdParam ";
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@PostIdParam", postId, dbType: System.Data.DbType.Int32);
            sqlParameters.Add("@UserIdParam", this.User.FindFirst("UserId")?.Value, dbType: System.Data.DbType.Int32);
            if (_dapper.ExecuteSqlWithParameterx(sql, sqlParameters))
            {
                return Ok();
            }
            throw new Exception("Failed to Delete post");
        }
    }
}