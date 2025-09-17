using Domain.Dtos.RoleDto;
using Infrastructure.Interface;
using Infrastructure.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Service;

public class RoleService(RoleManager<IdentityRole> roleManager) : IRoleService
{
    public async Task<ApiResponse<List<Role>>> GetAllRolesAsync()
    {
        var roles = await roleManager.Roles.Select(r => new Role
        {
            Id = r.Id,
            Name = r.Name
        }).ToListAsync();
        return new ApiResponse<List<Role>>(roles);
    }

}