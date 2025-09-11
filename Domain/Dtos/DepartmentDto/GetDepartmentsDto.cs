using Domain.Dtos.Passport;

namespace Domain.Dtos.DepartmentDto;

public class GetDepartmentsDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<PassportDto>? Passports { get; set; }
}