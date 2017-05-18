using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		// Конструктор
		public SMProject()
		{
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
	}
}
