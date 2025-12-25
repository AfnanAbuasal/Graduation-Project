using Mapster;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using Sconce.DAL.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Classes;

public class TextService : GenericService<TextRequest, TextResponse, Text>, ITextService
{
    private readonly ITextRepository _textRepository;
    private readonly ISectionRepository _sectionRepository;

    public TextService(
        ITextRepository textRepository,
        ISectionRepository sectionRepository)
        : base(textRepository)
    {
        _textRepository = textRepository;
        _sectionRepository = sectionRepository;
    }

    public async Task<Response> GetAllBySectionAsync(int sectionId, string instructorId)
    {
        // Verify section exists
        var section = await _sectionRepository.GetByIdAsync(sectionId);
        if (section == null)
            return new ErrorResponse { Errors = ["Section not found."] };

        // Verify section belongs to instructor
        if (section.InstructorId != instructorId)
            return new ErrorResponse { Errors = ["Unauthorized access to this section."] };

        // Get all texts for this section
        var texts = await _textRepository.GetAllBySectionIdAsync(sectionId, withTracking: false);

        return new SuccessResponse<IEnumerable<TextResponse>> { Data = texts.Adapt<IEnumerable<TextResponse>>() };
    }
}
