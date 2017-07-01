using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShowManager.Tools;

namespace ShowManager.Models
{
	// Жанровая группа
	public class SMGentre: SMElement
	{
		// Имя и идентификатор базового элемента - ИД и название группы жанра

		private List<SMElement> gentres = new List<SMElement>(); // Жанры
		private List<SMElement> directions = new List<SMElement>(); // Направления
		private List<SMElement> contents = new List<SMElement>(); // Состав участников
		private List<SMElement> ages = new List<SMElement>(); // Возрастные группы
		private List<SMElement> categories = new List<SMElement>(); // Категории
		private List<SMElement> evaluateTypes = new List<SMElement>(); // Типы оценок

		public List<SMElement> Gentres
		{
			get
			{
				return gentres;
			}
		}
		public List<SMElement> Directions
		{
			get
			{
				return directions;
			}
		}
		public List<SMElement> Contents
		{
			get
			{
				return contents;
			}
		}

		public List<SMElement> Ages
		{
			get
			{
				return ages;
			}
		}

		public List<SMElement> Categories
		{
			get
			{
				return categories;
			}
		}

		public List<SMElement> EvaluateTypes
		{
			get
			{
				return evaluateTypes;
			}
		}

		// Конструктор
		public SMGentre(string name, int imgKey)
		{
			this.Name = name;
			this.ImageKey = imgKey;
		}

		// Конструктор для загрузки
		public SMGentre(DataIO dio)
		{
			if (dio.SeekTo(DataIO.IN_ELEMENT) == 1)
			{
				IOLoad(dio);
			}
			LoadList(gentres, dio);
			LoadList(directions, dio);
			LoadList(contents, dio);
			LoadList(ages, dio);
			LoadList(categories, dio);
			LoadList(evaluateTypes, dio);
		}

		// Сохранение объекта
		public override void IOSave(DataIO dio)
		{
			dio.WriteString(DataIO.IN_GENTRE);
			base.IOSave(dio);
			SaveList(gentres, dio);
			SaveList(directions, dio);
			SaveList(contents, dio);
			SaveList(ages, dio);
			SaveList(categories, dio);
			SaveList(evaluateTypes, dio);
			dio.WriteString(DataIO.OUT_GENTRE);
		}

		// Сохранение списка в файл
		private void SaveList(List<SMElement> saveList, DataIO dio)
		{
			dio.WriteString(DataIO.IN_ARRAY);
			foreach(SMElement sme in saveList)
			{
				sme.IOSave(dio);
			}
			dio.WriteString(DataIO.OUT_ARRAY);
		}

		// Загрузка списка из файла
		private void LoadList(List<SMElement> loadList, DataIO dio)
		{
			loadList.Clear();
			while (dio.SeekTo(DataIO.IN_ELEMENT, DataIO.OUT_ARRAY) == 1)
			{
				loadList.Add(new SMElement(dio));
			}
		}
	}
}
