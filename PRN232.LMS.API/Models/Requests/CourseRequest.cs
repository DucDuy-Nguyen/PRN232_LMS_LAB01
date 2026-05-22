namespace PRN232.LMS.API.Models.Requests;

public class CourseRequest
{
    public string CourseName { get; set; } = null!;
    public int SemesterId { get; set; }
    public int SubjectId { get; set; }
}
