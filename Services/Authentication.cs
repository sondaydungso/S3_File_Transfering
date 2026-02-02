using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace S3_File_Transfering.Services
{
    public class Authentication
    {
        public Authentication() { }
        public static bool ValidateUser(string username, string password)
        {
            var dbContext = new DatabaseContext();
            var passwordHasher = new PasswordHasher();
            using (var connection = dbContext.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    
                    command.CommandText = "SELECT PasswordHash FROM Users WHERE Username = @username";
                    var usernameParam = command.CreateParameter();
                    usernameParam.ParameterName = "@username";
                    usernameParam.Value = username;
                    command.Parameters.Add(usernameParam);
                    var result = command.ExecuteScalar();
                    connection.Close();
                    
                    if (result == null || result == DBNull.Value)
                    {
                        return false; 
                    }
                    
                    string hashedPassword = result.ToString();
                    
                    return passwordHasher.VerifyPassword(password, hashedPassword);
                }
            }
        }
        public static int GetUserId(string username)
        {
            var dbContext = new DatabaseContext();
            using (var connection = dbContext.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT UserId FROM Users WHERE Username = @username";
                    var usernameParam = command.CreateParameter();
                    usernameParam.ParameterName = "@username";
                    usernameParam.Value = username;
                    command.Parameters.Add(usernameParam);
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }
        public static bool UsernameExists(string username)
        {
            var dbContext = new DatabaseContext();
            using (var connection = dbContext.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = @username";
                    var usernameParam = command.CreateParameter();
                    usernameParam.ParameterName = "@username";
                    usernameParam.Value = username;
                    command.Parameters.Add(usernameParam);
                    var result = command.ExecuteScalar();
                    connection.Close();
                    int count = Convert.ToInt32(result);
                    return count > 0;
                }
            }
        }

        public static bool Register(string username, string password)
        {
            try
            {
                
                if (UsernameExists(username))
                {
                    return false; 
                }

                var dbContext = new DatabaseContext();
                var passwordHasher = new PasswordHasher();
                
                
                string hashedPassword = passwordHasher.HashPassword(password);

                using (var connection = dbContext.CreateConnection())
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO Users (Username, PasswordHash) VALUES (@username, @password)";
                        var usernameParam = command.CreateParameter();
                        usernameParam.ParameterName = "@username";
                        usernameParam.Value = username;
                        command.Parameters.Add(usernameParam);
                        var passwordParam = command.CreateParameter();
                        passwordParam.ParameterName = "@password";
                        passwordParam.Value = hashedPassword;
                        command.Parameters.Add(passwordParam);
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                return true; 
            }
            catch
            {
                return false;
            }
        }
    }
}
