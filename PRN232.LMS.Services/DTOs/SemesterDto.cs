using System;

namespace PRN232.LMS.Services.Models;

public class SemesterDto
{
    public int SemesterId { get; set; }
    public string SemesterName { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
