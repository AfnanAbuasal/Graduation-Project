using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using Sconce.DAL.Repositories.Interfaces;

namespace Sconce.BLL.Services.Classes;

public class TextService : GenericService<TextRequest, TextResponse, Text>, ITextService
{
    public TextService(ITextRepository textRepository) : base(textRepository)
    {
    }
}
