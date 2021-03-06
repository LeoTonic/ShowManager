﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ShowManager.Tools;

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

		// Ссылка на список жанров
		private SMGentresBase gentres;

		// Имя фестиваля
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

		// Версия проекта - нужна для правильного сохранения в файл
		private int version;
		public int Version
		{
			get
			{
				return version;
			}
			set
			{
				version = value;
			}
		}

    // Внесены правки
    private bool isdirty;
    public bool IsDirty
    {
      get
      {
        return isdirty;
      }
    }
    public void SetDirty(bool status = true)
    {
      isdirty = status;
    }

    // Путь к сохраненному проекту
    private string filePath;
    public string FilePath
    {
      get
      {
        return filePath;
      }
      set
      {
        filePath = value;
      }
    }

		// Конструктор
		public SMProject(SMGentresBase gbase)
		{
			Name = "Новый фестиваль";
			Version = 1;
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

      SetDirty(false);
      FilePath = "";
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
		public void IOSave(DataIO dio)
		{
			dio.WriteString(DataIO.IN_PROJECT);
			dio.WriteInt(Version);
			dio.WriteString(Name);
			dio.WriteString(DataIO.IN_ARRAY);
			foreach (SMArtist artist in Artists)
			{
				artist.IOSave(dio);
			}
			dio.WriteString(DataIO.OUT_ARRAY);

			SaveList(dio, GroupsArtist);
			SaveList(dio, GroupsShow);
			SaveList(dio, GroupsPrepare);

			dio.WriteString(TrackFolderPath);
			dio.WriteString(DataIO.OUT_PROJECT);
		}

		public bool IOLoad(DataIO dio)
		{
			if (dio.SeekTo(DataIO.IN_PROJECT) == 1)
			{
				Version = dio.ReadInt();
				Name = dio.ReadString();
				if (dio.SeekTo(DataIO.IN_ARRAY) == 1)
				{
					Artists.Clear();

					while (dio.SeekTo(DataIO.IN_ARTIST, DataIO.OUT_ARRAY) == 1)
					{
						var newArtist = new SMArtist(this, gentres, null);
						if (newArtist.IOLoad(dio))
						{
							Artists.Add(newArtist);
						}
					}
				}
				if (dio.SeekTo(DataIO.IN_ARRAY) == 1)
					LoadList(dio, GroupsArtist);
				if (dio.SeekTo(DataIO.IN_ARRAY) == 1)
					LoadList(dio, GroupsShow);
				if (dio.SeekTo(DataIO.IN_ARRAY) == 1)
					LoadList(dio, GroupsPrepare);
				TrackFolderPath = dio.ReadString();
        SetDirty(false);
			}
			return (dio.SeekTo(DataIO.OUT_PROJECT) == 1);
		}
		private void SaveList(DataIO dio, List<SMGroup> list)
		{
			dio.WriteString(DataIO.IN_ARRAY);
			foreach(SMGroup group in list)
			{
				group.IOSave(dio);
			}
			dio.WriteString(DataIO.OUT_ARRAY);
		}
		private void LoadList(DataIO dio, List<SMGroup> list)
		{
			list.Clear();
			while (dio.SeekTo(DataIO.IN_GROUP, DataIO.OUT_ARRAY) == 1)
			{
				var newGroup = new SMGroup("");
				if (newGroup.IOLoad(dio))
				{
					list.Add(newGroup);
				}
			}
		}
	}
}
