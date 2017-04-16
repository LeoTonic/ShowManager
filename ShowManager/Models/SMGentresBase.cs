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

		// Управление элементами

		// Добавление элемента
		public long Add(string name, int imgKey)
		{
			SMGentre newGentre = new SMGentre(name, imgKey);
			GentreGroups.Add(newGentre);
			return newGentre.ID;
		}

		// Редактирование элемента
		public void Edit(long id, string name, int imgKey)
		{
			int index = GetItemIndexByID(id);
			if (index >= 0)
			{
				SMGentre gentre = GentreGroups[index];
				gentre.Name = name;
				gentre.ImageKey = imgKey;
			}
		}

		// Удаление элемента
		public void Remove(long id)
		{
			int index = GetItemIndexByID(id);
			if (index >= 0)
			{
				GentreGroups.RemoveAt(index);
			}
		}

		// Получение индекса элемента по ид
		private int GetItemIndexByID(long id)
		{
			for (int n = 0; n < GentreGroups.Count; n++)
			{
				if (GentreGroups[n].ID == id)
				{
					return n;
				}
			}
			return -1;
		}

		// Получение элемента
		public SMGentre GetGentreItem(long id)
		{
			int itemIndex = GetItemIndexByID(id);
			if (itemIndex >= 0)
			{
				return GentreGroups[itemIndex];
			}
			return null;
		}
	}
}
