using Microsoft.EntityFrameworkCore;
using Sconce.DAL.Data;
using Sconce.DAL.Models;
using Sconce.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Repositories.Classes
{
    public class InformationRequestRepository : GenericRepository<InformationRequest>, IInformationRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public InformationRequestRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<InformationRequest>> GetAllOrderedAsync()
        {
            return await _context.InformationRequests
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
    }
}
