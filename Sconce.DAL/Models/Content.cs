using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Models
{
	public class Content : BaseModel
	{
		public int SectionId { get; set; }
		public Section? Section { get; set; }

		public string Type { get; private set; }
		public int WeekNumber { get; set; }

		public Content()
		{
			// Set to most-derived class name (e.g., Assignment)
			Type = GetType().Name;
		}
	}
}

