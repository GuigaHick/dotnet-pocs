using JWTSecurity.Models;
using System.Collections.Generic;
using System.Linq;

namespace JWTSecurity.Repositories
{
    public static class UserRepository
    {
        public static User Get(string username, string password)
        {
            var users = new List<User>();
            users.Add(new User { Id = 1, Name = "batman", Password = "12345", Role = "manager" });
            users.Add(new User { Id = 2, Name = "Robbin", Password = "12345", Role = "employee" });

            return users.Where(user => user.Name.ToLower() == username && user.Password.ToLower() == password).FirstOrDefault();
        }
    }
}
