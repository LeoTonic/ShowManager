﻿using System.Collections.Generic;
using ShowManager.Models;

namespace ShowManager.Tools
{
	public class FilterView
	{
		static long newID = 1;

    public enum SortType
    {
      None = 1000,
      Artist = 1001,
      Company = 1002,
      Gentre = 1003,
      Age = 1004,
      Category = 1005,
      Show = 1006
    };

		// Сортировка
		public Dictionary<int, string> sortTypes; // Значения по каким можно сортировать
		public bool sortAscend; // Сортировка по возрастанию 
		public int SortingType; // Текущий тип сортировки

		// Фильтрация данных - Элемент данных
		public class FilterItem
		{
			public bool? Checked { get; set; }
			public List<long> ID { get; set; }
			public string Name { get; set; }

			public List<FilterItem> Children { get; set; }
			public FilterItem Parent { get; set; }

			public void AddChildren(long id, string name)
			{
				// Проверка на наличие
				foreach(FilterItem item in Children)
				{
					if (name == item.Name)
          {
            item.ID.Add(id);
            return;
          }
				}
				// Вставляем
				var fi = new FilterItem(id, name, this);
				Children.Add(fi);
			} // Добавляем элемент в детей

      public bool IsExist(long id)
      {
        foreach(long lid in ID)
        {
          if (lid == id && Checked == true) return true;
        }
        return false;
      } // Проверка на наличие ИД в массиве и установку фильтра

			public FilterItem(long id, string name, FilterItem parent) {
				Checked = false;
				Name = name;
        ID = new List<long>() { id };
				Parent = parent;
				Children = new List<FilterItem>();
			}
			// Копия через конструктор
			public FilterItem(FilterItem from)
			{
				Checked = from.Checked;
        ID = new List<long>();
        foreach(long id in from.ID) { ID.Add(id); }
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

    private long applyShowIDAll, applyShowIDNone, applyShowIDMix;

		// Инициализация SortingTypes
		private void InitializeSortTypes()
		{
			sortTypes = new Dictionary<int, string>()
			{
				{ (int)SortType.None, "Нет" },
				{ (int)SortType.Artist, "Исполнитель" },
				{ (int)SortType.Company, "Организация" },
				{ (int)SortType.Gentre, "Жанр" },
				{ (int)SortType.Age, "Возраст" },
				{ (int)SortType.Category, "Категория" },
				{ (int)SortType.Show, "Выступление" }
			};
			SortingType = (int)SortType.None;
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
      applyShowIDAll = newID++;
      applyShowIDMix = newID++;
      applyShowIDNone = newID++;
			filterShow.AddChildren(applyShowIDAll, "Полностью");
			filterShow.AddChildren(applyShowIDMix, "Частично");
			filterShow.AddChildren(applyShowIDNone, "Отсутствует");
			FilterItems.Add(filterShow);
		}

    // Возврат true если артист попадает по фильтрацию
    public bool IsFiltered(SMArtist artist)
    {
      // Проверка на пустой фильтр
      bool clear = true;
      foreach(FilterItem item in FilterItems)
      {
        foreach(FilterItem child in item.Children)
        {
          if (child.Checked == true) clear = false;
        }
        if (item.Checked == true) clear = false;
      }
      if (clear) return true;

      // Проверка на жанр
      foreach(FilterItem child in FilterItems[0].Children)
      {
        if (child.IsExist(artist.GentreGroup)) return true;
      }
      // Проверка на состав
      foreach (FilterItem child in FilterItems[1].Children)
      {
        if (child.IsExist(artist.GentreContent)) return true;
      }
      // Проверка на возраст
      foreach (FilterItem child in FilterItems[2].Children)
      {
        if (child.IsExist(artist.GentreAge)) return true;
      }
      // Проверка на категорию
      foreach (FilterItem child in FilterItems[3].Children)
      {
        if (child.IsExist(artist.GentreCategory)) return true;
      }
      // Проверка на наличие в выступлении
      foreach (FilterItem child in FilterItems[4].Children)
      {
        bool applyAll = artist.IsAppliedAll;
        bool applyNone = artist.IsAppliedNull;
        if (child.IsExist(applyShowIDAll) && applyAll) return true;
        else if (child.IsExist(applyShowIDNone) && applyNone) return true;
        else if (child.IsExist(applyShowIDMix) && !applyAll && !applyNone) return true;
      }
      return false;
    }
	}
}
