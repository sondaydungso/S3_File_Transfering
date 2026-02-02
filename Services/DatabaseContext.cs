using S3FileManager.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace S3_File_Transfering.Services
{
    public class DatabaseContext
    {
        private string _connectionString;
        public DatabaseContext()
        {
            Initialize();
        }

        public void Initialize()
        {
            EnvParser.Load(".env");
            string server = Environment.GetEnvironmentVariable("Server");
            string database = Environment.GetEnvironmentVariable("Database");
            string user = Environment.GetEnvironmentVariable("User_ID");
            string password = Environment.GetEnvironmentVariable("Password");
            _connectionString = $"Server={server};Database={database};User Id={user};Password={password};";
        }
        public IDbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
