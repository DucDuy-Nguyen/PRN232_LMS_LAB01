namespace PRN232.LMS.Services.Models;

public class CourseDto
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = null!;
    public int SemesterId { get; set; }
    public int SubjectId { get; set; }
    
    public SemesterDto? Semester { get; set; }
    public SubjectDto? Subject { get; set; }
}
