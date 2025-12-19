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
					Type = "zoom",
					CreatedAt = zoomMeeting.CreatedAt,
					Title = zoomMeeting.Title,
					Description = zoomMeeting.Description,
					Url = zoomMeeting.Url,
					InstructorId = zoomMeeting.InstructorId,
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
					Type = "assignment",
					CreatedAt = assignment.CreatedAt,
					Title = assignment.Title,
					Description = assignment.Description,
					DueDate = assignment.DueDate,
					MinGrade = assignment.MinGrade,
					MaxGrade = assignment.MaxGrade,
					FileUrl = assignment.FilePath // TODO: resolve to download URL if needed
				};
			}

			// Fallback for unknown content types
			throw new InvalidOperationException($"Unknown content type: {content.GetType().Name}");
		}
	}
}
