using AutoMapper;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.Models;

namespace PRN232.LMS.API.Mappings;

public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        // Student Mappings
        CreateMap<StudentRequest, StudentDto>();
        CreateMap<StudentDto, StudentResponse>();

        // Course Mappings
        CreateMap<CourseRequest, CourseDto>();
        CreateMap<CourseDto, CourseResponse>();

        // Enrollment Mappings
        CreateMap<EnrollmentRequest, EnrollmentDto>();
        CreateMap<EnrollmentDto, EnrollmentResponse>();

        // Semester Mappings
        CreateMap<SemesterRequest, SemesterDto>();
        CreateMap<SemesterDto, SemesterResponse>();

        // Subject Mappings
        CreateMap<SubjectRequest, SubjectDto>();
        CreateMap<SubjectDto, SubjectResponse>();
    }
}
