using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManager.Models
{
	// База групповых жанров
	class SMGentresBase
	{
		private List<SMGentre> GentreGroups;

		// Конструктор
		public SMGentresBase()
		{
			GentreGroups = new List<SMGentre>();
		}
	}
}
