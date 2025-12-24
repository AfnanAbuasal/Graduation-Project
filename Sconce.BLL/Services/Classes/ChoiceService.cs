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

        public async Task<(int NumberOfEntries, Response Response)> CreateAsync(ChoiceRequest request)
        {
            // Ensure question exists and is MultipleChoice
            var mcq = await _questionRepository.GetMultipleChoiceByIdAsync(request.QuestionId);
            if (mcq == null)
                return (0, new ErrorResponse { Errors = [$"Multiple choice question with Id: {request.QuestionId} not found."] });

            // Enforce uniqueness constraints
            if (await _choiceRepository.ExistsAsync(request.QuestionId, request.Text))
                return (0, new ErrorResponse { Errors = ["A choice with the same text already exists for this question."] });

            if (await _choiceRepository.ExistsBySortOrderAsync(request.QuestionId, request.SortOrder))
                return (0, new ErrorResponse { Errors = ["A choice with the same sort order already exists for this question."] });

            // Enforce "no duplicates" of correct when multiple selections are not allowed
            if (!mcq.AllowMultipleSelections && request.IsCorrect)
            {
                var existingChoices = await _choiceRepository.GetByQuestionIdAsync(request.QuestionId);
                var correctCount = existingChoices.Count(c => c.IsCorrect);
                if (correctCount > 0)
                    return (0, new ErrorResponse { Errors = ["Only one correct choice is allowed for this question."] });
            }

            var entity = request.Adapt<Choice>();
            var rows = await _choiceRepository.AddAsync(entity);
            
            // Update the question's UpdatedAt timestamp
            mcq.UpdatedAt = DateTime.UtcNow;
            await _questionRepository.UpdateAsync(mcq);

            return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) created successfully." });
        }

        public async Task<(int NumberOfEntries, Response Response)> UpdateAsync(int questionId, string text, ChoiceRequest request)
        {
            var existing = await _choiceRepository.GetByIdAsync(questionId, text);
            if (existing == null)
                return (0, new ErrorResponse { Errors = ["Choice not found."] });

            // Do not allow changing the composite key via this method
            if (request.QuestionId != questionId || !string.Equals(request.Text, text))
                return (0, new ErrorResponse { Errors = ["Changing QuestionId or Text is not supported. Delete and recreate the choice instead."] });

            // Validate sort order uniqueness if changed
            if (existing.SortOrder != request.SortOrder)
            {
                var sortExists = await _choiceRepository.ExistsBySortOrderAsync(questionId, request.SortOrder);
                if (sortExists)
                    return (0, new ErrorResponse { Errors = ["Another choice already uses the requested sort order."] });
            }

            // Enforce single-correct rules when multiple selections are not allowed
            var mcq = await _questionRepository.GetMultipleChoiceByIdAsync(questionId);
            if (mcq != null && !mcq.AllowMultipleSelections)
            {
                if (request.IsCorrect)
                {
                    // If setting this choice to correct, automatically unset all other correct choices
                    var allChoices = await _choiceRepository.GetByQuestionIdAsync(questionId);
                    foreach (var choice in allChoices.Where(c => !string.Equals(c.Text, text) && c.IsCorrect))
                    {
                        choice.IsCorrect = false;
                        await _choiceRepository.UpdateAsync(choice);
                    }
                }
            }

            // Update fields
            existing.IsCorrect = request.IsCorrect;
            existing.SortOrder = request.SortOrder;

            var rows = await _choiceRepository.UpdateAsync(existing);
            
            // Update the question's UpdatedAt timestamp
            mcq.UpdatedAt = DateTime.UtcNow;
            await _questionRepository.UpdateAsync(mcq);

            return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) updated successfully." });
        }

        public async Task<(int NumberOfEntries, Response Response)> DeleteAsync(int questionId, string text)
        {
            var existing = await _choiceRepository.GetByIdAsync(questionId, text);
            if (existing == null)
                return (0, new ErrorResponse { Errors = ["Choice not found."] });

            var rows = await _choiceRepository.DeleteAsync(existing);
            
            // Update the question's UpdatedAt timestamp
            var mcq = await _questionRepository.GetMultipleChoiceByIdAsync(questionId);
            if (mcq != null)
            {
                mcq.UpdatedAt = DateTime.UtcNow;
                await _questionRepository.UpdateAsync(mcq);
            }

            return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) deleted successfully." });
        }

        public async Task<(bool Success, Response Response)> GetByIdAsync(int questionId, string text)
        {
            var entity = await _choiceRepository.GetByIdAsync(questionId, text);
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