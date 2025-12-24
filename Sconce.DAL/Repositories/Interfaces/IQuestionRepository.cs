using Sconce.DAL.Models;
using Sconce.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Repositories.Interfaces
{
    public interface IQuestionRepository : IGenericRepository<Question>
    {
        Task<IEnumerable<Question>> GetAllByCourseIdAsync(int courseId);
        Task<IEnumerable<Question>> GetByCreatedByInstructorIdAsync(string instructorId);
        Task<Question?> GetByIdWithCourseAsync(int id);
        Task<IEnumerable<MultipleChoiceQuestion>> GetAllMultipleChoiceByCourseIdAsync(int courseId);
        Task<IEnumerable<EssayQuestion>> GetAllEssayByCourseIdAsync(int courseId);
        Task<MultipleChoiceQuestion?> GetMultipleChoiceByIdAsync(int id);
        Task<EssayQuestion?> GetEssayByIdAsync(int id);
        Task<IEnumerable<Question>> GetAllByTypeAsync(QuestionType type);
        Task<IEnumerable<Question>> GetAllByDifficultyAsync(Difficulty difficulty);
        Task<IEnumerable<Question>> SearchByPromptAsync(int courseId, string term);
        Task<int> CountByCourseAsync(int courseId);
        Task<int> CountByTypeAsync(int courseId, QuestionType type);
    }
}
