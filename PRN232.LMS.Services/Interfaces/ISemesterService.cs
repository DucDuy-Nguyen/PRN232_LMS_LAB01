using PRN232.LMS.Services.Models;

namespace PRN232.LMS.Services;

public interface ISemesterService
{
    Task<PaginatedResult<SemesterDto>> GetSemestersAsync(string? search, string? sort, int page, int size, string? fields);
    Task<SemesterDto?> GetSemesterByIdAsync(int id, string? fields);
    Task<SemesterDto> CreateSemesterAsync(SemesterDto dto);
    Task<bool> UpdateSemesterAsync(int id, SemesterDto dto);
    Task<bool> DeleteSemesterAsync(int id);
}
