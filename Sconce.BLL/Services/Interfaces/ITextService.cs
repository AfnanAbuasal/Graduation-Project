using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;

namespace Sconce.BLL.Services.Interfaces
{
    public interface ITextService : IGenericService<TextRequest, TextResponse, Text>
    {
        Task<Response> GetAllBySectionAsync(int sectionId, string instructorId);
    }
}
