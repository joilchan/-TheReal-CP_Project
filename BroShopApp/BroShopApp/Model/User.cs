using System;
using System.Collections.Generic;
using System.Text;

namespace BroShopApp.Model
{
    public class User
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
    }

    // Модель для запроса (чтобы совпадала с логикой "Логин или Email")
    public class LoginRequest
    {
        public string Identifier { get; set; }
        public string Password { get; set; }
    }
}
