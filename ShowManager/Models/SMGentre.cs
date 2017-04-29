using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		public SMGentre(BinaryReader br)
		{
			Load(br);
			LoadList(gentres, br);
			LoadList(directions, br);
			LoadList(contents, br);
			LoadList(ages, br);
			LoadList(categories, br);
			LoadList(evaluateTypes, br);
		}

		// Сохранение объекта
		public override void Save(BinaryWriter bw)
		{
			base.Save(bw);
			SaveList(gentres, bw);
			SaveList(directions, bw);
			SaveList(contents, bw);
			SaveList(ages, bw);
			SaveList(categories, bw);
			SaveList(evaluateTypes, bw);
		}

		// Сохранение списка в файл
		private void SaveList(List<SMElement> saveList, BinaryWriter bw)
		{
			try
			{
				bw.Write(saveList.Count);
				foreach(SMElement sme in saveList)
				{
					sme.Save(bw);
				}
			}
			catch (IOException ioex)
			{
				System.Diagnostics.Debug.WriteLine(ioex.Message);
			}
		}

		// Загрузка списка из файла
		private void LoadList(List<SMElement> loadList, BinaryReader br)
		{
			loadList.Clear();
			try
			{
				int lCount = br.ReadInt32();
				for (int n = 0; n < lCount; n++)
				{
					var newItem = new SMElement();
					newItem.Load(br);
					loadList.Add(newItem);
				}
			}
			catch (IOException ioex)
			{
				System.Diagnostics.Debug.WriteLine(ioex.Message);
			}
		}
	}
}
