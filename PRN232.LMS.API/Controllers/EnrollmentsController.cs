using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Models;
using PRN232.LMS.Services;
using PRN232.LMS.Services.Models;

namespace PRN232.LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetEnrollments(
        [FromQuery] string? search,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? fields = null,
        [FromQuery] string? expand = null)
    {
        var result = await _enrollmentService.GetEnrollmentsAsync(search, sort, page, size, fields, expand);
        var pagination = new PaginationMetadata { Page = result.Page, PageSize = result.PageSize, TotalItems = result.TotalItems, TotalPages = result.TotalPages };
        return Ok(ApiResponse<IEnumerable<EnrollmentDto>>.Ok(result.Data, "Fetched successfully", pagination));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEnrollment(int id, [FromQuery] string? fields = null, [FromQuery] string? expand = null)
    {
        var entity = await _enrollmentService.GetEnrollmentByIdAsync(id, fields, expand);
        if (entity == null) return NotFound(ApiResponse<object>.Error("Enrollment not found"));
        return Ok(ApiResponse<EnrollmentDto>.Ok(entity));
    }

    [HttpPost]
    public async Task<IActionResult> CreateEnrollment([FromBody] EnrollmentDto dto)
    {
        var created = await _enrollmentService.CreateEnrollmentAsync(dto);
        return CreatedAtAction(nameof(GetEnrollment), new { id = created.EnrollmentId }, ApiResponse<EnrollmentDto>.Ok(created, "Created successfully"));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEnrollment(int id, [FromBody] EnrollmentDto dto)
    {
        var success = await _enrollmentService.UpdateEnrollmentAsync(id, dto);
        if (!success) return NotFound(ApiResponse<object>.Error("Enrollment not found"));
        return Ok(ApiResponse<object>.Ok(null, "Updated successfully"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEnrollment(int id)
    {
        var success = await _enrollmentService.DeleteEnrollmentAsync(id);
        if (!success) return NotFound(ApiResponse<object>.Error("Enrollment not found"));
        return Ok(ApiResponse<object>.Ok(null, "Deleted successfully"));
    }
}
