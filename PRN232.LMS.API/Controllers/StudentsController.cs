using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Models;
using PRN232.LMS.Services;
using PRN232.LMS.Services.Models;

namespace PRN232.LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStudents(
        [FromQuery] string? search,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? fields = null)
    {
        var result = await _studentService.GetStudentsAsync(search, sort, page, size, fields);
        
        var pagination = new PaginationMetadata
        {
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages
        };

        return Ok(ApiResponse<IEnumerable<StudentDto>>.Ok(result.Data, "Fetched successfully", pagination));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetStudent(int id, [FromQuery] string? fields = null)
    {
        var student = await _studentService.GetStudentByIdAsync(id, fields);
        if (student == null)
            return NotFound(ApiResponse<object>.Error("Student not found"));

        return Ok(ApiResponse<StudentDto>.Ok(student));
    }

    [HttpPost]
    public async Task<IActionResult> CreateStudent([FromBody] StudentDto dto)
    {
        var created = await _studentService.CreateStudentAsync(dto);
        return CreatedAtAction(nameof(GetStudent), new { id = created.StudentId }, ApiResponse<StudentDto>.Ok(created, "Created successfully"));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStudent(int id, [FromBody] StudentDto dto)
    {
        var success = await _studentService.UpdateStudentAsync(id, dto);
        if (!success)
            return NotFound(ApiResponse<object>.Error("Student not found"));

        return Ok(ApiResponse<object>.Ok(null, "Updated successfully"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        var success = await _studentService.DeleteStudentAsync(id);
        if (!success)
            return NotFound(ApiResponse<object>.Error("Student not found"));

        return Ok(ApiResponse<object>.Ok(null, "Deleted successfully"));
    }
}
