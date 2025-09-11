using Microsoft.AspNetCore.Http;

namespace Domain.Dtos.Passport;

public class PassportUploadDto
{
    public int DepartmentId { get; set; }
    public DateTime CreatedAt { get; set; } 
    public IFormFile File { get; set; } = null!;
    
}