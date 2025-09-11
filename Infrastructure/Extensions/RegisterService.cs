using Infrastructure.Data;
using Infrastructure.Interface;
using Infrastructure.Repositories.DepartmentRepositories;
using Infrastructure.Repositories.EmployeeRepositories;
using Infrastructure.Repositories.PassportRepositories;
using Infrastructure.Repositories.UserRepositories;
using Infrastructure.Seed;
using Infrastructure.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class RegisterService
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<Seeder>();
        services.AddDbContext<DataContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPassportRepository,PassportRepository>();
        services.AddScoped<IPassportService, PassportService>();
        services.AddScoped<IRoleService, RoleService>();
    }
}