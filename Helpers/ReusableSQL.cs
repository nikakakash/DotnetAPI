using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Models;

namespace DotnetAPI.Helpers
{
    public class ReusableSQL
    {
        private readonly DataContextDapper _dapper;
        public ReusableSQL(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        public bool UpsertUser(UserComplete user)
        {
            string sql = @"EXEC TutorialAppSchema.spUser_Upsert
            @FirstName = @FirstNameParam,
            @LastName = @LastNameParam,
            @Email = @EmailParam,
            @Gender = @GenderParam,
            @JobTitle = @JobTitleParam,
            @Department = @DepartmentParam,
            @Salary = @SalaryParam,
            @Active = @ActiveParam"; 
            
            DynamicParameters sqlParameters = new DynamicParameters();

            sqlParameters.Add("@FirstNameParam", user.FirstName, dbType: System.Data.DbType.String);
            sqlParameters.Add("@LastNameParam", user.LastName, dbType: System.Data.DbType.String);
            sqlParameters.Add("@EmailParam", user.Email, dbType: System.Data.DbType.String);
            sqlParameters.Add("@GenderParam", user.Gender, dbType: System.Data.DbType.String);
            sqlParameters.Add("@JobTitleParam", user.JobTitle, dbType: System.Data.DbType.String);
            sqlParameters.Add("@DepartmentParam", user.Department, dbType: System.Data.DbType.String);
            sqlParameters.Add("@SalaryParam", user.Salary, dbType: System.Data.DbType.Decimal);
            sqlParameters.Add("@ActiveParam", user.Active, dbType: System.Data.DbType.Boolean);
            sqlParameters.Add("@UserIdParam", user.UserId, dbType: System.Data.DbType.Int32);

            return _dapper.ExecuteSqlWithParameterx(sql,sqlParameters);

        }
    }
}