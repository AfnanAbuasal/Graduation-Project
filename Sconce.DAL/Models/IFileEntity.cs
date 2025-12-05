using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Models
{
    public interface IFileEntity
    {
        string? FilePath { get; set; }
    }
}
