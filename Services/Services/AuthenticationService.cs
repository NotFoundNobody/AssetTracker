using AssetTracker.Data;
using Core.Entities;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;

public class AuthenticationService(AppDbContext context) : IAuthenticationService
{
    public async Task<User?> Login(LoginRequest login)
    {
        if (login == null)
            return null;
        var user = await context.Users
      .FirstOrDefaultAsync(x => x.Username == login.Username);

        if (user is null || !BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
            return null;

        await context.Entry(user)
            .Reference(x => x.Person)
            .LoadAsync();

        return user;
    }
  
}