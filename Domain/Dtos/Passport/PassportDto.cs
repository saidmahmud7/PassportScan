namespace Domain.Dtos.Passport;

public class PassportDto
{
    public int Id { get; set; }
    public string Data { get; set; }
    public string? FilePath { get; set; }
    public int DepartmentId { get; set; }
    public DateTime CreatedAt { get; set; } 
}