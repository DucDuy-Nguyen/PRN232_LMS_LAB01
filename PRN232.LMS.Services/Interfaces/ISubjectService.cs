using PRN232.LMS.Services.Models;

namespace PRN232.LMS.Services;

public interface ISubjectService
{
    Task<PaginatedResult<SubjectDto>> GetSubjectsAsync(string? search, string? sort, int page, int size, string? fields);
    Task<SubjectDto?> GetSubjectByIdAsync(int id, string? fields);
    Task<SubjectDto> CreateSubjectAsync(SubjectDto dto);
    Task<bool> UpdateSubjectAsync(int id, SubjectDto dto);
    Task<bool> DeleteSubjectAsync(int id);
}
