using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManager.Models
{
	// Группировка данных (участники, выступления, репетиции)
	class SMGroup: SMElement
	{
		private List<long> idList; // Список элементов группы (идентификаторы)

		// Конструктор
		public SMGroup(string name)
		{
			this.Name = name;
			idList = new List<long>();
		}
	}
}
