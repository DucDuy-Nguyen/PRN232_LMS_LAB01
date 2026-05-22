namespace PRN232.LMS.API.Models.Requests;

public class SemesterRequest
{
    public string SemesterName { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
