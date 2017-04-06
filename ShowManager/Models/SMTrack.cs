using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManager.Models
{
	// Класс композиции участника
	class SMTrack: SMElement
	{
		private SMArtist artist; // Ссылка на класс родитель исполнителя композиции
		private long length; // Продолжительность композиции

		// Конструктор
		public SMTrack(SMArtist parent, string name, long length)
		{
			this.artist = parent;
			this.Name = name;
			this.length = length;
		}
	}
}
