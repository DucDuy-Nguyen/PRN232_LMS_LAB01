using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Helpers;
using PRN232.LMS.API.Models;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services;
using PRN232.LMS.Services.Models;
using System.Dynamic;

namespace PRN232.LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly IMapper _mapper;

    public StudentsController(IStudentService studentService, IMapper mapper)
    {
        _studentService = studentService;
        _mapper = mapper;
    }

    /// <summary>
    /// Gets a list of students with optional searching, sorting, paging, and data shaping.
    /// </summary>
    /// <param name="search">Keyword to search in FullName or Email.</param>
    /// <param name="sort">Sorting fields (e.g., fullName, -dateOfBirth).</param>
    /// <param name="page">Page number.</param>
    /// <param name="size">Number of items per page.</param>
    /// <param name="fields">Fields to select for data shaping (e.g., studentId,fullName).</param>
    /// <returns>A paginated list of shaped student data.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ExpandoObject>>), StatusCodes.Status200OK)]
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

        var responseModels = _mapper.Map<IEnumerable<StudentResponse>>(result.Data);
        var shapedData = responseModels.ShapeData(fields);

        return Ok(ApiResponse<IEnumerable<ExpandoObject>>.Ok(shapedData, "Fetched successfully", pagination));
    }

    /// <summary>
    /// Gets a specific student by ID.
    /// </summary>
    /// <param name="id">The ID of the student.</param>
    /// <param name="fields">Fields to select for data shaping.</param>
    /// <returns>The requested student.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ExpandoObject>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudent(int id, [FromQuery] string? fields = null)
    {
        var student = await _studentService.GetStudentByIdAsync(id, fields);
        if (student == null)
            return NotFound(ApiResponse<object>.Error("Student not found"));

        var responseModel = _mapper.Map<StudentResponse>(student);
        var shapedData = responseModel.ShapeData(fields);

        return Ok(ApiResponse<ExpandoObject>.Ok(shapedData));
    }

    /// <summary>
    /// Creates a new student.
    /// </summary>
    /// <param name="request">The student creation request model.</param>
    /// <returns>The newly created student.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateStudent([FromBody] StudentRequest request)
    {
        var dto = _mapper.Map<StudentDto>(request);
        var created = await _studentService.CreateStudentAsync(dto);
        var responseModel = _mapper.Map<StudentResponse>(created);
        
        return CreatedAtAction(nameof(GetStudent), new { id = responseModel.StudentId }, ApiResponse<StudentResponse>.Ok(responseModel, "Created successfully"));
    }

    /// <summary>
    /// Updates an existing student.
    /// </summary>
    /// <param name="id">The ID of the student to update.</param>
    /// <param name="request">The student update request model.</param>
    /// <returns>Status of the update operation.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStudent(int id, [FromBody] StudentRequest request)
    {
        var dto = _mapper.Map<StudentDto>(request);
        var success = await _studentService.UpdateStudentAsync(id, dto);
        if (!success)
            return NotFound(ApiResponse<object>.Error("Student not found"));

        return Ok(ApiResponse<object>.Ok(new {}, "Updated successfully"));
    }

    /// <summary>
    /// Deletes a specific student.
    /// </summary>
    /// <param name="id">The ID of the student to delete.</param>
    /// <returns>Status of the delete operation.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        var success = await _studentService.DeleteStudentAsync(id);
        if (!success)
            return NotFound(ApiResponse<object>.Error("Student not found"));

        return Ok(ApiResponse<object>.Ok(new {}, "Deleted successfully"));
    }
}
