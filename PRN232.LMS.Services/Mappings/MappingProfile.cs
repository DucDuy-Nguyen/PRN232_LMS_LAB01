using AutoMapper;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Services.Models;

namespace PRN232.LMS.Services;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Student, StudentDto>().ReverseMap();
        CreateMap<Subject, SubjectDto>().ReverseMap();
        CreateMap<Semester, SemesterDto>().ReverseMap();
        CreateMap<Course, CourseDto>().ReverseMap();
        CreateMap<Enrollment, EnrollmentDto>().ReverseMap();
    }
}
