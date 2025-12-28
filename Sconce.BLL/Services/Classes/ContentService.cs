using Mapster;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models.Enums;
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
		private readonly ISectionRepository _sectionRepository;

		public ContentService(IContentRepository contentRepository, ISectionRepository sectionRepository)
		{
			_contentRepository = contentRepository;
			_sectionRepository = sectionRepository;
		}

		public async Task<Response> GetBySectionIdAsync(int sectionId)
		{
			// Validate Section exists and belongs to the instructor
			var section = await _sectionRepository.GetByIdAsync(sectionId);
			if (section == null)
				return new ErrorResponse { Errors = ["Section not found."] };

			//if (section.InstructorId != instructorId)
			//	return new ErrorResponse { Errors = ["Unauthorized access to this section."] };

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
			if (content is ZoomMeeting)
			{
				return content.Adapt<ZoomMeetingResponse>();
			}
			else if (content is Assignment)
			{
				return content.Adapt<AssignmentResponse>();
			}
			else if (content is Text)
			{
				return content.Adapt<TextResponse>();
			}
			else if (content is Exam)
			{
				return content.Adapt<ExamResponse>();
			}

			// Fallback for unknown content types
			throw new InvalidOperationException($"Unknown content type: {content.GetType().Name}");
		}
	}
}
