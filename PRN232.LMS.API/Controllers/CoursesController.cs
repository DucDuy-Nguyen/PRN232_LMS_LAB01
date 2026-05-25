using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Helpers;
using PRN232.LMS.API.Models;
using PRN232.LMS.API.Models.Common;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services;
using PRN232.LMS.Services.Models;
using System.Dynamic;

namespace PRN232.LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly IEnrollmentService _enrollmentService;
    private readonly IMapper _mapper;

    public CoursesController(ICourseService courseService, IEnrollmentService enrollmentService, IMapper mapper)
    {
        _courseService = courseService;
        _enrollmentService = enrollmentService;
        _mapper = mapper;
    }

    /// <summary>
    /// Gets a list of courses with optional searching, sorting, paging, and data shaping.
    /// </summary>
    /// <param name="search">Keyword to search.</param>
    /// <param name="sort">Sorting fields.</param>
    /// <param name="page">Page number.</param>
    /// <param name="size">Number of items per page.</param>
    /// <param name="fields">Fields to select for data shaping.</param>
    /// <param name="expand">Related entities to include.</param>
    /// <returns>A paginated list of shaped course data.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ExpandoObject>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourses(
        [FromQuery] string? search,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? fields = null,
        [FromQuery] string? expand = null)
    {
        var result = await _courseService.GetCoursesAsync(search, sort, page, size, fields, expand);
        
        var pagination = new PaginationMetadata
        {
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages
        };

        var responseModels = _mapper.Map<IEnumerable<CourseResponse>>(result.Data);
        var shapedData = responseModels.ShapeData(fields);

        return Ok(ApiResponse<IEnumerable<ExpandoObject>>.Ok(shapedData, "Fetched successfully", pagination));
    }

    /// <summary>
    /// Gets a specific course by ID.
    /// </summary>
    /// <param name="id">The ID of the course.</param>
    /// <param name="fields">Fields to select for data shaping.</param>
    /// <param name="expand">Related entities to include.</param>
    /// <returns>The requested course.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ExpandoObject>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourse(int id, [FromQuery] string? fields = null, [FromQuery] string? expand = null)
    {
        var course = await _courseService.GetCourseByIdAsync(id, fields, expand);
        if (course == null)
            return NotFound(ApiResponse<object>.Error("Course not found"));

        var responseModel = _mapper.Map<CourseResponse>(course);
        var shapedData = responseModel.ShapeData(fields);

        return Ok(ApiResponse<ExpandoObject>.Ok(shapedData));
    }

    /// <summary>
    /// Gets a list of enrollments for a specific course with optional expansion.
    /// </summary>
    /// <param name="id">The course ID.</param>
    /// <param name="expand">Related entities to include (e.g., student, course).</param>
    /// <returns>A list of enrollment data for the course.</returns>
    [HttpGet("{id}/enrollments")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<EnrollmentResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEnrollmentsForCourse(int id, [FromQuery] string? expand = null)
    {
        var course = await _courseService.GetCourseByIdAsync(id, null, null);
        if (course == null)
            return NotFound(ApiResponse<object>.Error("Course not found"));

        var result = await _enrollmentService.GetEnrollmentsByCourseIdAsync(id, expand);
        var responseModels = _mapper.Map<IEnumerable<EnrollmentResponse>>(result);

        return Ok(ApiResponse<IEnumerable<EnrollmentResponse>>.Ok(responseModels, "Enrollments retrieved successfully."));
    }

    /// <summary>
    /// Creates a new course.
    /// </summary>
    /// <param name="request">The course creation request model.</param>
    /// <returns>The newly created course.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateCourse([FromBody] CourseRequest request)
    {
        var dto = _mapper.Map<CourseDto>(request);
        var created = await _courseService.CreateCourseAsync(dto);
        var responseModel = _mapper.Map<CourseResponse>(created);
        
        return CreatedAtAction(nameof(GetCourse), new { id = responseModel.CourseId }, ApiResponse<CourseResponse>.Ok(responseModel, "Created successfully"));
    }

    /// <summary>
    /// Updates an existing course.
    /// </summary>
    /// <param name="id">The ID of the course to update.</param>
    /// <param name="request">The course update request model.</param>
    /// <returns>Status of the update operation.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseRequest request)
    {
        var dto = _mapper.Map<CourseDto>(request);
        var success = await _courseService.UpdateCourseAsync(id, dto);
        if (!success)
            return NotFound(ApiResponse<object>.Error("Course not found"));

        return Ok(ApiResponse<object>.Ok(new {}, "Updated successfully"));
    }

    /// <summary>
    /// Deletes a specific course.
    /// </summary>
    /// <param name="id">The ID of the course to delete.</param>
    /// <returns>Status of the delete operation.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        var success = await _courseService.DeleteCourseAsync(id);
        if (!success)
            return NotFound(ApiResponse<object>.Error("Course not found"));

        return Ok(ApiResponse<object>.Ok(new {}, "Deleted successfully"));
    }
}
