using Domain.Dtos;
using Domain.Dtos.Passport;
using Infrastructure.Interface;
using Infrastructure.Response;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controller;
[ApiController]
[Route("api/[controller]")]
public class PassportController(IPassportService service) : ControllerBase
{
    [HttpGet]
    public async Task<ApiResponse<List<PassportDto>>> GetAll() => await service.GetAllAsync();
    
    [HttpPost]
    public async Task<ApiResponse<string>> Create([FromForm] PassportUploadDto request) =>
        await service.ProcessPdfAsync(request.File);

    [HttpGet("{id}")]
    public async Task<ApiResponse<PassportDto>> GetById(int id) => await service.GetPassportAsync(id);
    [HttpDelete]
    public async Task<ApiResponse<string>> Delete(int id) => await service.DeletePassportAsync(id);

}