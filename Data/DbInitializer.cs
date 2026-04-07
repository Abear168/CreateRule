using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CreateRule.Data;
using CreateRule.Models;

namespace CreateRule.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            await context.Database.MigrateAsync();

            if (!await context.Roles.AnyAsync())
            {
                var roles = new List<ApplicationRole>
                {
                    new ApplicationRole { Name = "SuperAdmin", Description = "超级管理员", CreatedAt = DateTime.Now },
                    new ApplicationRole { Name = "Admin", Description = "管理员", CreatedAt = DateTime.Now },
                    new ApplicationRole { Name = "User", Description = "普通用户", CreatedAt = DateTime.Now }
                };

                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(role);
                }
            }

            if (!await context.Permissions.AnyAsync())
            {
                var permissions = new List<Permission>
                {
                    new Permission { Code = "rule.view", Name = "查看规则", Module = "规则管理", Description = "查看规则列表", CreatedAt = DateTime.Now },
                    new Permission { Code = "rule.generate", Name = "生成规则", Module = "规则管理", Description = "生成新规则", CreatedAt = DateTime.Now },
                    new Permission { Code = "user.view", Name = "查看用户", Module = "用户管理", Description = "查看用户列表", CreatedAt = DateTime.Now },
                    new Permission { Code = "user.create", Name = "创建用户", Module = "用户管理", Description = "创建新用户", CreatedAt = DateTime.Now },
                    new Permission { Code = "user.edit", Name = "编辑用户", Module = "用户管理", Description = "编辑用户信息", CreatedAt = DateTime.Now },
                    new Permission { Code = "user.delete", Name = "删除用户", Module = "用户管理", Description = "删除用户", CreatedAt = DateTime.Now },
                    new Permission { Code = "user.approve", Name = "审核用户", Module = "用户管理", Description = "审核用户注册", CreatedAt = DateTime.Now },
                    new Permission { Code = "role.manage", Name = "管理角色", Module = "角色管理", Description = "管理角色和权限", CreatedAt = DateTime.Now },
                    new Permission { Code = "permission.manage", Name = "管理权限", Module = "权限管理", Description = "管理系统权限", CreatedAt = DateTime.Now }
                };

                context.Permissions.AddRange(permissions);
                await context.SaveChangesAsync();
            }

            if (!await context.MenuItems.AnyAsync())
            {
                var menuItems = new List<MenuItem>
                {
                    new MenuItem { Name = "首页", Icon = "bi-house", Url = "/", Order = 1, CreatedAt = DateTime.Now },
                    new MenuItem { Name = "规则生成", Icon = "bi-lightning", Order = 2, CreatedAt = DateTime.Now },
                    new MenuItem { Name = "规则管理", Icon = "bi-gear", Order = 3, CreatedAt = DateTime.Now },
                    new MenuItem { Name = "系统管理", Icon = "bi-sliders", Order = 4, CreatedAt = DateTime.Now }
                };

                context.MenuItems.AddRange(menuItems);
                await context.SaveChangesAsync();

                var ruleGenerationMenu = menuItems.FirstOrDefault(m => m.Name == "规则生成");
                var ruleManagementMenu = menuItems.FirstOrDefault(m => m.Name == "规则管理");
                var systemManagementMenu = menuItems.FirstOrDefault(m => m.Name == "系统管理");

                var subMenuItems = new List<MenuItem>
                {
                    new MenuItem { Name = "规则导入", Icon = "bi-upload", Url = "/RuleImport", ParentId = ruleGenerationMenu?.Id, Order = 1, PermissionCode = "rule.view", CreatedAt = DateTime.Now },
                    new MenuItem { Name = "内箱规则定义", Icon = "bi-box", Url = "/InnerBoxRules", ParentId = ruleManagementMenu?.Id, Order = 1, PermissionCode = "rule.view", CreatedAt = DateTime.Now },
                    new MenuItem { Name = "用户管理", Icon = "bi-people", Url = "/Admin/Users", ParentId = systemManagementMenu?.Id, Order = 1, PermissionCode = "user.view", CreatedAt = DateTime.Now },
                    new MenuItem { Name = "角色管理", Icon = "bi-shield", Url = "/Admin/Roles", ParentId = systemManagementMenu?.Id, Order = 2, PermissionCode = "role.manage", CreatedAt = DateTime.Now },
                    new MenuItem { Name = "权限管理", Icon = "bi-key", Url = "/Admin/Permissions", ParentId = systemManagementMenu?.Id, Order = 3, PermissionCode = "permission.manage", CreatedAt = DateTime.Now },
                    new MenuItem { Name = "审核管理", Icon = "bi-check-circle", Url = "/Admin/Users/Pending", ParentId = systemManagementMenu?.Id, Order = 4, PermissionCode = "user.approve", CreatedAt = DateTime.Now }
                };

                context.MenuItems.AddRange(subMenuItems);
                await context.SaveChangesAsync();
            }

            if (!await context.Users.AnyAsync())
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    RealName = "系统管理员",
                    Department = "IT部",
                    CreatedAt = DateTime.Now,
                    ApprovalStatus = ApprovalStatus.Approved,
                    ApprovedAt = DateTime.Now,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "SuperAdmin");

                    var superAdminRole = await roleManager.FindByNameAsync("SuperAdmin");
                    var allPermissions = await context.Permissions.ToListAsync();
                    
                    foreach (var permission in allPermissions)
                    {
                        context.RolePermissions.Add(new RolePermission
                        {
                            RoleId = superAdminRole!.Id,
                            PermissionId = permission.Id
                        });
                    }

                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
