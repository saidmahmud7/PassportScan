using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos.DepartmentDto;

public class UpdateDepartmentDto
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
}