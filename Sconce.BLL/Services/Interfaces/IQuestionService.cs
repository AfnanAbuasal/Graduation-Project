using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using Sconce.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Interfaces
{
    public interface IQuestionService : IGenericService<QuestionRequest, QuestionResponse, Question>
    {
        Task<Response> GetAllByCourseIdAsync(int courseId);
        Task<Response> GetAllByInstructorIdAsync(string instructorId);
        Task<(int NumberOfEntries, Response Response)> CreateMultipleChoiceQuestionAsync(MultipleChoiceQuestionRequest request);
        Task<(int NumberOfEntries, Response Response)> CreateEssayQuestionAsync(EssayQuestionRequest request);
        Task<(int NumberOfEntries, Response Response)> UpdateMultipleChoiceQuestionAsync(int id, MultipleChoiceQuestionRequest request);
        Task<(int NumberOfEntries, Response Response)> UpdateEssayQuestionAsync(int id, EssayQuestionRequest request);
        Task<(bool Success, Response Response)> GetMultipleChoiceByIdAsync(int id);
        Task<(bool Success, Response Response)> GetEssayByIdAsync(int id);

        // Additional query methods mirroring repository
        Task<Response> GetAllMultipleChoiceByCourseIdAsync(int courseId);
        Task<Response> GetAllEssayByCourseIdAsync(int courseId);
        Task<Response> GetAllByTypeAsync(QuestionType type);
        Task<Response> GetAllByDifficultyAsync(Difficulty difficulty);
        Task<Response> SearchByPromptAsync(int courseId, string term);
        Task<Response> CountByCourseAsync(int courseId);
        Task<Response> CountByTypeAsync(int courseId, QuestionType type);
    }
}
