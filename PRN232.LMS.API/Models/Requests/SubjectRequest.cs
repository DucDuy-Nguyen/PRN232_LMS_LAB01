namespace PRN232.LMS.API.Models.Requests;

public class SubjectRequest
{
    public string SubjectCode { get; set; } = null!;
    public string SubjectName { get; set; } = null!;
    public int Credit { get; set; }
}
