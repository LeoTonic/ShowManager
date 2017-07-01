using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;
using ShowManager.Tools;

namespace ShowManager.Models
{
	// Базовый элемент во всех массивах моделей данных
	public class SMElement
	{
		static long lastID; // последний идентификатор

		private CultureInfo ci = new CultureInfo("en-US");

		protected string TimeString(TimeSpan from)
		{
			return from.ToString(@"hh\:mm\:ss", ci);
		}
		protected TimeSpan StringTime(string from)
		{
			if (!TimeSpan.TryParse(from, out TimeSpan tsVal))
			{
				tsVal = TimeSpan.Zero;
			}
			return tsVal;
		}

		private long id; // Идентификатор элемента (для поиска)
		private string name; // Имя элемента

		private int imageKey; // Ключ к изображению (из словаря ImgPath в App)

		public long ID
		{
			get
			{
				return this.id;
			}
			set
			{
				if (value > 0)
				{
					this.id = value;
				}
				else
				{
					System.Console.WriteLine("SMElement ID is not valid!");
					GenerateID();
				}
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
//				if (string.IsNullOrWhiteSpace(value))
//				{
//					System.Console.WriteLine("SMElement Name is empty!");
//				}
				this.name = value;
			}
		}

		public int ImageKey
		{
			get
			{
				return this.imageKey;
			}
			set
			{
				this.imageKey = value;
			}
		}

		// Конструктор
		public SMElement()
		{
			GenerateID();
			this.name = "";
		}

		public SMElement(DataIO dio)
		{
			ID = dio.ReadLong();
			Name = dio.ReadString();
			ImageKey = dio.ReadInt();

			dio.SeekTo(DataIO.OUT_ELEMENT);
		}

		public SMElement(string sName, int nKey)
		{
			GenerateID();
			this.name = sName;
			this.imageKey = nKey;
		}

		// Генерация идентификатора
		private void GenerateID()
		{
			long curID;

			while (true)
			{
				curID = DateTime.Now.Ticks;
				if (curID == lastID) continue;
				this.id = curID;
				lastID = curID;
				break;
			}
		}

		public virtual void Load(BinaryReader br)
		{
			try
			{
				ID = br.ReadInt64();
				Name = br.ReadString();
				ImageKey = br.ReadInt32();
			}
			catch (IOException ioex)
			{
				System.Console.WriteLine(ioex.Message);
			}
		}

		public virtual void IOSave(DataIO dio)
		{
			dio.WriteString(DataIO.IN_ELEMENT);
			dio.WriteLong(ID);
			dio.WriteString(Name);
			dio.WriteInt(ImageKey);
			dio.WriteString(DataIO.OUT_ELEMENT);
		}
		public virtual bool IOLoad(DataIO dio)
		{
			ID = dio.ReadLong();
			Name = dio.ReadString();
			ImageKey = dio.ReadInt();

			if (dio.SeekTo(DataIO.OUT_ELEMENT) == 1)
				return true;
			else
				return false;
		}
	}
}
