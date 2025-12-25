using Sconce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Repositories.Interfaces
{
    public interface ITextRepository : IGenericRepository<Text>
    {
        Task<IEnumerable<Text>> GetAllBySectionIdAsync(int sectionId, bool withTracking = false);
    }
}
