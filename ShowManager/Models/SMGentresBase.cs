using System;
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

		// Перемещение элемента
		public void Move(long id, int insertIndex)
		{
			int index = GetItemIndexByID(id);
			if (index >= 0)
			{
				SMGentre gentre = GentreGroups[index];
				GentreGroups.RemoveAt(index);
				if (insertIndex >= 0)
				{
					GentreGroups.Insert(insertIndex, gentre);
				}
				else
				{
					GentreGroups.Add(gentre);
				}
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

		//
		// РАБОТА С ФАЙЛОМ
		//

		// Сохранение
		public void Save(BinaryWriter bw)
		{
			try
			{
				bw.Write(GentreGroups.Count);
				foreach (SMGentre g in GentreGroups)
				{
					g.Save(bw);
				}
			}
			catch (IOException ioex)
			{
				System.Diagnostics.Debug.WriteLine(ioex.Message);
			}

		}

		// Загрузка
		public void Load(BinaryReader br)
		{
			GentreGroups.Clear();
			try
			{
				int gCount = br.ReadInt32();
				for (int n = 0; n < gCount; n++)
				{
					var ng = new SMGentre(br);
					GentreGroups.Add(ng);
				}
			}
			catch (IOException ioex)
			{
				System.Diagnostics.Debug.WriteLine(ioex.Message);
			}
		}
	}
}
