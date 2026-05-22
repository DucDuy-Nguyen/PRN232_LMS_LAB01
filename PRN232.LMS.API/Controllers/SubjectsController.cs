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
public class SubjectsController : ControllerBase
{
    private readonly ISubjectService _subjectService;
    private readonly IMapper _mapper;

    public SubjectsController(ISubjectService subjectService, IMapper mapper)
    {
        _subjectService = subjectService;
        _mapper = mapper;
    }

    /// <summary>
    /// Gets a list of subjects with optional searching, sorting, paging, and data shaping.
    /// </summary>
    /// <param name="search">Keyword to search.</param>
    /// <param name="sort">Sorting fields.</param>
    /// <param name="page">Page number.</param>
    /// <param name="size">Number of items per page.</param>
    /// <param name="fields">Fields to select for data shaping.</param>
    /// <returns>A paginated list of shaped subject data.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ExpandoObject>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubjects(
        [FromQuery] string? search,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? fields = null)
    {
        var result = await _subjectService.GetSubjectsAsync(search, sort, page, size, fields);
        
        var pagination = new PaginationMetadata
        {
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages
        };

        var responseModels = _mapper.Map<IEnumerable<SubjectResponse>>(result.Data);
        var shapedData = responseModels.ShapeData(fields);

        return Ok(ApiResponse<IEnumerable<ExpandoObject>>.Ok(shapedData, "Fetched successfully", pagination));
    }

    /// <summary>
    /// Gets a specific subject by ID.
    /// </summary>
    /// <param name="id">The ID of the subject.</param>
    /// <param name="fields">Fields to select for data shaping.</param>
    /// <returns>The requested subject.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ExpandoObject>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSubject(int id, [FromQuery] string? fields = null)
    {
        var subject = await _subjectService.GetSubjectByIdAsync(id, fields);
        if (subject == null)
            return NotFound(ApiResponse<object>.Error("Subject not found"));

        var responseModel = _mapper.Map<SubjectResponse>(subject);
        var shapedData = responseModel.ShapeData(fields);

        return Ok(ApiResponse<ExpandoObject>.Ok(shapedData));
    }

    /// <summary>
    /// Creates a new subject.
    /// </summary>
    /// <param name="request">The subject creation request model.</param>
    /// <returns>The newly created subject.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateSubject([FromBody] SubjectRequest request)
    {
        var dto = _mapper.Map<SubjectDto>(request);
        var created = await _subjectService.CreateSubjectAsync(dto);
        var responseModel = _mapper.Map<SubjectResponse>(created);
        
        return CreatedAtAction(nameof(GetSubject), new { id = responseModel.SubjectId }, ApiResponse<SubjectResponse>.Ok(responseModel, "Created successfully"));
    }

    /// <summary>
    /// Updates an existing subject.
    /// </summary>
    /// <param name="id">The ID of the subject to update.</param>
    /// <param name="request">The subject update request model.</param>
    /// <returns>Status of the update operation.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSubject(int id, [FromBody] SubjectRequest request)
    {
        var dto = _mapper.Map<SubjectDto>(request);
        var success = await _subjectService.UpdateSubjectAsync(id, dto);
        if (!success)
            return NotFound(ApiResponse<object>.Error("Subject not found"));

        return Ok(ApiResponse<object>.Ok(new {}, "Updated successfully"));
    }

    /// <summary>
    /// Deletes a specific subject.
    /// </summary>
    /// <param name="id">The ID of the subject to delete.</param>
    /// <returns>Status of the delete operation.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSubject(int id)
    {
        var success = await _subjectService.DeleteSubjectAsync(id);
        if (!success)
            return NotFound(ApiResponse<object>.Error("Subject not found"));

        return Ok(ApiResponse<object>.Ok(new {}, "Deleted successfully"));
    }
}
