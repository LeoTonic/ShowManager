using System.Collections.Generic;
using ShowManager.Models;

namespace ShowManager.Tools
{
	public class FilterView
	{
		static long newID = 1;

		// Сортировка
		public Dictionary<int, string> sortTypes; // Значения по каким можно сортировать
		public bool sortAscend; // Сортировка по возрастанию 
		public int SortingType; // Текущий тип сортировки

		// Фильтрация данных - Элемент данных
		public class FilterItem
		{
			public bool? Checked { get; set; }
			public long ID { get; set; }
			public string Name { get; set; }

			public List<FilterItem> Children { get; set; }
			public FilterItem Parent { get; set; }

			public void AddChildren(long id, string name)
			{
				// Проверка на наличие
				foreach(FilterItem item in Children)
				{
					if (name == item.Name)
						return;
				}
				// Вставляем
				var fi = new FilterItem()
				{
					ID = id,
					Name = name,
					Checked = false,
					Parent = this,
					Children = null
				};
				Children.Add(fi);
			} // Добавляем элемент в детей
		}
		public List<FilterItem> FilterItems; // Список значений для фильтра

		public FilterView(SMGentresBase gentres)
		{
			// Инициализация SortingTypes
			sortTypes = new Dictionary<int, string>()
			{
				{ 1000, "Нет" },
				{ 1001, "Исполнитель" },
				{ 1002, "Организация" },
				{ 1003, "Жанр" },
				{ 1004, "Возраст" },
				{ 1005, "Категория" },
				{ 1006, "Выступление" }
			};
			SortingType = 1000;
			sortAscend = true;

			// Инициализация FilterItems
			FilterItems = new List<FilterItem>();

			var filterGentre = new FilterItem()
			{
				ID = newID++,
				Name = "Жанр",
				Checked = false,
				Parent = null,
				Children = new List<FilterItem>()
			}; // Жанр
			var filterContent = new FilterItem()
			{
				ID = newID++,
				Name = "Состав участников",
				Checked = false,
				Parent = null,
				Children = new List<FilterItem>()
			}; // Состав участников
			var filterAge = new FilterItem()
			{
				ID = newID++,
				Name = "Возрастная группа",
				Checked = false,
				Parent = null,
				Children = new List<FilterItem>()
			}; // Возрастная группа
			var filterCategory = new FilterItem()
			{
				ID = newID++,
				Name = "Категория",
				Checked = false,
				Parent = null,
				Children = new List<FilterItem>()
			}; // Категория

			foreach (SMGentre gentre in gentres.GentreGroups)
			{
				filterGentre.AddChildren(gentre.ID, gentre.Name);

				foreach(SMElement element in gentre.Contents)
				{
					filterContent.AddChildren(element.ID, element.Name);
				}
				foreach(SMElement element in gentre.Ages)
				{
					filterAge.AddChildren(element.ID, element.Name);
				}
				foreach(SMElement element in gentre.Categories)
				{
					filterCategory.AddChildren(element.ID, element.Name);
				}
			}
			FilterItems.Add(filterGentre);
			FilterItems.Add(filterContent);
			FilterItems.Add(filterAge);
			FilterItems.Add(filterCategory);

			var filterShow = new FilterItem()
			{
				ID = newID++,
				Name = "Участник на выступлении",
				Checked = false,
				Parent = null,
				Children = new List<FilterItem>()
			}; // Выступления
			filterShow.AddChildren(newID++, "Полностью");
			filterShow.AddChildren(newID++, "Частично");
			filterShow.AddChildren(newID++, "Отсутствует");
			FilterItems.Add(filterShow);
		}
	}
}
