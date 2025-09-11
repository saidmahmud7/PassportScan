using Microsoft.AspNetCore.Http;

namespace Domain.Dtos.Passport;

public class PassportUploadDto
{
    public IFormFile File { get; set; }
}