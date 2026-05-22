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
public class SemestersController : ControllerBase
{
    private readonly ISemesterService _semesterService;
    private readonly IMapper _mapper;

    public SemestersController(ISemesterService semesterService, IMapper mapper)
    {
        _semesterService = semesterService;
        _mapper = mapper;
    }

    /// <summary>
    /// Gets a list of semesters with optional searching, sorting, paging, and data shaping.
    /// </summary>
    /// <param name="search">Keyword to search.</param>
    /// <param name="sort">Sorting fields.</param>
    /// <param name="page">Page number.</param>
    /// <param name="size">Number of items per page.</param>
    /// <param name="fields">Fields to select for data shaping.</param>
    /// <returns>A paginated list of shaped semester data.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ExpandoObject>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSemesters(
        [FromQuery] string? search,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? fields = null)
    {
        var result = await _semesterService.GetSemestersAsync(search, sort, page, size, fields);
        
        var pagination = new PaginationMetadata
        {
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages
        };

        var responseModels = _mapper.Map<IEnumerable<SemesterResponse>>(result.Data);
        var shapedData = responseModels.ShapeData(fields);

        return Ok(ApiResponse<IEnumerable<ExpandoObject>>.Ok(shapedData, "Fetched successfully", pagination));
    }

    /// <summary>
    /// Gets a specific semester by ID.
    /// </summary>
    /// <param name="id">The ID of the semester.</param>
    /// <param name="fields">Fields to select for data shaping.</param>
    /// <returns>The requested semester.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ExpandoObject>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSemester(int id, [FromQuery] string? fields = null)
    {
        var semester = await _semesterService.GetSemesterByIdAsync(id, fields);
        if (semester == null)
            return NotFound(ApiResponse<object>.Error("Semester not found"));

        var responseModel = _mapper.Map<SemesterResponse>(semester);
        var shapedData = responseModel.ShapeData(fields);

        return Ok(ApiResponse<ExpandoObject>.Ok(shapedData));
    }

    /// <summary>
    /// Creates a new semester.
    /// </summary>
    /// <param name="request">The semester creation request model.</param>
    /// <returns>The newly created semester.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateSemester([FromBody] SemesterRequest request)
    {
        var dto = _mapper.Map<SemesterDto>(request);
        var created = await _semesterService.CreateSemesterAsync(dto);
        var responseModel = _mapper.Map<SemesterResponse>(created);
        
        return CreatedAtAction(nameof(GetSemester), new { id = responseModel.SemesterId }, ApiResponse<SemesterResponse>.Ok(responseModel, "Created successfully"));
    }

    /// <summary>
    /// Updates an existing semester.
    /// </summary>
    /// <param name="id">The ID of the semester to update.</param>
    /// <param name="request">The semester update request model.</param>
    /// <returns>Status of the update operation.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSemester(int id, [FromBody] SemesterRequest request)
    {
        var dto = _mapper.Map<SemesterDto>(request);
        var success = await _semesterService.UpdateSemesterAsync(id, dto);
        if (!success)
            return NotFound(ApiResponse<object>.Error("Semester not found"));

        return Ok(ApiResponse<object>.Ok(new {}, "Updated successfully"));
    }

    /// <summary>
    /// Deletes a specific semester.
    /// </summary>
    /// <param name="id">The ID of the semester to delete.</param>
    /// <returns>Status of the delete operation.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSemester(int id)
    {
        var success = await _semesterService.DeleteSemesterAsync(id);
        if (!success)
            return NotFound(ApiResponse<object>.Error("Semester not found"));

        return Ok(ApiResponse<object>.Ok(new {}, "Deleted successfully"));
    }
}
