namespace Domain.Entities;


public class Passport
{
    public int Id { get; set; }
    public string Data { get; set; } 
    
    public DateTimeOffset CreatedAt { get; set; } = 
        new DateTimeOffset(DateTime.UtcNow).ToOffset(TimeSpan.FromHours(5));
    public string? FullText { get; set; }
    public string? FilePath { get; set; }
    //navigation
    public int DepartmentId { get; set; }
    public Department Department { get; set; }
}