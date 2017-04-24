using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
					System.Diagnostics.Debug.WriteLine("SMElement ID is not valid!");
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
					System.Diagnostics.Debug.WriteLine("SMElement Name is empty!");
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

		// Генерация идентификатора
		private void GenerateID()
		{
			this.id = DateTime.Now.Ticks;
		}

		protected bool Save(StreamWriter sw)
		{
			try
			{
				sw.WriteLine("SM_ELEMENT:");
				sw.WriteLine(ID.ToString());
				sw.WriteLine(Name);
				sw.WriteLine(ImageKey.ToString());
			}
			catch (IOException ioex)
			{
				System.Diagnostics.Debug.WriteLine(ioex.Message);
				return false;
			}
			return true;
		}

		/*
		 * // Example #4: Append new text to an existing file.
        // The using statement automatically flushes AND CLOSES the stream and calls 
        // IDisposable.Dispose on the stream object.
        using (System.IO.StreamWriter file = 
            new System.IO.StreamWriter(@"C:\Users\Public\TestFolder\WriteLines2.txt", true))
        {
            file.WriteLine("Fourth line");
        }
		 */
	}
}
