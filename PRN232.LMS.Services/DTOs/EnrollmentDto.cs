using System;

namespace PRN232.LMS.Services.Models;

public class EnrollmentDto
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrollDate { get; set; }
    public string Status { get; set; } = null!;

    public StudentDto? Student { get; set; }
    public CourseDto? Course { get; set; }
}
