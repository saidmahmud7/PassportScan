using System.Net;
using Domain.Dtos.DepartmentDto;
using Domain.Dtos.Passport;
using Domain.Entities;
using Domain.Filter;
using Infrastructure.Interface;
using Infrastructure.Repositories.DepartmentRepositories;
using Infrastructure.Response;

namespace Infrastructure.Service;

public class DepartmentService(IDepartmentRepository repository) : IDepartmentService
{
    public async Task<PaginationResponse<List<GetDepartmentsDto>>> GetAllDepartmentsAsync(DepartmentFilter filter)
    {
        var departments = await repository.GetAll(filter);
        var totalRecords = departments.Count();
        var data = departments
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();

        var result = data.Select(d => new GetDepartmentsDto()
        {
            Id = d.Id,
            Name = d.Name,
            Passports = d.Passports?.Select(p => new PassportDto()
            {
                Id = p.Id,
                Data = p.Data,
                FilePath = p.FilePath,
                CreatedAt = p.CreatedAt,
            }).ToList()
        }).ToList();
        return new PaginationResponse<List<GetDepartmentsDto>>(result, totalRecords, filter.PageNumber,
            filter.PageSize);
    }

    public async Task<ApiResponse<GetDepartmentsDto>> GetByIdAsync(int id)
    {
        var department = await repository.GetDepartment(d => d.Id == id);
        if (department == null)
        {
            return new ApiResponse<GetDepartmentsDto>(HttpStatusCode.NotFound, "Department Not Found");
        }

        var result = new GetDepartmentsDto()
        {
            Id = department.Id,
            Name = department.Name,
            Passports = department.Passports?.Select(p => new PassportDto()
            {
                Id = p.Id,
                Data = p.Data,
                FilePath = p.FilePath,
                CreatedAt = p.CreatedAt,
            }).ToList()
        };
        return new ApiResponse<GetDepartmentsDto>(result);
    }

    public async Task<ApiResponse<string>> CreateAsync(AddDepartmentDto request)
    {
        var department = new Department()
        {
            Name = request.Name,
        };
        var result = await repository.CreateDepartment(department);
        return result == 1
            ? new ApiResponse<string>(HttpStatusCode.OK, "Success")
            : new ApiResponse<string>(HttpStatusCode.BadRequest, "Failed");
    }

    public async Task<ApiResponse<string>> UpdateAsync(int id, UpdateDepartmentDto request)
    {
        var department = await repository.GetDepartment(d => d.Id == id);
        if (department == null)
        {
            return new ApiResponse<string>(HttpStatusCode.NotFound, "Department Not Found");
        }

        department.Name = request.Name;
        var result = await repository.UpdateDepartment(department);

        return result == 1
            ? new ApiResponse<string>(HttpStatusCode.OK, "Success")
            : new ApiResponse<string>(HttpStatusCode.BadRequest, "Failed");
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        var department = await repository.GetDepartment(d => d.Id == id);
        if (department == null)
        {
            return new ApiResponse<string>(HttpStatusCode.NotFound, "Department Not Found");
        }
        var result = await repository.DeleteDepartment(department);
        return result == 1
            ? new ApiResponse<string>(HttpStatusCode.OK, "Success")
            : new ApiResponse<string>(HttpStatusCode.BadRequest, "Failed");
    }
}