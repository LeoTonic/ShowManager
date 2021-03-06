﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using ShowManager.Tools;

namespace ShowManager.Models
{
	// Класс исполнителя
	public class SMArtist : SMElement, INotifyPropertyChanged
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

		// Массив композиций
		public ObservableCollection<SMTrack> Tracks { get; set; }

		// Индекс изображения в зависимости от добавления треков на выступление
		public int AppliedTracksImage
		{
			get
			{
				if (IsAppliedAll)
					return 223;
				else if (IsAppliedNull)
					return 221;
				else
					return 222;
			}
		}

		// Все треки исполнителя использованы на выступлении
		public bool IsAppliedAll
		{
			get
			{
				var isApplied = true;
				if (Tracks.Count > 0)
				{
					foreach (SMTrack track in Tracks)
					{
						if (!track.IsApplied)
							isApplied = false;
					}
				}
				else
				{
					isApplied = false;
				}
				return isApplied;
			}
		}
		// Ни один из треков исполнителя не добавлены в выступление
		public bool IsAppliedNull
		{
			get
			{
				var isApplied = false;
				foreach (SMTrack track in Tracks)
				{
					if (track.IsApplied)
						isApplied = true;
				}
				return !isApplied;
			}
		}

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
		public TimeSpan PrepareTimeStart
		{
			get
			{
				return this.repTimeStart;
			}
			set
			{
				this.repTimeStart = value;
			}
		}
		private TimeSpan repTimeStart; // Время на подготовку к репетиции
		public TimeSpan PrepareTimeLength
		{
			get
			{
				return this.repTimeLength;
			}
			set
			{
				this.repTimeLength = value;
			}
		}
		private TimeSpan repTimeLength; // Время для репетиции
		public TimeSpan PrepareTimeFinish
		{
			get
			{
				return this.repTimeFinish;
			}
			set
			{
				this.repTimeFinish = value;
			}
		}
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
			Tracks = new ObservableCollection<SMTrack>();
			this.project = parent;
			this.gentreBase = gb;

			if (from != null)
			{
				Assign(from);
			}
			else
			{
				this.CompanyName = "";
				this.TeamDirector = "";
				this.TeamConcertMaster = "";
				this.TeamSoundDirector = "";
				this.TeamDanceMaster = "";
				this.TeamVocalMaster = "";

				this.ContactCity = "";
				this.ContactPhone = "";
				this.ContactEmail = "";
				this.ContactRegion = "";
				this.ContactAddress = "";

				this.TeamCount = "";
				this.TeamRider = "";
				this.TeamTool = "";

				this.PrepareTimeStart = TimeSpan.Zero;
				this.PrepareTimeFinish = TimeSpan.Zero;
				this.PrepareTimeLength = TimeSpan.Zero;
				this.PrepareInfo = "";

			}
		}

		// Присвоение
		public void Assign(SMArtist from)
		{
			this.ID = from.ID;
			this.Tracks.Clear();
			foreach(SMTrack getTrack in from.Tracks)
			{
				var newTrack = new SMTrack(this);
				newTrack.Assign(getTrack);
				this.Tracks.Add(newTrack);
			}

			this.ArtistName = from.ArtistName;
			this.GentreGroup = from.GentreGroup;
			this.GentreClass = from.GentreClass;
			this.GentreDirection = from.GentreDirection;
			this.GentreContent = from.GentreContent;
			this.GentreAge = from.GentreAge;
			this.GentreCategory = from.GentreCategory;

			this.CompanyName = from.CompanyName;
			this.TeamDirector = from.TeamDirector;
			this.TeamConcertMaster = from.TeamConcertMaster;
			this.TeamSoundDirector = from.TeamSoundDirector;
			this.TeamDanceMaster = from.TeamDanceMaster;
			this.TeamVocalMaster = from.TeamVocalMaster;

			this.ContactCity = from.ContactCity;
			this.ContactPhone = from.ContactPhone;
			this.ContactEmail = from.ContactEmail;
			this.ContactRegion = from.ContactRegion;
			this.ContactAddress = from.ContactAddress;

			this.TeamCount = from.TeamCount;
			this.TeamRider = from.TeamRider;
			this.TeamTool = from.TeamTool;

			this.PrepareTimeStart = from.PrepareTimeStart;
			this.PrepareTimeFinish = from.PrepareTimeFinish;
			this.PrepareTimeLength = from.PrepareTimeLength;
			this.PrepareInfo = from.PrepareInfo;
		}

		public override void IOSave(DataIO dio)
		{
			dio.WriteString(DataIO.IN_ARTIST);
			base.IOSave(dio);
			dio.WriteString(DataIO.IN_ARRAY);
			foreach (SMTrack track in Tracks)
			{
				track.IOSave(dio);
			}
			dio.WriteString(DataIO.OUT_ARRAY);

			dio.WriteLong(GentreGroup);
			dio.WriteLong(GentreClass);
			dio.WriteLong(GentreDirection);
			dio.WriteLong(GentreContent);
			dio.WriteLong(GentreAge);
			dio.WriteLong(GentreCategory);

			dio.WriteString(CompanyName);
			dio.WriteString(TeamDirector);
			dio.WriteString(TeamConcertMaster);
			dio.WriteString(TeamSoundDirector);
			dio.WriteString(TeamDanceMaster);
			dio.WriteString(TeamVocalMaster);

			dio.WriteString(ContactCity);
			dio.WriteString(ContactPhone);
			dio.WriteString(ContactEmail);
			dio.WriteString(ContactRegion);
			dio.WriteString(ContactAddress);

			dio.WriteString(TeamCount);
			dio.WriteString(TeamRider);
			dio.WriteString(TeamTool);

			dio.WriteTime(PrepareTimeStart);
			dio.WriteTime(PrepareTimeFinish);
			dio.WriteTime(PrepareTimeLength);

			dio.WriteString(PrepareInfo);

			dio.WriteString(DataIO.OUT_ARTIST);
		}

		public override bool IOLoad(DataIO dio)
		{
			if (dio.SeekTo(DataIO.IN_ELEMENT) == 1)
			{
				if (!base.IOLoad(dio))
					return false;
			}

			if (dio.SeekTo(DataIO.IN_ARRAY) == 1)
			{
				Tracks.Clear();
				while (dio.SeekTo(DataIO.IN_TRACK, DataIO.OUT_ARRAY) == 1)
				{
					var newTrack = new SMTrack(this);
					if (newTrack.IOLoad(dio))
					{
						Tracks.Add(newTrack);
					}
				}
			}
			GentreGroup = dio.ReadLong();
			GentreClass = dio.ReadLong();
			GentreDirection = dio.ReadLong();
			GentreContent = dio.ReadLong();
			GentreAge = dio.ReadLong();
			GentreCategory = dio.ReadLong();

			CompanyName = dio.ReadString();
			TeamDirector = dio.ReadString();
			TeamConcertMaster = dio.ReadString();
			TeamSoundDirector = dio.ReadString();
			TeamDanceMaster = dio.ReadString();
			TeamVocalMaster = dio.ReadString();

			ContactCity = dio.ReadString();
			ContactPhone = dio.ReadString();
			ContactEmail = dio.ReadString();
			ContactRegion = dio.ReadString();
			ContactAddress = dio.ReadString();

			TeamCount = dio.ReadString();
			TeamRider = dio.ReadString();
			TeamTool = dio.ReadString();

			PrepareTimeStart = dio.ReadTime();
			PrepareTimeFinish = dio.ReadTime();
			PrepareTimeLength = dio.ReadTime();

			PrepareInfo = dio.ReadString();

			return (dio.SeekTo(DataIO.OUT_ARTIST) == 1);
		}
	}
}
