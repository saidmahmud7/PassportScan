using Domain.Dtos;
using Domain.Dtos.Passport;
using Infrastructure.Interface;
using Infrastructure.Response;
using Infrastructure.Seed;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controller;

[ApiController]
[Route("api/[controller]")]
public class PassportController(IPassportService service) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<ApiResponse<List<PassportDto>>> GetAll() => await service.GetAllAsync();

    [Authorize]
    [HttpPost]
    public async Task<ApiResponse<string>> UploadPassport([FromForm] PassportUploadDto dto)
    {
        return await service.ProcessPdfAsync(dto);
    }


    [Authorize]
    [HttpGet("{id}")]
    public async Task<ApiResponse<PassportDto>> GetById(int id) => await service.GetPassportAsync(id);

    [HttpDelete]
    [Authorize]
    public async Task<ApiResponse<string>> Delete(int id) => await service.DeletePassportAsync(id);
}