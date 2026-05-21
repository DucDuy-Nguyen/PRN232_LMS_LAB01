using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Models;
using PRN232.LMS.Services;
using PRN232.LMS.Services.Models;

namespace PRN232.LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SemestersController : ControllerBase
{
    private readonly ISemesterService _semesterService;

    public SemestersController(ISemesterService semesterService)
    {
        _semesterService = semesterService;
    }

    [HttpGet]
    public async Task<IActionResult> GetSemesters(
        [FromQuery] string? search,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? fields = null)
    {
        var result = await _semesterService.GetSemestersAsync(search, sort, page, size, fields);
        var pagination = new PaginationMetadata { Page = result.Page, PageSize = result.PageSize, TotalItems = result.TotalItems, TotalPages = result.TotalPages };
        return Ok(ApiResponse<IEnumerable<SemesterDto>>.Ok(result.Data, "Fetched successfully", pagination));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSemester(int id, [FromQuery] string? fields = null)
    {
        var entity = await _semesterService.GetSemesterByIdAsync(id, fields);
        if (entity == null) return NotFound(ApiResponse<object>.Error("Semester not found"));
        return Ok(ApiResponse<SemesterDto>.Ok(entity));
    }

    [HttpPost]
    public async Task<IActionResult> CreateSemester([FromBody] SemesterDto dto)
    {
        var created = await _semesterService.CreateSemesterAsync(dto);
        return CreatedAtAction(nameof(GetSemester), new { id = created.SemesterId }, ApiResponse<SemesterDto>.Ok(created, "Created successfully"));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSemester(int id, [FromBody] SemesterDto dto)
    {
        var success = await _semesterService.UpdateSemesterAsync(id, dto);
        if (!success) return NotFound(ApiResponse<object>.Error("Semester not found"));
        return Ok(ApiResponse<object>.Ok(null, "Updated successfully"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSemester(int id)
    {
        var success = await _semesterService.DeleteSemesterAsync(id);
        if (!success) return NotFound(ApiResponse<object>.Error("Semester not found"));
        return Ok(ApiResponse<object>.Ok(null, "Deleted successfully"));
    }
}
