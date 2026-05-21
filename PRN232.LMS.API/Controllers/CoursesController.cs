using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Models;
using PRN232.LMS.Services;
using PRN232.LMS.Services.Models;

namespace PRN232.LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCourses(
        [FromQuery] string? search,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? fields = null,
        [FromQuery] string? expand = null)
    {
        var result = await _courseService.GetCoursesAsync(search, sort, page, size, fields, expand);
        var pagination = new PaginationMetadata { Page = result.Page, PageSize = result.PageSize, TotalItems = result.TotalItems, TotalPages = result.TotalPages };
        return Ok(ApiResponse<IEnumerable<CourseDto>>.Ok(result.Data, "Fetched successfully", pagination));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourse(int id, [FromQuery] string? fields = null, [FromQuery] string? expand = null)
    {
        var entity = await _courseService.GetCourseByIdAsync(id, fields, expand);
        if (entity == null) return NotFound(ApiResponse<object>.Error("Course not found"));
        return Ok(ApiResponse<CourseDto>.Ok(entity));
    }

    [HttpPost]
    public async Task<IActionResult> CreateCourse([FromBody] CourseDto dto)
    {
        var created = await _courseService.CreateCourseAsync(dto);
        return CreatedAtAction(nameof(GetCourse), new { id = created.CourseId }, ApiResponse<CourseDto>.Ok(created, "Created successfully"));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseDto dto)
    {
        var success = await _courseService.UpdateCourseAsync(id, dto);
        if (!success) return NotFound(ApiResponse<object>.Error("Course not found"));
        return Ok(ApiResponse<object>.Ok(null, "Updated successfully"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        var success = await _courseService.DeleteCourseAsync(id);
        if (!success) return NotFound(ApiResponse<object>.Error("Course not found"));
        return Ok(ApiResponse<object>.Ok(null, "Deleted successfully"));
    }
}
