using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManager.Models
{
	// Жанровая группа
	class SMGentre: SMElement
	{
		// Имя и идентификатор базового элемента - ИД и название группы жанра

		private List<SMElement> Gentres; // Жанры
		private List<SMElement> Directions; // Направления
		private List<SMElement> Contents; // Состав участников
		private List<SMElement> Ages; // Возрастные группы
		private List<SMElement> Categories; // Категории
		private List<string> EvaluateTypes; // Типы оценок

		// Конструктор
		public SMGentre(string name)
		{
			this.Name = name;

			Gentres = new List<SMElement>();
			Directions = new List<SMElement>();
			Contents = new List<SMElement>();
			Ages = new List<SMElement>();
			Categories = new List<SMElement>();
			EvaluateTypes = new List<string>();
		}
	}
}
