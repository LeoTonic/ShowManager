using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ShowManager.Models
{
	// Базовый элемент во всех массивах моделей данных
	public class SMElement
	{
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
				if (string.IsNullOrWhiteSpace(value))
				{
                    System.Console.WriteLine("SMElement Name is empty!");
				}
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

		public SMElement(string sName, int nKey)
		{
			GenerateID();
			this.name = sName;
			this.imageKey = nKey;
		}

		// Генерация идентификатора
		private void GenerateID()
		{
			this.id = DateTime.Now.Ticks;
		}

		public virtual void Save(BinaryWriter bw)
		{
			try
			{
				bw.Write(ID);
				bw.Write(Name);
				bw.Write(ImageKey);
			}
			catch (IOException ioex)
			{
                System.Console.WriteLine(ioex.Message);
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
	}
}
