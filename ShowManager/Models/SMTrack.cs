using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;

namespace ShowManager.Models
{
	// Класс композиции участника
	public class SMTrack : SMElement, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private CultureInfo ci = new CultureInfo("en-US");

		private SMArtist artist; // Ссылка на класс родитель исполнителя композиции
		public SMArtist ParentArtist
		{
			get
			{
				return this.artist;
			}
		}

		private TimeSpan length; // Продолжительность композиции
		private long minusID; // Идентификатор в коллекции минусовок
		private string minusArtist; // Имя артиста в минусовке (для экспорта)
		private string minusTrack; // Имя трека в минусовке (для экспорта)
		private string minusExtention; // Расширение файла (для экспорта)

		public string TrackName
		{
			get
			{
				return this.Name;
			}
			set
			{
				this.Name = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TrackName"));
			}
		}
		public string MinusArtistName
		{
			get
			{
				return this.minusArtist;
			}
			set
			{
				this.minusArtist = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MinusArtistName"));
			}
		}

		public string MinusTrackName
		{
			get
			{
				return this.minusTrack;
			}
			set
			{
				this.minusTrack = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MinusTrackName"));
			}
		}

		public long MinusID
		{
			get
			{
				return this.minusID;
			}
			set
			{
				this.minusID = value;
			}
		}

		public string MinusExtention
		{
			get
			{
				return this.minusExtention;
			}
			set
			{
				this.minusExtention = value;
			}
		}

		public TimeSpan TrackLength
		{
			get
			{
				return this.length;
			}
			set
			{
				this.length = value;
			}
		}

		public string MinusExist
		{
			get
			{
				if (this.minusID == 0)
					return "";
				else
					return "V";
			}
		}

		// Конструктор
		public SMTrack(SMArtist parent)
		{
			this.artist = parent;
			this.Name = "";
			this.length = TimeSpan.Zero;
			this.minusID = 0;
			this.minusTrack = "";
			this.minusArtist = "";
			this.minusExtention = "";
		}

		// Копия свойств из другого объекта
		public void Assign(SMTrack from)
		{
			this.Name = from.Name;
			this.ID = from.ID;
			this.length = from.length;
			this.minusID = from.minusID;
			this.minusTrack = from.minusTrack;
			this.minusArtist = from.minusArtist;
			this.minusExtention = from.minusExtention;
		}
	}
}
