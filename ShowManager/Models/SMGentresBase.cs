using System;
using System.Windows;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManager.Models
{
	// База групповых жанров
	public class SMGentresBase
	{
		private List<SMGentre> gentreGroups;

		public List<SMGentre> GentreGroups
		{
			get
			{
				return gentreGroups;
			}
		}

		private string filePath;

		// Конструктор
		public SMGentresBase()
		{
			gentreGroups = new List<SMGentre>();
			App curApp = (App)Application.Current;
			filePath = string.Concat(AppDomain.CurrentDomain.BaseDirectory, (string)curApp.FindResource("gentresFilePath"));

			try
			{
				using (var br = new BinaryReader(File.Open(filePath, FileMode.Open)))
				{
					int gCount = br.ReadInt32();
					for (int n = 0; n < gCount; n++)
					{
						var ng = new SMGentre(br);
						gentreGroups.Add(ng);
					}
				}
			}
			catch (IOException ioex)
			{
                System.Console.WriteLine(ioex.Message);
			}
		}

		// Управление элементами

		// Добавление элемента
		public long Add(string name, int imgKey)
		{
			SMGentre newGentre = new SMGentre(name, imgKey);
			gentreGroups.Add(newGentre);
			return newGentre.ID;
		}

		// Редактирование элемента
		public void Edit(long id, string name, int imgKey)
		{
			int index = GetItemIndexByID(id);
			if (index >= 0)
			{
				SMGentre gentre = gentreGroups[index];
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
				gentreGroups.RemoveAt(index);
			}
		}

		// Перемещение элемента
		public void Move(long id, int insertIndex)
		{
			int index = GetItemIndexByID(id);
			if (index >= 0)
			{
				SMGentre gentre = gentreGroups[index];
				gentreGroups.RemoveAt(index);
				if (insertIndex >= 0)
				{
					gentreGroups.Insert(insertIndex, gentre);
				}
				else
				{
					gentreGroups.Add(gentre);
				}
			}
		}

		// Получение индекса элемента по ид
		private int GetItemIndexByID(long id)
		{
			for (int n = 0; n < gentreGroups.Count; n++)
			{
				if (gentreGroups[n].ID == id)
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
				return gentreGroups[itemIndex];
			}
			return null;
		}

		// Получение элемента в классе
		public SMElement GetClassItem(List<SMElement> classesList, long itemID)
		{
			int index = GetClassItemIndex(classesList, itemID);
			if (index >= 0)
				return classesList[index];
			else
				return null;
		}

		// Редактирование элемента в классе
		public void EditClassItem(List<SMElement> classesList, long itemID, string sName, int nKey)
		{
			int index = GetClassItemIndex(classesList, itemID);
			if (index >= 0)
			{
				SMElement sme = classesList[index];
				sme.Name = sName;
				sme.ImageKey = nKey;
			}
		}

		// Перемещение элемента в классе
		public void MoveClassItem(List<SMElement> classesList, long itemID, int insertIndex)
		{
			int index = GetClassItemIndex(classesList, itemID);
			if (index >= 0)
			{
				SMElement sme = classesList[index];
				classesList.RemoveAt(index);
				if (insertIndex >= 0)
				{
					classesList.Insert(insertIndex, sme);
				}
				else
				{
					classesList.Add(sme);
				}
			}
		}

		// Удаление элемента в классе
		public void DeleteClassItem(List<SMElement> classesList, long itemID)
		{
			int itemIndex = GetClassItemIndex(classesList, itemID);
			if (itemIndex >= 0)
			{
				classesList.RemoveAt(itemIndex);
			}
		}

		private int GetClassItemIndex(List<SMElement> clsList, long itemID)
		{
			for (int n = 0; n < clsList.Count; n++)
			{
				if (clsList[n].ID == itemID)
				{
					return n;
				}
			}
			return -1;
		}

		// Вспомогательная функция для получения ключей изображений
		public enum GentreClassType
		{
			GentreGroup = 0,
			GentreClass,
			Direction,
			Content,
			Age,
			Category
		}

		public int GetImageKey(long gentreID, GentreClassType gcType, long itemID)
		{
			// Получим жанр
			SMGentre getGentre = GetGentreItem(gentreID);

			if (getGentre == null)
				return 101;

			List<SMElement> getList = null;
			switch (gcType)
			{
				case GentreClassType.GentreGroup:
					return getGentre.ImageKey;
				case GentreClassType.GentreClass:
					getList = getGentre.Gentres;
					break;
				case GentreClassType.Direction:
					getList = getGentre.Directions;
					break;
				case GentreClassType.Content:
					getList = getGentre.Contents;
					break;
				case GentreClassType.Age:
					getList = getGentre.Ages;
					break;
				case GentreClassType.Category:
					getList = getGentre.Categories;
					break;
			}
			if (getList == null)
				return 101;
			SMElement getItem = GetClassItem(getList, itemID);
			return getItem.ImageKey;
		}

		//
		// РАБОТА С ФАЙЛОМ
		//

		// Сохранение
		public void Save()
		{
			BinaryWriter bw;
			try
			{
				bw = new BinaryWriter(new FileStream(filePath, FileMode.Create));
			}
			catch (IOException ex)
			{
                System.Console.WriteLine(ex.Message);
    			return;
			}

			try
			{
				bw.Write(gentreGroups.Count);
				foreach (SMGentre g in gentreGroups)
				{
					g.Save(bw);
				}
			}
			catch (IOException ex)
			{
                System.Console.WriteLine(ex.Message);
                return;
			}
			finally
			{
				bw.Flush();
				bw.Close();
			}
		}
	}
}
