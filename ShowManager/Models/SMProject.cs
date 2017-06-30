using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ShowManager.Models
{
	// Класс проекта фестиваля
	public class SMProject
	{
		// Типы группы
		public enum GroupType
		{
			Artist = 0,
			Show,
			Prepare
		}

		public List<SMArtist> Artists
		{
			get
			{
				return this.artists;
			}
		}
		private List<SMArtist> artists; // Массив участников фестиваля

		public List<SMGroup> GroupsArtist
		{
			get
			{
				return this.groupsArtist;
			}
		}
		private List<SMGroup> groupsArtist; // Массив групп участников

		public List<SMGroup> GroupsShow
		{
			get
			{
				return this.groupsShow;
			}
		}
		private List<SMGroup> groupsShow; // Массив групп в выступлениях

		public List<SMGroup> GroupsPrepare
		{
			get
			{
				return this.groupsPrepare;
			}
		}
		private List<SMGroup> groupsPrepare; // Массив групп в репетициях

		private string trackFolder;
		public string TrackFolderPath
		{
			get
			{
				return this.trackFolder;
			}
			set
			{
				this.trackFolder = value;
			}
		}

		private SMGentresBase gentres;

		private string name;
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		// Конструктор
		public SMProject(SMGentresBase gbase)
		{
			Name = "Новый фестиваль";

			gentres = gbase;

			artists = new List<SMArtist>();
			groupsArtist = new List<SMGroup>();
			groupsShow = new List<SMGroup>();
			groupsPrepare = new List<SMGroup>();

			// Добавляем базовые группы
			var artistGroup = new SMGroup("Основная группа");
			var showGroup = new SMGroup("Основная часть");
			var prepGroup = new SMGroup("Основная часть");

			groupsArtist.Add(artistGroup);
			groupsShow.Add(showGroup);
			groupsPrepare.Add(prepGroup);
		}

		// Получение ссылки на группу по имени
		public SMGroup GetGroup(GroupType groupType, string groupName)
		{
			List<SMGroup> getGroup = null;
			switch (groupType)
			{
				case GroupType.Artist:
					getGroup = this.groupsArtist;
					break;
				case GroupType.Show:
					getGroup = this.groupsShow;
					break;
				case GroupType.Prepare:
					getGroup = this.groupsPrepare;
					break;
			}
			if (getGroup == null)
				return null;

			// Поиск по имени группы
			foreach (SMGroup group in getGroup)
			{
				if (group.Name == groupName)
				{
					return group;
				}
			}
			return null;
		}

		// Получение ссылки на объект исполнителя по идентификатору
		public SMArtist GetArtistByID(long ID)
		{
			foreach(SMArtist artist in artists)
			{
				if (artist.ID == ID)
				{
					return artist;
				}
			}
			return null;
		}

		// Получение ссылки на объект трека по идентификатору
		public SMTrack GetTrackByID(long ID)
		{
			foreach(SMArtist artist in artists)
			{
				foreach(SMTrack track in artist.Tracks)
				{
					if (track.ID == ID)
						return track;
				}
			}
			return null;
		}

		// Проверка наличия трека в группах выступлений
		public bool IsTrackAppliedInShow(long ID)
		{
			foreach(SMGroup showGroup in groupsShow)
			{
				foreach(long itemID in showGroup.IDList)
				{
					if (itemID == ID)
						return true;
				}
			}
			return false;
		}

		// Удаление артиста во всех группах репетиций и выступлений
		public void RemoveArtist(long id)
		{
			// Почистим выступления
			SMArtist artist = GetArtistByID(id);
			if (artist != null)
			{
				foreach(SMTrack track in artist.Tracks)
				{
					foreach(SMGroup sGroup in groupsShow)
					{
						sGroup.Remove(track.ID);
					}
				}
			}
			// Почистим репетиции
			foreach(SMGroup pGroup in groupsPrepare)
			{
				pGroup.Remove(id);
			}
		}

		// Удаление трека во всех группах выступлений
		public void RemoveTrack(long id)
		{
			foreach(SMGroup sGroup in groupsShow)
			{
				sGroup.Remove(id);
			}
		}
		// Файловые операции
		public void Save(BinaryWriter bw)
		{
			try
			{
				bw.Write(Artists.Count);
				foreach(SMArtist artist in Artists)
				{
					artist.Save(bw);
				}

				SaveList(bw, GroupsArtist);
				SaveList(bw, GroupsShow);
				SaveList(bw, GroupsPrepare);

				bw.Write(TrackFolderPath);
				bw.Write(Name);
			}
			catch (IOException ioex)
			{
				System.Console.WriteLine(ioex.Message);
			}
		}

		public void Load(BinaryReader br)
		{
			try
			{
				Artists.Clear();
				var aCount = br.ReadInt32();
				for (var n = 0; n < aCount; n++)
				{
					var newArtist = new SMArtist(this, gentres, null);
					newArtist.Load(br);
					Artists.Add(newArtist);
				}

				LoadList(br, GroupsArtist);
				LoadList(br, GroupsShow);
				LoadList(br, GroupsPrepare);

				TrackFolderPath = br.ReadString();
				Name = br.ReadString();
			}
			catch (IOException ioex)
			{
				System.Console.WriteLine(ioex.Message);
			}
		}

		private void SaveList(BinaryWriter bw, List<SMGroup> list)
		{
			try
			{
				bw.Write(list.Count);
				foreach(SMGroup group in list)
				{
					group.Save(bw);
				}
			}
			catch (IOException ioex)
			{
				System.Console.WriteLine(ioex.Message);
			}
		}
		private void LoadList(BinaryReader br, List<SMGroup> list)
		{
			try
			{
				list.Clear();
				var iCount = br.ReadInt32();
				for (var n = 0; n < iCount; n++)
				{
					var newItem = new SMGroup("");
					newItem.Load(br);
					list.Add(newItem);
				}
			}
			catch (IOException ioex)
			{
				System.Console.WriteLine(ioex.Message);
			}
		}
	}
}
