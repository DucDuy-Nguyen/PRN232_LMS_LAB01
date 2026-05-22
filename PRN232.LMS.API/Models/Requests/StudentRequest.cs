namespace PRN232.LMS.API.Models.Requests;

public class StudentRequest
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
}
