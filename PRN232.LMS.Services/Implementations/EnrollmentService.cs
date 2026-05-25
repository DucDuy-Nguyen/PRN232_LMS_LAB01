using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Services.Models;
using System.Linq.Dynamic.Core;

namespace PRN232.LMS.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IGenericRepository<Enrollment> _repository;
    private readonly IMapper _mapper;

    public EnrollmentService(IGenericRepository<Enrollment> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    /// <summary>
    /// Applies Include/ThenInclude based on the expand string.
    /// Handles: student, course, semester, subject.
    /// semester and subject are nested under course.
    /// </summary>
    private IQueryable<Enrollment> ApplyExpand(IQueryable<Enrollment> baseQuery, string? expand)
    {
        if (string.IsNullOrWhiteSpace(expand)) return baseQuery;

        var expands = expand.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim().ToLowerInvariant())
                            .ToList();

        var query = baseQuery;

        if (expands.Contains("student"))
            query = query.Include(e => e.Student);

        bool needsCourse = expands.Contains("course") || expands.Contains("semester") || expands.Contains("subject");
        bool needsSemester = expands.Contains("semester");
        bool needsSubject = expands.Contains("subject");

        if (needsCourse)
        {
            if (needsSemester)
                query = query.Include(e => e.Course).ThenInclude(c => c.Semester);
            if (needsSubject)
                query = query.Include(e => e.Course).ThenInclude(c => c.Subject);
            if (!needsSemester && !needsSubject)
                query = query.Include(e => e.Course);
        }

        return query;
    }

    public async Task<PaginatedResult<EnrollmentDto>> GetEnrollmentsAsync(string? search, string? sort, int page, int size, string? fields, string? expand)
    {
        var query = ApplyExpand(_repository.GetAll(), expand);

        // 1. Search
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(e => e.Status.Contains(search));

        // 2. Sort
        if (!string.IsNullOrWhiteSpace(sort))
        {
            var sortParams = sort.Split(',');
            var sortExpression = "";
            foreach (var param in sortParams)
            {
                var p = param.Trim();
                if (p.StartsWith("-")) sortExpression += p.Substring(1) + " descending, ";
                else sortExpression += p + " ascending, ";
            }
            sortExpression = sortExpression.TrimEnd(',', ' ');
            if (!string.IsNullOrEmpty(sortExpression)) query = query.OrderBy(sortExpression);
        }
        else
        {
            query = query.OrderBy(e => e.EnrollmentId);
        }

        var totalItems = await query.CountAsync();
        page = page < 1 ? 1 : page;
        size = size < 1 ? 10 : size;
        query = query.Skip((page - 1) * size).Take(size);

        var entities = await query.ToListAsync();
        return new PaginatedResult<EnrollmentDto>
        {
            Data = _mapper.Map<List<EnrollmentDto>>(entities),
            Page = page,
            PageSize = size,
            TotalItems = totalItems
        };
    }

    public async Task<IEnumerable<EnrollmentDto>> GetEnrollmentsByCourseIdAsync(int courseId, string? expand)
    {
        var query = ApplyExpand(_repository.GetAll().Where(e => e.CourseId == courseId), expand);
        var entities = await query.ToListAsync();
        return _mapper.Map<List<EnrollmentDto>>(entities);
    }

    public async Task<EnrollmentDto?> GetEnrollmentByIdAsync(int id, string? fields, string? expand)
    {
        var query = ApplyExpand(_repository.GetAll(), expand);
        var entity = await query.FirstOrDefaultAsync(e => e.EnrollmentId == id);
        if (entity == null) return null;
        return _mapper.Map<EnrollmentDto>(entity);
    }

    public async Task<EnrollmentDto> CreateEnrollmentAsync(EnrollmentDto dto)
    {
        var entity = _mapper.Map<Enrollment>(dto);
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
        return _mapper.Map<EnrollmentDto>(entity);
    }

    public async Task<bool> UpdateEnrollmentAsync(int id, EnrollmentDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;
        existing.StudentId = dto.StudentId;
        existing.CourseId = dto.CourseId;
        existing.EnrollDate = dto.EnrollDate;
        existing.Status = dto.Status;
        _repository.Update(existing);
        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteEnrollmentAsync(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;
        _repository.Delete(existing);
        await _repository.SaveChangesAsync();
        return true;
    }
}
