using PRN232.LMS.Services.Models;

namespace PRN232.LMS.Services;

public interface IStudentService
{
    Task<PaginatedResult<StudentDto>> GetStudentsAsync(string? search, string? sort, int page, int size, string? fields);
    Task<StudentDto?> GetStudentByIdAsync(int id, string? fields);
    Task<StudentDto> CreateStudentAsync(StudentDto studentDto);
    Task<bool> UpdateStudentAsync(int id, StudentDto studentDto);
    Task<bool> DeleteStudentAsync(int id);
}
