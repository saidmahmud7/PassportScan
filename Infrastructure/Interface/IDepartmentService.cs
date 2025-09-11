using Domain.Dtos.DepartmentDto;
using Domain.Filter;
using Infrastructure.Response;

namespace Infrastructure.Interface;

public interface IDepartmentService
{
    Task<PaginationResponse<List<GetDepartmentsDto>>> GetAllDepartmentsAsync(DepartmentFilter filter);
    Task<ApiResponse<GetDepartmentsDto>> GetByIdAsync(int id);
    Task<ApiResponse<string>> CreateAsync(AddDepartmentDto request);
    Task<ApiResponse<string>> UpdateAsync(int id,UpdateDepartmentDto request);
    Task<ApiResponse<string>> DeleteAsync(int id);
}