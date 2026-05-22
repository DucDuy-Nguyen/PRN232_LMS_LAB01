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
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;
    private readonly IMapper _mapper;

    public EnrollmentsController(IEnrollmentService enrollmentService, IMapper mapper)
    {
        _enrollmentService = enrollmentService;
        _mapper = mapper;
    }

    /// <summary>
    /// Gets a list of enrollments with optional searching, sorting, paging, data shaping and expansion.
    /// </summary>
    /// <param name="search">Keyword to search.</param>
    /// <param name="sort">Sorting fields.</param>
    /// <param name="page">Page number.</param>
    /// <param name="size">Number of items per page.</param>
    /// <param name="fields">Fields to select for data shaping.</param>
    /// <param name="expand">Related entities to include (e.g., student, course).</param>
    /// <returns>A paginated list of shaped enrollment data.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ExpandoObject>>), StatusCodes.Status200OK)]
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
        
        var responseModels = _mapper.Map<IEnumerable<EnrollmentResponse>>(result.Data);
        var shapedData = responseModels.ShapeData(fields);

        return Ok(ApiResponse<IEnumerable<ExpandoObject>>.Ok(shapedData, "Fetched successfully", pagination));
    }

    /// <summary>
    /// Gets a specific enrollment by ID.
    /// </summary>
    /// <param name="id">The ID of the enrollment.</param>
    /// <param name="fields">Fields to select for data shaping.</param>
    /// <param name="expand">Related entities to include (e.g., student, course).</param>
    /// <returns>The requested enrollment.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ExpandoObject>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEnrollment(int id, [FromQuery] string? fields = null, [FromQuery] string? expand = null)
    {
        var entity = await _enrollmentService.GetEnrollmentByIdAsync(id, fields, expand);
        if (entity == null) return NotFound(ApiResponse<object>.Error("Enrollment not found"));

        var responseModel = _mapper.Map<EnrollmentResponse>(entity);
        var shapedData = responseModel.ShapeData(fields);

        return Ok(ApiResponse<ExpandoObject>.Ok(shapedData));
    }

    /// <summary>
    /// Creates a new enrollment.
    /// </summary>
    /// <param name="request">The enrollment creation request model.</param>
    /// <returns>The newly created enrollment.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateEnrollment([FromBody] EnrollmentRequest request)
    {
        var dto = _mapper.Map<EnrollmentDto>(request);
        var created = await _enrollmentService.CreateEnrollmentAsync(dto);
        var responseModel = _mapper.Map<EnrollmentResponse>(created);

        return CreatedAtAction(nameof(GetEnrollment), new { id = responseModel.EnrollmentId }, ApiResponse<EnrollmentResponse>.Ok(responseModel, "Created successfully"));
    }

    /// <summary>
    /// Updates an existing enrollment.
    /// </summary>
    /// <param name="id">The ID of the enrollment to update.</param>
    /// <param name="request">The enrollment update request model.</param>
    /// <returns>Status of the update operation.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEnrollment(int id, [FromBody] EnrollmentRequest request)
    {
        var dto = _mapper.Map<EnrollmentDto>(request);
        var success = await _enrollmentService.UpdateEnrollmentAsync(id, dto);
        if (!success) return NotFound(ApiResponse<object>.Error("Enrollment not found"));
        return Ok(ApiResponse<object>.Ok(new {}, "Updated successfully"));
    }

    /// <summary>
    /// Deletes a specific enrollment.
    /// </summary>
    /// <param name="id">The ID of the enrollment to delete.</param>
    /// <returns>Status of the delete operation.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEnrollment(int id)
    {
        var success = await _enrollmentService.DeleteEnrollmentAsync(id);
        if (!success) return NotFound(ApiResponse<object>.Error("Enrollment not found"));
        return Ok(ApiResponse<object>.Ok(new {}, "Deleted successfully"));
    }
}
