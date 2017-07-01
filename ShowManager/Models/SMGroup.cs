using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShowManager.Tools;

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

		private TimeSpan timeStart; // Время начала активности группы (для репетиций и выступлений)
		public TimeSpan TimeStart
		{
			get
			{
				return this.timeStart;
			}
			set
			{
				this.timeStart = value;
			}
		}
		// Конструктор
		public SMGroup(string name)
		{
			this.Name = name;
			idList = new List<long>();
		}

		// Добавление идентификатора
		public bool Add(long id, int insIndex)
		{
			// Проверим наличие
			if (idList.Contains(id))
			{
				return false;
			}
			if (insIndex >= 0)
			{
				idList.Insert(insIndex, id);
			}
			else
			{
				idList.Add(id);
			}
			return true;
		}

		// Удаление идентификатора
		public bool Remove(long id)
		{
			if (!idList.Contains(id))
			{
				return false;
			}
			idList.Remove(id);
			return true;
		}

		// Перемещение
		public void Move(long id, int insertIndex)
		{
			if (!idList.Contains(id))
			{
				return;
			}

			idList.Remove(id);
			if (insertIndex >= 0)
			{
				idList.Insert(insertIndex, id);
			}
			else
			{
				idList.Add(id);
			}
		}

		// Удаляем содержимое
		public void Clear()
		{
			idList.Clear();
		}

		// Клонируем список
		public void CloneList(List<long> cloneTo)
		{
			cloneTo.Clear();
			foreach(long id in idList)
			{
				cloneTo.Add(id);
			}
		}

		// Файловые операции
		public override void IOSave(DataIO dio)
		{
			dio.WriteString(DataIO.IN_GROUP);
			base.IOSave(dio);
			dio.WriteString(DataIO.IN_ARRAY);
			foreach(long id in IDList)
			{
				dio.WriteLong(id);
			}
			dio.WriteString(DataIO.OUT_ARRAY);
			dio.WriteTime(TimeStart);
			dio.WriteString(DataIO.OUT_GROUP);
		}
		public override bool IOLoad(DataIO dio)
		{
			if (dio.SeekTo(DataIO.IN_ELEMENT) == 1)
			{
				if (!base.IOLoad(dio))
					return false;
			}
			if (dio.SeekTo(DataIO.IN_ARRAY) == 1)
			{
				IDList.Clear();
				var sVal = dio.ReadString();
				while (sVal != DataIO.OUT_ARRAY)
				{
					if (long.TryParse(sVal, out long id))
					{
						IDList.Add(id);
					}
					else
					{
						IDList.Add(0);
					}
					sVal = dio.ReadString();
				}
			}
			TimeStart = dio.ReadTime();
			return (dio.SeekTo(DataIO.OUT_GROUP) == 1);
		}
	}
}
