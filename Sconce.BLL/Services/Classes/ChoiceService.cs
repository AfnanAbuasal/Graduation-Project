using Mapster;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using Sconce.DAL.Repositories.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Classes
{
    public class ChoiceService : IChoiceService
    {
        private readonly IChoiceRepository _choiceRepository;
        private readonly IQuestionRepository _questionRepository;

        public ChoiceService(IChoiceRepository choiceRepository, IQuestionRepository questionRepository)
        {
            _choiceRepository = choiceRepository;
            _questionRepository = questionRepository;
        }

        public async Task<(int NumberOfEntries, Response Response)> CreateAsync(int questionId, ChoiceRequest request)
        {
            // Ensure question exists and is MultipleChoice
            var mcq = await _questionRepository.GetMultipleChoiceByIdAsync(questionId);
            if (mcq == null)
                return (0, new ErrorResponse { Errors = [$"Multiple choice question with Id: {questionId} not found."] });

            // Enforce uniqueness constraints
            if (await _choiceRepository.ExistsAsync(questionId, request.Text))
                return (0, new ErrorResponse { Errors = ["A choice with the same text already exists for this question."] });

            // Enforce "no duplicates" of correct when multiple selections are not allowed
            if (!mcq.AllowMultipleSelections && request.IsCorrect)
            {
                var existingChoices = await _choiceRepository.GetByQuestionIdAsync(questionId);
                var correctCount = existingChoices.Count(c => c.IsCorrect);
                if (correctCount > 0)
                    return (0, new ErrorResponse { Errors = ["Only one correct choice is allowed for this question."] });
            }

            var entity = request.Adapt<Choice>();
            entity.QuestionId = questionId;
            var rows = await _choiceRepository.AddAsync(entity);
            
            // Update the question's UpdatedAt timestamp
            mcq.UpdatedAt = DateTime.UtcNow;
            await _questionRepository.UpdateAsync(mcq);

            return (rows, new SuccessResponse<ChoiceResponse> { Data = entity.Adapt<ChoiceResponse>() });
        }

        public async Task<(int NumberOfEntries, Response Response)> UpdateAsync(int id, ChoiceRequest request)
        {
            var existing = await _choiceRepository.GetByIdAsync(id);
            if (existing == null)
                return (0, new ErrorResponse { Errors = ["Choice not found."] });

            // Enforce single-correct rules when multiple selections are not allowed
            var mcq = await _questionRepository.GetMultipleChoiceByIdAsync(existing.QuestionId);
            if (mcq != null && !mcq.AllowMultipleSelections)
            {
                if (request.IsCorrect)
                {
                    // If setting this choice to correct, automatically unset all other correct choices
                    var allChoices = await _choiceRepository.GetByQuestionIdAsync(existing.QuestionId);
                    foreach (var choice in allChoices.Where(c => c.Id != id && c.IsCorrect))
                    {
                        choice.IsCorrect = false;
                        await _choiceRepository.UpdateAsync(choice);
                    }
                }
            }

            // Update fields
            existing.Text = request.Text;
            existing.IsCorrect = request.IsCorrect;

            var rows = await _choiceRepository.UpdateAsync(existing);
            
            // Update the question's UpdatedAt timestamp
            if (mcq != null)
            {
                mcq.UpdatedAt = DateTime.UtcNow;
                await _questionRepository.UpdateAsync(mcq);
            }

            return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) updated successfully." });
        }

        public async Task<(int NumberOfEntries, Response Response)> DeleteAsync(int id)
        {
            var existing = await _choiceRepository.GetByIdAsync(id);
            if (existing == null)
                return (0, new ErrorResponse { Errors = ["Choice not found."] });

            var rows = await _choiceRepository.DeleteAsync(existing);
            
            // Update the question's UpdatedAt timestamp
            var mcq = await _questionRepository.GetMultipleChoiceByIdAsync(existing.QuestionId);
            if (mcq != null)
            {
                mcq.UpdatedAt = DateTime.UtcNow;
                await _questionRepository.UpdateAsync(mcq);
            }

            return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) deleted successfully." });
        }

        public async Task<(bool Success, Response Response)> GetByIdAsync(int id)
        {
            var entity = await _choiceRepository.GetByIdAsync(id);
            if (entity == null)
                return (false, new ErrorResponse { Errors = ["Not Found."] });

            var dto = entity.Adapt<ChoiceResponse>();
            return (true, new SuccessResponse<ChoiceResponse> { Data = dto });
        }

        public async Task<Response> GetByQuestionIdAsync(int questionId)
        {
            var items = await _choiceRepository.GetByQuestionIdAsync(questionId);
            var dtos = items.Select(c => c.Adapt<ChoiceResponse>());
            return new SuccessResponse<IEnumerable<ChoiceResponse>> { Data = dtos };
        }
    }
}