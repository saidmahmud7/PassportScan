using Domain.Dtos.RoleDto;
using Infrastructure.Interface;
using Infrastructure.Response;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controller;

[ApiController]
[Route("api/[controller]")]
public class RoleController(IRoleService service) : ControllerBase
{
    [HttpGet]
    public async Task<ApiResponse<List<Role>>> GetAllRolesAsync() => await service.GetAllRolesAsync();
}