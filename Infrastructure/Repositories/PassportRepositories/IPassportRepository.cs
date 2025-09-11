using System.Linq.Expressions;
using Domain.Entities;

namespace Infrastructure.Repositories.PassportRepositories;

public interface IPassportRepository
{
    Task<List<Passport>> GetPassportsAsync();
    Task<Passport?> GetByIdAsync(Expression<Func<Passport, bool>>? filter = null);
    Task<int> AddAsync(Passport passport);

    Task<int> UpdateAsync(Passport request);
    Task<int> DeleteAsync(Passport request);
}