using EventSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace EventSystem.Server.Endpoints;

public static class RoleEndpoints
{
    public static void MapRoleEndpoints(this WebApplication app)
    {
        var userGroup = app.MapGroup("/api/role")
            .RequireAuthorization()
            .WithTags("UserRole Management");

        userGroup.MapGet("/get-all-roles", [Authorize(Roles = "Admin, SuperAdmin")]
        async (IRoleService _roleService) =>
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Results.Ok(roles);
        })
        .WithName("GetAllRoles");
    }
}
