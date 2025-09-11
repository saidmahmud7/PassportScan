using Domain.Dtos.RoleDto;
using Infrastructure.Response;

namespace Infrastructure.Interface;


public interface IRoleService
{
    Task<ApiResponse<List<Role>>> GetAllRolesAsync();
}