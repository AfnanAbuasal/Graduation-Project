using Mapster;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
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
    public class DocumentService : FileGenericService<DocumentRequest, DocumentResponse, Document>, IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly ISectionRepository _sectionRepository;
        private readonly IUrlHelper _urlHelper;
        private readonly IFileService _fileService;
        private readonly string _folder;

        public DocumentService(
            IDocumentRepository documentRepository,
            ISectionRepository sectionRepository,
            IFileService fileService,
            IUrlHelper urlHelper)
            : base(documentRepository, fileService, urlHelper, "Uploads/Documents")
        {
            _documentRepository = documentRepository;
            _sectionRepository = sectionRepository;
            _urlHelper = urlHelper;
            _fileService = fileService;
            _folder = "Uploads/Documents";
        }

        public override async Task<(int NumberOfEntries, Response Response)> CreateAsync(DocumentRequest request)
        {
            // Validate section exists
            var section = await _sectionRepository.GetByIdAsync(request.SectionId);
            if (section == null)
                return (0, new ErrorResponse { Errors = ["Section not found."] });

            // Validate file
            if (request.File == null || request.File.Length == 0)
                return (0, new ErrorResponse { Errors = ["File is required."] });

            // Create entity from request
            var document = request.Adapt<Document>();

            // Save file
            document.FilePath = await _fileService.SaveFileAsync(request.File, _folder);

            // Persist entity
            var rows = await _documentRepository.AddAsync(document);

            // Update section timestamp
            await UpdateSectionTimestampAsync(request.SectionId);

            // Map response with FileUrl
            var response = document.Adapt<DocumentResponse>();
            response.FileUrl = _urlHelper.BuildUrl(document.FilePath);

            return (rows, new SuccessResponse<DocumentResponse> { Data = response });
        }

        public override async Task<(int NumberOfEntries, Response Response)> UpdateAsync(int id, DocumentRequest request)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null)
                return (0, new ErrorResponse { Errors = ["Document not found."] });

            // Validate section exists
            var section = await _sectionRepository.GetByIdAsync(request.SectionId);
            if (section == null)
                return (0, new ErrorResponse { Errors = ["Section not found."] });

            // Track original section for timestamp update
            var originalSectionId = document.SectionId;

            // Update properties
            document.Title = request.Title;
            document.SectionId = request.SectionId;
            document.WeekNumber = request.WeekNumber;

            // Handle file update if provided
            if (request.File != null && request.File.Length > 0)
            {
                if (!string.IsNullOrEmpty(document.FilePath))
                {
                    await _fileService.DeleteFileAsync(document.FilePath);
                }

                document.FilePath = await _fileService.SaveFileAsync(request.File, _folder);
            }

            document.UpdatedAt = DateTime.UtcNow;

            // Persist changes
            var rows = await _documentRepository.UpdateAsync(document);

            // Update section timestamp
            if (originalSectionId.HasValue)
            {
                await UpdateSectionTimestampAsync(originalSectionId.Value);
            }

            // Map response with FileUrl
            var response = document.Adapt<DocumentResponse>();
            response.FileUrl = _urlHelper.BuildUrl(document.FilePath);

            return (rows, new SuccessResponse<DocumentResponse> { Data = response });
        }

        public override async Task<(int NumberOfEntries, Response Response)> DeleteAsync(int id)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null)
                return (0, new ErrorResponse { Errors = ["Document not found."] });

            var sectionId = document.SectionId;
            var result = await base.DeleteAsync(id);

            if (result.NumberOfEntries > 0 && sectionId.HasValue)
            {
                await UpdateSectionTimestampAsync(sectionId.Value);
            }

            return result;
        }

        public async Task<Response> GetAllBySectionIdAsync(int sectionId)
        {
            // Validate section exists
            var section = await _sectionRepository.GetByIdAsync(sectionId);
            if (section == null)
                return new ErrorResponse { Errors = ["Section not found."] };

            // Get all documents for this section
            var documents = await _documentRepository.GetAllBySectionIdAsync(sectionId, withTracking: false);

            // Map to response with FileUrl
            var responses = documents.Select(d =>
            {
                var response = d.Adapt<DocumentResponse>();
                response.FileUrl = _urlHelper.BuildUrl(d.FilePath);
                return response;
            });

            return new SuccessResponse<IEnumerable<DocumentResponse>> { Data = responses };
        }

        private async Task UpdateSectionTimestampAsync(int sectionId)
        {
            var section = await _sectionRepository.GetByIdAsync(sectionId);
            if (section != null)
            {
                section.UpdatedAt = DateTime.UtcNow;
                await _sectionRepository.UpdateAsync(section);
            }
        }
    }
}
