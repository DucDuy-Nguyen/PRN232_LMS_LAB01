using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Services.Models;
using System.Linq.Dynamic.Core;

namespace PRN232.LMS.Services;

public class StudentService : IStudentService
{
    private readonly IGenericRepository<Student> _repository;
    private readonly IMapper _mapper;

    public StudentService(IGenericRepository<Student> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<StudentDto>> GetStudentsAsync(string? search, string? sort, int page, int size, string? fields)
    {
        var query = _repository.GetAll();

        // 1. Search
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(s => s.FullName.Contains(search) || s.Email.Contains(search));
        }

        // 2. Sort (using System.Linq.Dynamic.Core)
        if (!string.IsNullOrWhiteSpace(sort))
        {
            var sortParams = sort.Split(',');
            var sortExpression = "";
            foreach (var param in sortParams)
            {
                var p = param.Trim();
                if (p.StartsWith("-"))
                    sortExpression += p.Substring(1) + " descending, ";
                else
                    sortExpression += p + " ascending, ";
            }
            sortExpression = sortExpression.TrimEnd(',', ' ');
            if (!string.IsNullOrEmpty(sortExpression))
            {
                query = query.OrderBy(sortExpression);
            }
        }
        else
        {
            query = query.OrderBy(s => s.StudentId);
        }

        // 3. Count Total Items
        var totalItems = await query.CountAsync();

        // 4. Paging
        page = page < 1 ? 1 : page;
        size = size < 1 ? 10 : size;
        query = query.Skip((page - 1) * size).Take(size);

        var students = await query.ToListAsync();
        var dtos = _mapper.Map<List<StudentDto>>(students);

        return new PaginatedResult<StudentDto>
        {
            Data = dtos,
            Page = page,
            PageSize = size,
            TotalItems = totalItems
        };
    }

    public async Task<StudentDto?> GetStudentByIdAsync(int id, string? fields)
    {
        var student = await _repository.GetByIdAsync(id);
        if (student == null) return null;
        return _mapper.Map<StudentDto>(student);
    }

    public async Task<StudentDto> CreateStudentAsync(StudentDto studentDto)
    {
        var entity = _mapper.Map<Student>(studentDto);
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
        return _mapper.Map<StudentDto>(entity);
    }

    public async Task<bool> UpdateStudentAsync(int id, StudentDto studentDto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;

        existing.FullName = studentDto.FullName;
        existing.Email = studentDto.Email;
        existing.DateOfBirth = studentDto.DateOfBirth;

        _repository.Update(existing);
        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteStudentAsync(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;

        _repository.Delete(existing);
        await _repository.SaveChangesAsync();
        return true;
    }
}
