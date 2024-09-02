using Microsoft.AspNetCore.Mvc;
using Moq;
using SCMS_back_end.Controllers;
using SCMS_back_end.Data;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Repositories.Interfaces;
using Xunit;

namespace SCMS_back_end_Tests
{
    public class StudentControllerTest
    {
        [Fact]
        public async Task GetStudentById_ShouldReturnStudent()
        {
            // Arrange
            var mockStudentService = new Mock<IStudent>();
            var mockContext = new Mock<StudyCenterDbContext>(); // Mock the DbContext
            var studentId = 1;
            var expectedStudent = new StudentDtoResponse
            {
                StudentId = studentId.ToString(),  // Convert to string
                FullName = "John Doe",
                PhoneNumber = "123-456-7890",
                Level = 3 // Assuming level is an int, set it as needed
            };

            mockStudentService.Setup(s => s.GetStudentByIdAsync(studentId)).ReturnsAsync(expectedStudent);

            // Pass the mocked DbContext and service to the controller
            var controller = new StudentsController(mockContext.Object, mockStudentService.Object);

            // Act
            var result = await controller.GetStudent(studentId);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<StudentDtoResponse>(actionResult.Value);
            Assert.Equal(expectedStudent.FullName, returnValue.FullName);
            Assert.Equal(expectedStudent.PhoneNumber, returnValue.PhoneNumber);
            Assert.Equal(expectedStudent.Level, returnValue.Level);
        }
    }
}
