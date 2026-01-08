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
    public class ProgramRepository : GenericRepository<Program>, IProgramRepository
    {
        private readonly ApplicationDbContext _context;

        public ProgramRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Program>> GetProgramsByExamWriterAsync(string instructorId)
        {
            return await _context.Programs
                .AsNoTracking()
                .Where(p => p.HasProficiencyExam && p.ExamWriterInstructorId == instructorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Program>> GetProgramsByEvaluatorAsync(string instructorId)
        {
            return await _context.Programs
                .AsNoTracking()
                .Where(p => p.HasProficiencyExam && p.EvaluatorInstructorId == instructorId)
                .ToListAsync();
        }
    }
}
