using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories;
using PRN232.LMS.Services;
using PRN232.LMS.API.Mappings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    // CẬP NHẬT TẠI ĐÂY: Kiểm tra nếu có file XML thì mới map vào Swagger
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configure Dependency Injection
builder.Services.AddDbContext<Prn232LmsLab01Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<ISemesterService, SemesterService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<MappingProfile>();
    cfg.AddProfile<ApiMappingProfile>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || true) // Always enable swagger for this lab
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CẬP NHẬT TẠI ĐÂY: Comment bỏ dòng này để Docker không tự động ép chuyển hướng HTTPS lỗi
// app.UseHttpsRedirection(); 

app.UseAuthorization();

app.MapControllers();
// Chèn đoạn này vào cuối file Program.cs (trước app.Run())
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<Prn232LmsLab01Context>();
    context.Database.EnsureCreated(); // Creates DB in Docker if missing
    PRN232.LMS.API.Helpers.DataSeeder.Initialize(context); // Seeds dummy data
}

app.Run();