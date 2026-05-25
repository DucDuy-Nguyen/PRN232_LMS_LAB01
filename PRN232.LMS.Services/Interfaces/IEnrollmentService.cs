using PRN232.LMS.Services.Models;

namespace PRN232.LMS.Services;

public interface IEnrollmentService
{
    Task<PaginatedResult<EnrollmentDto>> GetEnrollmentsAsync(string? search, string? sort, int page, int size, string? fields, string? expand);
    Task<IEnumerable<EnrollmentDto>> GetEnrollmentsByCourseIdAsync(int courseId, string? expand);
    Task<EnrollmentDto?> GetEnrollmentByIdAsync(int id, string? fields, string? expand);
    Task<EnrollmentDto> CreateEnrollmentAsync(EnrollmentDto dto);
    Task<bool> UpdateEnrollmentAsync(int id, EnrollmentDto dto);
    Task<bool> DeleteEnrollmentAsync(int id);
}
