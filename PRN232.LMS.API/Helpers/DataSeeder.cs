using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.API.Helpers;

public static class DataSeeder
{
    public static void Initialize(Prn232LmsLab01Context context)
    {
        // Check if data already exists
        if (context.Semesters.Any() || context.Students.Any() || context.Subjects.Any())
        {
            return; // DB has been seeded
        }

        var random = new Random();

        // 1. Seed 5 Semesters
        var semesters = new List<Semester>();
        for (int i = 1; i <= 5; i++)
        {
            semesters.Add(new Semester
            {
                SemesterName = $"Semester {i}",
                StartDate = DateTime.Now.AddMonths(i * 3 - 6),
                EndDate = DateTime.Now.AddMonths(i * 3 - 3)
            });
        }
        context.Semesters.AddRange(semesters);
        context.SaveChanges();

        // 2. Seed 10 Subjects
        var subjects = new List<Subject>();
        for (int i = 1; i <= 10; i++)
        {
            subjects.Add(new Subject
            {
                SubjectCode = $"SUBJ{i:D3}",
                SubjectName = $"Subject {i}",
                Credit = random.Next(2, 5)
            });
        }
        context.Subjects.AddRange(subjects);
        context.SaveChanges();

        // 3. Seed 20 Courses
        var courses = new List<Course>();
        for (int i = 1; i <= 20; i++)
        {
            courses.Add(new Course
            {
                CourseName = $"Course {i}",
                SemesterId = semesters[random.Next(semesters.Count)].SemesterId,
                SubjectId = subjects[random.Next(subjects.Count)].SubjectId
            });
        }
        context.Courses.AddRange(courses);
        context.SaveChanges();

        // 4. Seed 50 Students
        var students = new List<Student>();
        for (int i = 1; i <= 50; i++)
        {
            students.Add(new Student
            {
                FullName = $"Student {i}",
                Email = $"student{i}@example.com",
                DateOfBirth = DateTime.Now.AddYears(-20).AddDays(-random.Next(1, 1000))
            });
        }
        context.Students.AddRange(students);
        context.SaveChanges();

        // 5. Seed 500 Enrollments
        var enrollments = new List<Enrollment>();
        var statuses = new[] { "Active", "Completed", "Dropped" };
        
        for (int i = 1; i <= 500; i++)
        {
            enrollments.Add(new Enrollment
            {
                StudentId = students[random.Next(students.Count)].StudentId,
                CourseId = courses[random.Next(courses.Count)].CourseId,
                EnrollDate = DateTime.Now.AddDays(-random.Next(1, 100)),
                Status = statuses[random.Next(statuses.Length)]
            });
        }
        context.Enrollments.AddRange(enrollments);
        context.SaveChanges();
    }
}
