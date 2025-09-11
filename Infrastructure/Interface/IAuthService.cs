using Domain.Dtos.Auth;
using Infrastructure.Response;

namespace Infrastructure.Interface;

public interface IAuthService
{
    Task<ApiResponse<string>> Register(RegisterDto model);
    Task<ApiResponse<string>> Login(LoginDto login);
    Task<ApiResponse<string>> RemoveRoleFromUser(RoleDto userRole);
    Task<ApiResponse<string>> AddRoleToUser(RoleDto userRole);
}