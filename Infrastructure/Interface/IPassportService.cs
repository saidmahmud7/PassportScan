using Domain.Dtos;
using Domain.Dtos.Passport;
using Infrastructure.Response;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Interface;

public interface IPassportService
{
    Task<ApiResponse<List<PassportDto>>> GetAllAsync();
    Task<ApiResponse<PassportDto>> GetPassportAsync(int id);
    Task<ApiResponse<string>> ProcessPdfAsync(IFormFile file,PassportDto dto);
    Task<ApiResponse<string>> UpdatePassportAsync(int id,PassportDto dto);
    Task<ApiResponse<string>> DeletePassportAsync(int id);
}