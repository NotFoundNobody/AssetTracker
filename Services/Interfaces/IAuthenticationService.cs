using Core.Entities;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<User?> Login(LoginRequest login);
    }
}
