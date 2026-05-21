using PRN232.LMS.Services.Models;

namespace PRN232.LMS.Services;

public interface ICourseService
{
    Task<PaginatedResult<CourseDto>> GetCoursesAsync(string? search, string? sort, int page, int size, string? fields, string? expand);
    Task<CourseDto?> GetCourseByIdAsync(int id, string? fields, string? expand);
    Task<CourseDto> CreateCourseAsync(CourseDto dto);
    Task<bool> UpdateCourseAsync(int id, CourseDto dto);
    Task<bool> DeleteCourseAsync(int id);
}
