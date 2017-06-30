using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;

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

		public override void Save(BinaryWriter bw)
		{
			base.Save(bw);
			try
			{
				bw.Write(Tracks.Count);
				foreach(SMTrack track in Tracks)
				{
					track.Save(bw);
				}

				bw.Write(GentreGroup);
				bw.Write(GentreClass);
				bw.Write(GentreDirection);
				bw.Write(GentreContent);
				bw.Write(GentreAge);
				bw.Write(GentreCategory);

				bw.Write(CompanyName);
				bw.Write(TeamDirector);
				bw.Write(TeamConcertMaster);
				bw.Write(TeamSoundDirector);
				bw.Write(TeamDanceMaster);
				bw.Write(TeamVocalMaster);

				bw.Write(ContactCity);
				bw.Write(ContactPhone);
				bw.Write(ContactEmail);
				bw.Write(ContactRegion);
				bw.Write(ContactAddress);

				bw.Write(TeamCount);
				bw.Write(TeamRider);
				bw.Write(TeamTool);

				bw.Write(TimeString(PrepareTimeStart));
				bw.Write(TimeString(PrepareTimeFinish));
				bw.Write(TimeString(PrepareTimeLength));

				bw.Write(PrepareInfo);
			}
			catch (IOException ioex)
			{
				System.Console.WriteLine(ioex.Message);
			}
		}

		public override void Load(BinaryReader br)
		{
			base.Load(br);
			try
			{
				Tracks.Clear();
				var tCount = br.ReadInt32();
				for (var n = 0; n < tCount; n++)
				{
					var newTrack = new SMTrack(this);
					newTrack.Load(br);
					Tracks.Add(newTrack);
				}

				GentreGroup = br.ReadInt64();
				GentreClass = br.ReadInt64();
				GentreDirection = br.ReadInt64();
				GentreContent = br.ReadInt64();
				GentreAge = br.ReadInt64();
				GentreCategory = br.ReadInt64();

				CompanyName = br.ReadString();
				TeamDirector = br.ReadString();
				TeamConcertMaster = br.ReadString();
				TeamSoundDirector = br.ReadString();
				TeamDanceMaster = br.ReadString();
				TeamVocalMaster = br.ReadString();

				ContactCity = br.ReadString();
				ContactPhone = br.ReadString();
				ContactEmail = br.ReadString();
				ContactRegion = br.ReadString();
				ContactAddress = br.ReadString();

				TeamCount = br.ReadString();
				TeamRider = br.ReadString();
				TeamTool = br.ReadString();

				PrepareTimeStart = StringTime(br.ReadString());
				PrepareTimeFinish = StringTime(br.ReadString());
				PrepareTimeLength = StringTime(br.ReadString());

				PrepareInfo = br.ReadString();
			}
			catch (IOException ioex)
			{
				System.Console.WriteLine(ioex.Message);
			}
		}
	}
}
