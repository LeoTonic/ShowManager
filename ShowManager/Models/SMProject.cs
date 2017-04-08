using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManager.Models
{
	// Класс проекта фестиваля
	class SMProject
	{
		private List<SMArtist> artists; // Массив участников фестиваля
		private List<SMGroup> groupsArtist; // Массив групп участников
		private List<SMGroup> groupsShow; // Массив групп в выступлениях
		private List<SMGroup> groupsPrepare; // Массив групп в репетициях

		// Конструктор
		public SMProject()
		{
			artists = new List<SMArtist>();
			groupsArtist = new List<SMGroup>();
			groupsShow = new List<SMGroup>();
			groupsPrepare = new List<SMGroup>();
		}
	}
}
