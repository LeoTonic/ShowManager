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
				var fi = new FilterItem(id, name, this);
				Children.Add(fi);
			} // Добавляем элемент в детей

			public FilterItem(long id, string name, FilterItem parent) {
				Checked = false;
				Name = name;
				Parent = parent;
				Children = new List<FilterItem>();
			}
			// Копия через конструктор
			public FilterItem(FilterItem from)
			{
				Checked = from.Checked;
				ID = from.ID;
				Name = from.Name;
				Parent = from.Parent;
				Children = new List<FilterItem>();
				foreach(FilterItem fi in from.Children)
				{
					var newFI = new FilterItem(fi);
					Children.Add(newFI);
				}
			}
		}
		public List<FilterItem> FilterItems; // Список значений для фильтра

		// Инициализация SortingTypes
		private void InitializeSortTypes()
		{
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
		}

		public FilterView(FilterView from)
		{
			InitializeSortTypes();
			FilterItems = new List<FilterItem>();
			Assign(from);
		}

		public void Assign(FilterView from)
		{
			SortingType = from.SortingType;
			sortAscend = from.sortAscend;

			FilterItems.Clear();
			foreach (FilterItem fi in from.FilterItems)
			{
				var newFI = new FilterItem(fi);
				FilterItems.Add(newFI);
			}
		}

		public FilterView(SMGentresBase gentres)
		{
			InitializeSortTypes();

			// Инициализация FilterItems
			FilterItems = new List<FilterItem>();

			var filterGentre = new FilterItem(newID++, "Жанр", null);
			var filterContent = new FilterItem(newID++, "Состав участников", null);
			var filterAge = new FilterItem(newID++, "Возрастная группа", null);
			var filterCategory = new FilterItem(newID++, "Категория", null);

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

			var filterShow = new FilterItem(newID++, "Участник на выступлении", null);

			filterShow.AddChildren(newID++, "Полностью");
			filterShow.AddChildren(newID++, "Частично");
			filterShow.AddChildren(newID++, "Отсутствует");
			FilterItems.Add(filterShow);
		}
	}
}
