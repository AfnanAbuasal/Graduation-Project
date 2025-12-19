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
    public class TextRepository : GenericRepository<Text>, ITextRepository
    {
        public TextRepository(ApplicationDbContext context) : base(context) { }
    }
}
