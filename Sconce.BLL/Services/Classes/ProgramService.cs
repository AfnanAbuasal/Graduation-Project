using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using Sconce.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Classes
{
    public class ProgramService : GenericService<ProgramRequest, ProgramResponse, Program>, IProgramService
    {
        private readonly IProgramRepository _programRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProgramService(IProgramRepository programRepository, UserManager<ApplicationUser> userManager) : base(programRepository)
        {
            _programRepository = programRepository;
            _userManager = userManager;
        }

        public async Task<(int NumberOfEntries, Response Response)> IncreasePlannedLevelCountAsync(int programId, IncreasePlannedCountRequest request)
        {
            // Get the program to update
            var program = await _programRepository.GetByIdAsync(programId);
            if (program == null)
                return (0, new ErrorResponse { Errors = ["Program not found."] });

            // Increase the planned level count
            program.PlannedLevelCount += request.Increment;
            program.UpdatedAt = DateTime.UtcNow;

            var rows = await _programRepository.UpdateAsync(program);

            return (rows, new SuccessResponse<string> { Data = $"Planned level count increased to {program.PlannedLevelCount}." });
        }

        public async Task<(bool Success, Response Response)> AssignExamWriterInstructorAsync(int programId, string instructorId)
        {
            // Load program by id
            var program = await _programRepository.GetByIdAsync(programId);
            if (program == null)
                return (false, new ErrorResponse { Errors = ["Program not found."] });

            // Check if program uses proficiency exam
            if (!program.HasProficiencyExam)
                return (false, new ErrorResponse { Errors = ["This program does not use a proficiency exam."] });

            // Validate instructor exists and has Instructor role
            var instructor = await _userManager.FindByIdAsync(instructorId);
            if (instructor == null || !(instructor is Instructor))
                return (false, new ErrorResponse { Errors = ["Instructor not found."] });

            // Set the exam writer instructor
            program.ExamWriterInstructorId = instructorId;
            program.UpdatedAt = DateTime.UtcNow;

            await _programRepository.UpdateAsync(program);

            var response = new SuccessResponse<ProgramResponse>
            {
                Data = program.Adapt<ProgramResponse>()
            };

            return (true, response);
        }

        public async Task<(bool Success, Response Response)> AssignEvaluatorInstructorAsync(int programId, string instructorId)
        {
            // Load program by id
            var program = await _programRepository.GetByIdAsync(programId);
            if (program == null)
                return (false, new ErrorResponse { Errors = ["Program not found."] });

            // Check if program uses proficiency exam
            if (!program.HasProficiencyExam)
                return (false, new ErrorResponse { Errors = ["This program does not use a proficiency exam."] });

            // Validate instructor exists and has Instructor role
            var instructor = await _userManager.FindByIdAsync(instructorId);
            if (instructor == null || !(instructor is Instructor))
                return (false, new ErrorResponse { Errors = ["Instructor not found."] });

            // Set the evaluator instructor
            program.EvaluatorInstructorId = instructorId;
            program.UpdatedAt = DateTime.UtcNow;

            await _programRepository.UpdateAsync(program);

            var response = new SuccessResponse<ProgramResponse>
            {
                Data = program.Adapt<ProgramResponse>()
            };

            return (true, response);
        }
    }
}
