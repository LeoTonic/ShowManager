using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManager.Models
{
	// Группировка данных (участники, выступления, репетиции)
	public class SMGroup: SMElement
	{
		public List<long> IDList
		{
			get
			{
				return this.idList;
			}
		}
		private List<long> idList; // Список элементов группы (идентификаторы)

		// Конструктор
		public SMGroup(string name)
		{
			this.Name = name;
			idList = new List<long>();
		}

		// Добавление идентификатора
		public bool Add(long id)
		{
			// Проверим наличие
			if (idList.Contains(id))
			{
				return false;
			}
			idList.Add(id);
			return true;
		}
	}
}
