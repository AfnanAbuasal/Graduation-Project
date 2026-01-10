using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Interfaces
{
    public interface IDocumentService : IGenericService<DocumentRequest, DocumentResponse, Document>
    {
        Task<Response> GetAllBySectionIdAsync(int sectionId);
    }
}
