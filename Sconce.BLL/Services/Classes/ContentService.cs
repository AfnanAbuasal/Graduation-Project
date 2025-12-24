using Mapster;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using Sconce.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Classes
{
	public class ContentService : IContentService
	{
		private readonly IContentRepository _contentRepository;

		public ContentService(IContentRepository contentRepository)
		{
			_contentRepository = contentRepository;
		}

		public async Task<Response> GetBySectionIdAsync(int sectionId)
		{
			var contents = await _contentRepository.GetBySectionIdAsync(sectionId);
			var contentResponses = contents.Select(MapContentToResponse).ToList();

			return new SuccessResponse<List<object>>
			{
				Data = contentResponses
			};
		}

		private object MapContentToResponse(Content content)
		{
			// Map specific content types to their existing response DTOs
			if (content is ZoomMeeting zoomMeeting)
			{
				return new ZoomMeetingResponse
				{
					Id = zoomMeeting.Id,
					SectionId = zoomMeeting.SectionId,
					WeekNumber = zoomMeeting.WeekNumber,
					Type = zoomMeeting.Type,
					CreatedAt = zoomMeeting.CreatedAt,
					Title = zoomMeeting.Title,
					Description = zoomMeeting.Description,
					Url = zoomMeeting.Url,
					ZoomData = zoomMeeting.ZoomData?.Adapt<ZoomDataResponse>()
				};
			}
			else if (content is Assignment assignment)
			{
				return new AssignmentResponse
				{
					Id = assignment.Id,
					SectionId = assignment.SectionId,
					WeekNumber = assignment.WeekNumber,
					Type = assignment.Type,
					CreatedAt = assignment.CreatedAt,
					Title = assignment.Title,
					Description = assignment.Description,
					DueDate = assignment.DueDate,
					MinGrade = assignment.MinGrade,
					MaxGrade = assignment.MaxGrade,
					FileUrl = assignment.FilePath
				};
			}
			else if (content is Exam exam)
			{
				return new ExamResponse
				{
					Id = exam.Id,
					SectionId = exam.SectionId,
					WeekNumber = exam.WeekNumber,
					Type = exam.Type,
					CreatedAt = exam.CreatedAt,
					Title = exam.Title,
					AvailableFrom = exam.AvailableFrom,
					AvailableTo = exam.AvailableTo,
					DurationMinutes = exam.DurationMinutes,
					AttemptsAllowed = exam.AttemptsAllowed,
					ShuffleQuestions = exam.ShuffleQuestions,
					ExamStatus = exam.ExamStatus
				};
			}
			else if (content is Text text)
			{
				return new TextResponse
				{
					Id = text.Id,
					SectionId = text.SectionId,
					WeekNumber = text.WeekNumber,
					Type = text.Type,
					CreatedAt = text.CreatedAt,
					Title = text.Title,
					Body = text.Body
				};
			}

			// Fallback for unknown content types
			throw new InvalidOperationException($"Unknown content type: {content.GetType().Name}");
		}
	}
}
