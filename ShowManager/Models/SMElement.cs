using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManager.Models
{
	public class SMElement
	{
		private long id; // Идентификатор элемента (для поиска)
		private string name; // Имя элемента

		private string imagePath; // Путь к картинке

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

		public string ImagePath
		{
			get
			{
				return this.imagePath;
			}
			set
			{
				this.imagePath = value;
			}

		}
		// Конструктор
		public SMElement()
		{
			GenerateID();
			this.name = "";
			this.imagePath = "";
		}

		// Генерация идентификатора
		private void GenerateID()
		{
			this.id = DateTime.Now.Ticks;
		}

	}
}
