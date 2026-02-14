using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Data
{
    public class DataContextDapper
    {
        private readonly IConfiguration _config;
        public DataContextDapper(IConfiguration config)
        {
            _config = config;
        }

        public IEnumerable<T> LoadData<T>(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Query<T>(sql);
        }
        public T LoadDataSingle<T>(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.QuerySingle<T>(sql);
        }

        public bool ExecuteSql(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var result = dbConnection.Execute(sql);
            return result > 0;
        }

        public int ExecuteSqlWithRowCount(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var result = dbConnection.Execute(sql);
            return result;
        }
        public bool ExecuteSqlWithParameterx(string sql, DynamicParameters Parameters)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var result = dbConnection.Execute(sql,Parameters);
            return result > 0;


            // SqlCommand sqlCommandWithParams = new SqlCommand(sql);
            // foreach (SqlParameter param in Parameters)
            // {
            //     sqlCommandWithParams.Parameters.Add(param); 
            // } 

            // SqlConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            // dbConnection.Open();

            // sqlCommandWithParams.Connection = dbConnection;

            // int rowsAffected = sqlCommandWithParams.ExecuteNonQuery();
            // dbConnection.Close();
            // return rowsAffected > 0;
        }


         public IEnumerable<T> LoadDataWithParameter<T>(string sql, DynamicParameters Parameters)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Query<T>(sql, Parameters);
        }
        public T LoadDataSingleWithParameter<T>(string sql,DynamicParameters Parameters)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.QuerySingle<T>(sql,Parameters);
        }
    }
}   