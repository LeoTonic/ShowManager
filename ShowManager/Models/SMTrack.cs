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
	public class SMTrack: SMElement, INotifyPropertyChanged
	{
        public event PropertyChangedEventHandler PropertyChanged;
        private CultureInfo ci = new CultureInfo("en-US");

        private SMArtist artist; // Ссылка на класс родитель исполнителя композиции
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

        public string TrackLength
        {
            get
            {
                return this.length.ToString(@"hh\:mm\:ss", ci);
            }
            set
            {
                try
                {
                    this.length = TimeSpan.Parse(value);
                }
                catch (FormatException)
                {
                    this.length = TimeSpan.Zero;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TrackLength"));
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
		public SMTrack(SMArtist parent, string name, TimeSpan length)
		{
			this.artist = parent;
			this.Name = name;
			this.length = length;
            this.minusID = 0;
            this.minusTrack = "";
            this.minusArtist = "";
            this.minusExtention = "";
		}
	}
}
