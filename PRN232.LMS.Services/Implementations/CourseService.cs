using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Services.Models;
using System.Linq.Dynamic.Core;

namespace PRN232.LMS.Services;

public class CourseService : ICourseService
{
    private readonly IGenericRepository<Course> _repository;
    private readonly IMapper _mapper;

    public CourseService(IGenericRepository<Course> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<CourseDto>> GetCoursesAsync(string? search, string? sort, int page, int size, string? fields, string? expand)
    {
        var query = _repository.GetAll();

        // 0. Expand
        if (!string.IsNullOrWhiteSpace(expand))
        {
            var expands = expand.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var inc in expands)
            {
                if (inc.Trim().Equals("semester", StringComparison.OrdinalIgnoreCase)) query = query.Include(e => e.Semester);
                if (inc.Trim().Equals("subject", StringComparison.OrdinalIgnoreCase)) query = query.Include(e => e.Subject);
            }
        }

        // 1. Search
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(e => e.CourseName.Contains(search));
        }

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
            query = query.OrderBy(e => e.CourseId);
        }

        var totalItems = await query.CountAsync();
        page = page < 1 ? 1 : page;
        size = size < 1 ? 10 : size;
        query = query.Skip((page - 1) * size).Take(size);

        var entities = await query.ToListAsync();
        return new PaginatedResult<CourseDto>
        {
            Data = _mapper.Map<List<CourseDto>>(entities),
            Page = page,
            PageSize = size,
            TotalItems = totalItems
        };
    }

    public async Task<CourseDto?> GetCourseByIdAsync(int id, string? fields, string? expand)
    {
        var query = _repository.GetAll();
        if (!string.IsNullOrWhiteSpace(expand))
        {
            var expands = expand.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var inc in expands)
            {
                if (inc.Trim().Equals("semester", StringComparison.OrdinalIgnoreCase)) query = query.Include(e => e.Semester);
                if (inc.Trim().Equals("subject", StringComparison.OrdinalIgnoreCase)) query = query.Include(e => e.Subject);
            }
        }
        var entity = await query.FirstOrDefaultAsync(e => e.CourseId == id);
        if (entity == null) return null;
        return _mapper.Map<CourseDto>(entity);
    }

    public async Task<CourseDto> CreateCourseAsync(CourseDto dto)
    {
        var entity = _mapper.Map<Course>(dto);
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
        return _mapper.Map<CourseDto>(entity);
    }

    public async Task<bool> UpdateCourseAsync(int id, CourseDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;
        existing.CourseName = dto.CourseName;
        existing.SemesterId = dto.SemesterId;
        existing.SubjectId = dto.SubjectId;
        _repository.Update(existing);
        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteCourseAsync(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;
        _repository.Delete(existing);
        await _repository.SaveChangesAsync();
        return true;
    }
}
