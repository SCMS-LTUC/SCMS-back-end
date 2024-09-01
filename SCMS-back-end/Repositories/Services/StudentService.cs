using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Repositories.Interfaces;

namespace SCMS_back_end.Repositories.Services
{
    public class StudentService : IStudent
    {
        private readonly StudyCenterDbContext _context;

        public StudentService(StudyCenterDbContext context)
        {
            _context = context;
        }

        public void AddStudent(StudentDtoRequest studentDto)
        {
            if (_context.Students == null)
            {
                throw new InvalidOperationException("Entity set 'StudyCenterDbContext.Students' is null.");
            }

            // Validate the studentDto object
            if (studentDto == null)
            {
                throw new ArgumentNullException(nameof(studentDto), "Student data is required.");
            }

            if (string.IsNullOrWhiteSpace(studentDto.UserId))
            {
                throw new ArgumentException("UserId is required and cannot be empty or whitespace.");
            }

            if (string.IsNullOrWhiteSpace(studentDto.FullName))
            {
                throw new ArgumentException("FullName is required and cannot be empty or whitespace.");
            }

            if (studentDto.Level <= 0)
            {
                throw new ArgumentException("Level must be a positive integer.");
            }

            if (!string.IsNullOrWhiteSpace(studentDto.PhoneNumber) && studentDto.PhoneNumber.Length > 255)
            {
                throw new ArgumentException("PhoneNumber cannot exceed 255 characters.");
            }

            // Map StudentDtoRequest to Student entity
            var student = new Student
            {
                UserId = studentDto.UserId,
                FullName = studentDto.FullName,
                Level = studentDto.Level,
                PhoneNumber = studentDto.PhoneNumber
            };

            // Add the student to the context
            _context.Students.Add(student);
            _context.SaveChanges(); // Synchronously save changes to the database
        }


        public void DeleteStudent(int id)
        {
            throw new NotImplementedException();
        }

        public void DropStudentFromCourse(int studentId, int courseId)
        {
            throw new NotImplementedException();
        }

        public void EnrollStudentInCourse(int studentId, int courseId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Student> GetAllStudents()
        {
            throw new NotImplementedException();
        }

        public Student GetStudentById(int id)
        {
            var student = _context.Students
                .Include(s => s.StudentCourses)
                .Include(s => s.StudentAssignments)
                .Include(s => s.LectureAttendances)
                .FirstOrDefault(s => s.StudentId == id);

            if (student == null)
            {
                throw new KeyNotFoundException($"Student with ID {id} not found.");
            }

            return student;
        }


        public IEnumerable<Student> GetStudentsByCourseId(int courseId)
        {
            throw new NotImplementedException();
        }

        public void UpdateStudent(int id, StudentDtoRequest studentDto)
        {
            if (_context.Students == null)
            {
                throw new InvalidOperationException("Entity set 'StudyCenterDbContext.Students' is null.");
            }

            // Validate the studentDto object
            if (studentDto == null)
            {
                throw new ArgumentNullException(nameof(studentDto), "Student data is required.");
            }

            // Retrieve the existing student by ID
            var existingStudent = _context.Students.FirstOrDefault(s => s.StudentId == id);

            if (existingStudent == null)
            {
                throw new KeyNotFoundException($"Student with ID {id} was not found.");
            }

            // Update the student's properties
            existingStudent.UserId = studentDto.UserId;
            existingStudent.FullName = studentDto.FullName;
            existingStudent.Level = studentDto.Level;
            existingStudent.PhoneNumber = studentDto.PhoneNumber;

            // Save the changes to the database
            _context.Students.Update(existingStudent);
            _context.SaveChanges(); // Synchronously save changes to the database

        }

    }
}
