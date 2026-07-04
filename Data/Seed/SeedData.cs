using AssetTracker.Data;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Seed;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        // --------------------
        // Permissions
        // --------------------
        System.IO.File.WriteAllText(@"C:\Users\a.akbari\Downloads\ssda.txt","Seed Started");


        var permissionNames = new[]
        {
            "Equipment.View",
            "Equipment.Create",
            "Equipment.Edit",
            "Equipment.Delete",
            "Equipment.Assign",
            "Equipment.Return",
            "Equipment.Export",

            "EquipmentType.View",
            "EquipmentType.Create",
            "EquipmentType.Edit",
            "EquipmentType.Delete",

            "EquipmentStatus.View",
            "EquipmentStatus.Create",
            "EquipmentStatus.Edit",
            "EquipmentStatus.Delete",

            "User.View",
            "User.Create",
            "User.Edit",
            "User.Delete",
            "User.ChangePassword",

            "Role.View",
            "Role.Create",
            "Role.Edit",
            "Role.Delete",
            "Role.AssignPermissions",

            "Report.View",
            "Report.Export",

            "System.Settings",
            "System.AuditLog"
        };

        foreach (var permissionName in permissionNames)
        {
            var exists = await context.Permissions
                .AnyAsync(x => x.Name == permissionName);

            if (!exists)
            {
                await context.Permissions.AddAsync(new Permission
                {
                    PermissionID = Guid.CreateVersion7(),
                    Name = permissionName,
                    Description = permissionName
                });
            }
        }

        await context.SaveChangesAsync();

        // --------------------
        // Admin Role
        // --------------------

        var adminRole = await context.Roles
            .FirstOrDefaultAsync(x => x.Name == "Admin");

        if (adminRole == null)
        {
            adminRole = new Role
            {
                RoleID = Guid.CreateVersion7(),
                Name = "Admin",
                Description = "System Administrator"
            };

            await context.Roles.AddAsync(adminRole);
            await context.SaveChangesAsync();
        }

        // --------------------
        // Assign Permissions To Admin
        // --------------------

        var permissions = await context.Permissions.ToListAsync();

        foreach (var permission in permissions)
        {
            var exists = await context.RolePermissions.AnyAsync(x =>
                x.RoleID == adminRole.RoleID &&
                x.PermissionID == permission.PermissionID);

            if (!exists)
            {
                await context.RolePermissions.AddAsync(new RolePermission
                {
                    RoleID = adminRole.RoleID,
                    PermissionID = permission.PermissionID
                });
            }
        }

        await context.SaveChangesAsync();

        // --------------------
        // Admin Person 
        // --------------------

        var adminPerson = await context.Persons
.FirstOrDefaultAsync(x => x.FirstName == "admin");

        if (adminPerson == null)
        {
            adminPerson = new Person
            {
                PersonID = Guid.CreateVersion7(),
                FirstName = "admin",
                LastName= "admin",
                CellPhone ="0",
                NationaleCode="0" ,
                IsDeleted=false
                  
            };

            await context.Persons.AddAsync(adminPerson);
            await context.SaveChangesAsync();
        }
        // --------------------
        // Admin User
        // --------------------

        var adminUser = await context.Users
            .FirstOrDefaultAsync(x => x.Username == "admin");

        if (adminUser == null)
        {
            adminUser = new User
            {
                UserID = Guid.CreateVersion7(),
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                 PersonID=adminPerson.PersonID,
                 Person=adminPerson,
                IsActive = true
            };

            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();
        }

        // --------------------
        // Assign Admin Role To User
        // --------------------

        var userRoleExists = await context.UserRoles.AnyAsync(x =>
            x.UserID == adminUser.UserID &&
            x.RoleID == adminRole.RoleID);

        if (!userRoleExists)
        {
            await context.UserRoles.AddAsync(new UserRole
            {
                UserID = adminUser.UserID,
                RoleID = adminRole.RoleID
            });

            await context.SaveChangesAsync();
        }
    }
}
