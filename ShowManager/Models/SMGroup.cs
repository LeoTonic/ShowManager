using System;
using System.Collections.Generic;
using System.IO;
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
		public override void Save(BinaryWriter bw)
		{
			base.Save(bw);

			try
			{
				bw.Write(IDList.Count);
				foreach(long id in IDList)
				{
					bw.Write(id);
				}
				bw.Write(TimeString(TimeStart));
			}
			catch (IOException ioex)
			{
				System.Console.WriteLine(ioex.Message);
			}
		}
		public override void Load(BinaryReader br)
		{
			base.Load(br);

			try
			{
				IDList.Clear();
				var idCount = br.ReadInt32();
				for (var n = 0; n < idCount; n++)
				{
					long id = br.ReadInt64();
					IDList.Add(id);
				}
				TimeStart = StringTime(br.ReadString());
			}
			catch (IOException ioex)
			{
				System.Console.WriteLine(ioex.Message);
			}
		}
	}
}
