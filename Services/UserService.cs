using System.Collections.Generic;
using System.Data;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using RabbitProcessHawk.Models;

namespace RabbitProcessHawk
{
    public class UserService : IUserService
    {
        private readonly string _connectionString;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _passwordHasher = new PasswordHasher<User>();
        }

        public bool ValidateUser(string username, string password)
        {
            using IDbConnection db = new Microsoft.Data.SqlClient.SqlConnection(_connectionString);
            var query = "SELECT PasswordHash FROM Users WHERE UserName = @UserName";
            var hash = db.QuerySingleOrDefault<string>(query, new { UserName = username });

            if (hash == null)
            {
                return false;
            }

            var result = _passwordHasher.VerifyHashedPassword(null, hash, password);
            return result == PasswordVerificationResult.Success;
        }
    }
}
