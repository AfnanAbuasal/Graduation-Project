using Sconce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Repositories.Interfaces
{
    public interface IDocumentRepository : IGenericRepository<Document>
    {
        Task<IEnumerable<Document>> GetAllBySectionIdAsync(int sectionId, bool withTracking = false);
    }
}
