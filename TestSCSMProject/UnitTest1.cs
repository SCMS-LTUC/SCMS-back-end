  using Moq;
    using Xunit;
    using Microsoft.AspNetCore.Mvc;
    using YourNamespace.Controllers; // Replace with your actual namespace
    using YourNamespace.Services;
    using YourNamespace.Models;


namespace TestSCSMProject
{

    public class StudentAssignmentsControllerTests
    {
        private readonly Mock<IStudentAssignments> _mockService;
        private readonly StudentAssignmentsController _controller;

        public StudentAssignmentsControllerTests()
        {
            _mockService = new Mock<IStudentAssignments>();
            _controller = new StudentAssignmentsController(_mockService.Object);
        }
    }
}
