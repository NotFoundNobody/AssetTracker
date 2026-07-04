using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class LoginRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
