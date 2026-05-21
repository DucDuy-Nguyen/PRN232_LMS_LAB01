using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Services.Models;
using System.Linq.Dynamic.Core;

namespace PRN232.LMS.Services;

public class SubjectService : ISubjectService
{
    private readonly IGenericRepository<Subject> _repository;
    private readonly IMapper _mapper;

    public SubjectService(IGenericRepository<Subject> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<SubjectDto>> GetSubjectsAsync(string? search, string? sort, int page, int size, string? fields)
    {
        var query = _repository.GetAll();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(e => e.SubjectName.Contains(search) || e.SubjectCode.Contains(search));

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
        else query = query.OrderBy(e => e.SubjectId);

        var totalItems = await query.CountAsync();
        page = page < 1 ? 1 : page;
        size = size < 1 ? 10 : size;
        query = query.Skip((page - 1) * size).Take(size);

        var entities = await query.ToListAsync();
        return new PaginatedResult<SubjectDto>
        {
            Data = _mapper.Map<List<SubjectDto>>(entities),
            Page = page,
            PageSize = size,
            TotalItems = totalItems
        };
    }

    public async Task<SubjectDto?> GetSubjectByIdAsync(int id, string? fields)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return null;
        return _mapper.Map<SubjectDto>(entity);
    }

    public async Task<SubjectDto> CreateSubjectAsync(SubjectDto dto)
    {
        var entity = _mapper.Map<Subject>(dto);
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
        return _mapper.Map<SubjectDto>(entity);
    }

    public async Task<bool> UpdateSubjectAsync(int id, SubjectDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;
        existing.SubjectCode = dto.SubjectCode;
        existing.SubjectName = dto.SubjectName;
        existing.Credit = dto.Credit;
        _repository.Update(existing);
        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteSubjectAsync(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;
        _repository.Delete(existing);
        await _repository.SaveChangesAsync();
        return true;
    }
}
