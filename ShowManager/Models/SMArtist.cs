using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace ShowManager.Models
{
	// Класс исполнителя
	public class SMArtist: SMElement, INotifyPropertyChanged
	{
    public event PropertyChangedEventHandler PropertyChanged;
    private SMGentresBase gentreBase;
		private SMProject project;
		public SMProject ParentProject
		{
			get
			{
				return this.project;
			}
		}
		
    public ObservableCollection<SMTrack> Tracks
    {
      get
      {
        return this.tracks;
      }
    }
		private ObservableCollection<SMTrack> tracks = new ObservableCollection<SMTrack>(); // Массив композиций участника

#region Artist_Props
        public string ArtistName
        {
            get
            {
                return this.Name;
            }
            set
            {
                this.Name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ArtistName"));
            }
        }
        // Категория участника
        public long GentreGroup
        {
            get
            {
                return this.propGentreGroupID;
            }
            set
            {
                this.propGentreGroupID = value;
            }
        }
        private long propGentreGroupID; // Группа жанра (вокал, хореография, эстрада и т.д.)

        public long GentreClass
        {
            get
            {
                return this.propGentreTypeID;
            }
            set
            {
                this.propGentreTypeID = value;
            }
        }
        private long propGentreTypeID; // Жанр (вокал, хореография, и т.д.)

        public long GentreDirection
        {
            get
            {
                return this.propDirectionID;
            }
            set
            {
                this.propDirectionID = value;
            }
        }
        private long propDirectionID; // Направление (академическое, народное и т.д.)

        public long GentreContent
        {
            get
            {
                return this.propContentID;
            }
            set
            {
                this.propContentID = value;
            }
        }
        private long propContentID; // Состав (ансамбль, хор, дуэт, трио и т.д.)

        public long GentreAge
        {
            get
            {
                return this.propAgeID;
            }
            set
            {
                this.propAgeID = value;
            }
        }
        private long propAgeID; // Возрастная группа

        public long GentreCategory
        {
            get
            {
                return this.propCategoryID;
            }
            set
            {
                this.propCategoryID = value;
            }
        }
        private long propCategoryID; // Категория (начинающий, любитель, профессионал)
        #endregion

#region Company_Props
        public string CompanyName
        {
            get
            {
                return this.baseCompany;
            }
            set
            {
                this.baseCompany = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CompanyName"));

            }
        }
		private string baseCompany; // Организация - ведомственная принадлежность
        public string TeamDirector
        {
            get
            {
                return this.teamDirector;
            }
            set
            {
                this.teamDirector = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TeamDirector"));
            }
        }
        private string teamDirector; // Руководитель

        public string TeamConcertMaster
        {
            get
            {
                return this.teamConcertMaster;
            }
            set
            {
                this.teamConcertMaster = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TeamConcertMaster"));
            }
        }
        private string teamConcertMaster; // Концертмейстер

        public string TeamSoundDirector
        {
            get
            {
                return this.teamSoundDirector;
            }
            set
            {
                this.teamSoundDirector = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TeamSoundDirector"));
            }
        }
        private string teamSoundDirector; // Звукорежиссер


        public string TeamDanceMaster
        {
            get
            {
                return this.teamDanceMaster;
            }
            set
            {
                this.teamDanceMaster = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TeamDanceMaster"));
            }
        }
        private string teamDanceMaster; // Хореограф

        public string TeamVocalMaster
        {
            get
            {
                return this.teamVocalMaster;
            }
            set
            {
                this.teamVocalMaster = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TeamVocalMaster"));
            }
        }
        private string teamVocalMaster; // Педагог по вокалу
        #endregion

#region Contact_Props
        public string ContactCity
        {
            get
            {
                return this.baseCity;
            }
            set
            {
                this.baseCity = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ContactCity"));
            }
        }
		private string baseCity; // Населенный пункт

        public string ContactPhone
        {
            get
            {
                return this.basePhone;
            }
            set
            {
                this.basePhone = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ContactPhone"));
            }
        }
		private string basePhone; // Телефон

        public string ContactEmail
        {
            get
            {
                return this.baseEmail;
            }
            set
            {
                this.baseEmail = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ContactEmail"));
            }
        }
        private string baseEmail; // Электронная почта

        public string ContactRegion
        {
            get
            {
                return this.baseRegion;
            }
            set
            {
                this.baseRegion = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ContactRegion"));
            }
        }
        private string baseRegion; // Область

        public string ContactAddress
        {
            get
            {
                return this.baseAddress;
            }
            set
            {
                this.baseAddress = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ContactAddress"));
            }
        }
        private string baseAddress; // Почтовый адрес
        #endregion

#region Addition_Props
        public string TeamCount
        {
            get
            {
                return this.teamCount.ToString();
            }
            set
            {
                try
                {
                    this.teamCount = int.Parse(value);
                }
                catch
                {
                    this.teamCount = 0;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TeamCount"));
            }
        }
        private int teamCount; // Численность коллектива

        public string TeamRider
        {
            get
            {
                return this.teamTechRider;
            }
            set
            {
                this.teamTechRider = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TeamRider"));
            }
        }
        private string teamTechRider; // Техрайдер

        public string TeamTool
        {
            get
            {
                return this.teamTool;
            }
            set
            {
                this.teamTool = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TeamTool"));
            }
        }
        private string teamTool; // Информация об инструменте

        // Репетиция
        private TimeSpan repTimeStart; // Время на подготовку к репетиции
        private TimeSpan repTimeLength; // Время для репетиции
        private TimeSpan repTimeFinish; // Время на разборку оборудования после репетиции

        public string PrepareInfo
        {
            get
            {
                return this.repInfo;
            }
            set
            {
                this.repInfo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PrepareInfo"));
            }
        }
        private string repInfo; // Информация для репетиции
#endregion

        // Конструктор
        public SMArtist(SMProject parent, SMGentresBase gb, SMArtist from)
        {
					this.project = parent;
					this.gentreBase = gb;

          if (from == null)
          {
            this.Name = "";
          }
          else
          {
            this.Name = from.Name;
            this.ID = from.ID;
          }
        }
	}
}
