﻿using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request.Assignment;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Models.Dto.Response.Assignment;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface IAssignment
    {
        Task<DtoAddAssignmentResponse> AddAssignment(DtoAddAssignmentRequest Assignment);

        Task<List<DtoAddAssignmentResponse>> GetAllAssignmentsByCourseID(int CourseID);

        Task<DtoUpdateAssignmentResponse> UpdateAssignmentByID(int AssignmentID, DtoUpdateAssignmentRequest Assignment);

        Task<DtoAddAssignmentResponse> GetAllAssignmentInfoByAssignmentID(int AssignmentID);

        Task DeleteAssignment(int AssignmentID);

        Task<List<DtoGetAllStudentAssignmentsRequest>> GetStudentAssignmentsByCourseId(int courseId, int studentId);

        Task<List<DtoGetAllStudentRquest>> GetAllStudentsSubmissionByAssignmentId(int assignmentId);
    }
}
