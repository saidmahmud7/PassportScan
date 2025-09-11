using System.Linq.Expressions;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories.PassportRepositories;

public class PassportRepository(DataContext context, ILogger<PassportRepository> logger) : IPassportRepository
{
    public async Task<List<Passport>> GetPassportsAsync()
    {
        var query = context.Passports.AsQueryable();
        var passports = await query.ToListAsync();
        return passports;
    }

    public async Task<Passport?> GetByIdAsync(Expression<Func<Passport, bool>>? filter = null)
    {
        var query = context.Passports.AsQueryable();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task<int> AddAsync(Passport passport)
    {
        try
        {
            await context.Passports.AddAsync(passport);
            return await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при добавлении Pdf");
            return 0;
        }
    }

    public async Task<int> UpdateAsync(Passport request)
    {
        try
        {
            context.Update(request);
            return await context.SaveChangesAsync();
        }
        catch(Exception e)
        {
            logger.LogError(e,"Ошибка при обновлении");
            return 0;
        }
    }

    public async Task<int> DeleteAsync(Passport request)
    {
        try
        {
            context.Remove(request);
            return await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при удалении Pdf");
            return 0;
        }
    }
}