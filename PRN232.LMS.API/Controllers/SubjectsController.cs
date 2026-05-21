using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Models;
using PRN232.LMS.Services;
using PRN232.LMS.Services.Models;

namespace PRN232.LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubjectsController : ControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectsController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    [HttpGet]
    public async Task<IActionResult> GetSubjects(
        [FromQuery] string? search,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? fields = null)
    {
        var result = await _subjectService.GetSubjectsAsync(search, sort, page, size, fields);
        var pagination = new PaginationMetadata { Page = result.Page, PageSize = result.PageSize, TotalItems = result.TotalItems, TotalPages = result.TotalPages };
        return Ok(ApiResponse<IEnumerable<SubjectDto>>.Ok(result.Data, "Fetched successfully", pagination));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSubject(int id, [FromQuery] string? fields = null)
    {
        var entity = await _subjectService.GetSubjectByIdAsync(id, fields);
        if (entity == null) return NotFound(ApiResponse<object>.Error("Subject not found"));
        return Ok(ApiResponse<SubjectDto>.Ok(entity));
    }

    [HttpPost]
    public async Task<IActionResult> CreateSubject([FromBody] SubjectDto dto)
    {
        var created = await _subjectService.CreateSubjectAsync(dto);
        return CreatedAtAction(nameof(GetSubject), new { id = created.SubjectId }, ApiResponse<SubjectDto>.Ok(created, "Created successfully"));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSubject(int id, [FromBody] SubjectDto dto)
    {
        var success = await _subjectService.UpdateSubjectAsync(id, dto);
        if (!success) return NotFound(ApiResponse<object>.Error("Subject not found"));
        return Ok(ApiResponse<object>.Ok(null, "Updated successfully"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSubject(int id)
    {
        var success = await _subjectService.DeleteSubjectAsync(id);
        if (!success) return NotFound(ApiResponse<object>.Error("Subject not found"));
        return Ok(ApiResponse<object>.Ok(null, "Deleted successfully"));
    }
}
