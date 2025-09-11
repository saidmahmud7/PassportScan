using Domain.Dtos.UserDto;
using Domain.Filter;
using Infrastructure.Response;

namespace Infrastructure.Interface;

public interface IUserService
{
    Task<PaginationResponse<List<GetUserDto>>> GetAllUserAsync(UserFilter filter);
    Task<ApiResponse<string>> UpdateAsync(string id,UpdateUserDto request);
    Task<ApiResponse<string>> DeleteAsync(string id);
}