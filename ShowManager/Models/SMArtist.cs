using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManager.Models
{
	// Класс исполнителя
	class SMArtist: SMElement
	{
		private List<SMTrack> tracks; // Массив композиций участника

		private string baseCompany; // Организация - ведомственная принадлежность
		private string baseCity; // Населенный пункт
		private string basePhone; // Телефон
		private string baseEmail; // Электронная почта

		private string teamDirector; // Руководитель
		private string teamConcertMaster; // Концертмейстер
		private string teamDanceMaster; // Хореограф
		private string teamVocalMaster; // Педагог по вокалу
		private int teamCount; // Численность коллектива
		private string teamTechRider; // Техрайдер
		private string teamInfo; // Дополнительная информация

		// Репетиция
		private long repTimeStart; // Время на подготовку к репетиции
		private long repTimeLength; // Время для репетиции
		private long repTimeFinish; // Время на разборку оборудования после репетиции
		private string repInfo; // Информация для репетиции

		// Категория участника
		private int propGentreGroupID; // Группа жанра (вокал, хореография, эстрада и т.д.)

		private int propGentreTypeID; // Жанр (вокал, хореография, и т.д.)
		private int propDirectionID; // Направление (академическое, народное и т.д.)
		private int propContentID; // Состав (ансамбль, хор, дуэт, трио и т.д.)
		private int propAgeID; // Возрастная группа
		private int propCategoryID; // Категория (начинающий, любитель, профессионал)

		// Конструктор
		public SMArtist(string name, string company)
		{
			this.Name = name;
			this.baseCompany = company;

			tracks = new List<SMTrack>();
		}
	}
}
