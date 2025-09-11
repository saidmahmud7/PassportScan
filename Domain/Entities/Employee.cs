namespace Domain.Entities;

public class Employee
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    //navigation
    public string UserId { get; set; }
    public User User { get; set; }
    public int DepartmentId { get; set; }
    public Department Department { get; set; }
}