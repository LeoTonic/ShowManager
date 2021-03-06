﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ShowManager.Controls;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ShowManager.Models;

namespace ShowManager
{
	/// <summary>
	/// Логика взаимодействия для Artist.xaml
	/// </summary>
	public partial class Artist : Window, ICommandCatcher
	{
		public SMArtist ArtistObj
		{
			get
			{
				return this.artist;
			}
		}

		private SMArtist artist;
		private SMGentresBase gentresBase;

		public Artist(SMProject project, SMGentresBase gb, SMArtist fromArtist)
		{
			InitializeComponent();
			TracksToolbar.SetValues("item-add", "Добавить произведение", "item-edit", "Изменить произведение", "item-delete", "Удалить произведение", this);
			TracksToolbar.HideRecycle();

			gentresBase = gb;
			artist = new SMArtist(project, gb, fromArtist);

			this.DataContext = artist;
			tracksView.ItemsSource = artist.Tracks;

			// Заливка непривязанных элементов
			PrepareTimeStart.Value = artist.PrepareTimeStart;
			PrepareTimeFinish.Value = artist.PrepareTimeFinish;
			PrepareTimeLength.Value = artist.PrepareTimeLength;

			// Формирование групп жанров
			GentreGroup.Items.Clear();
			foreach (SMGentre gentre in gentresBase.GentreGroups)
			{
				var newItem = new ComboBoxItem() { Content = gentre.Name, Tag = gentre.ID };
				GentreGroup.Items.Add(newItem);
			}
			if (GentreGroup.Items.Count > 0)
			{
				GentreGroup.SelectedIndex = GetComboItemIndex(artist.GentreGroup, GentreGroup.Items);
			}
		}

		// Получение индекса элемента по идентификатору в тэге
		private int GetComboItemIndex(long id, ItemCollection items)
		{
			for (int n = 0; n < items.Count; n++)
			{
				var ci = items[n] as ComboBoxItem;
				if (GetComboItemID(ci) == id)
				{
					return n;
				}
			}
			return 0;
		}

		// Получение идентификатора по элементу
		private long GetComboItemID(ComboBoxItem cbi)
		{
			if (cbi == null)
				return -1;
			else
				return (long)cbi.Tag;
		}

		public void ToolBarAdd(SMToolbar tb)
		{
			var tDlg = new TrackDialog(artist, null);
			if (tDlg.ShowDialog() == true)
			{
				var newTrack = new SMTrack(artist);
				newTrack.Assign(tDlg.Track);
				artist.Tracks.Add(newTrack);
			}
		}
		public void ToolBarEdit(SMToolbar tb)
		{
			var selItem = tracksView.SelectedItem as SMTrack;
			if (selItem == null)
				return;
			var tDlg = new TrackDialog(artist, selItem);
			if (tDlg.ShowDialog() == true)
			{
				// Ищем трек
				foreach (SMTrack getTrack in artist.Tracks)
				{
					if (getTrack.ID == selItem.ID)
					{
						// Обновляем его
						getTrack.Assign(tDlg.Track);
						tracksView.Items.Refresh();
						return;
					}
				}
			}
		}
		public void ToolBarRemove(SMToolbar tb)
		{
			var selItem = tracksView.SelectedItem as SMTrack;
			if (selItem == null)
				return;
			if (MessageBox.Show("Удаляем композицию?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				for (int n = 0; n < artist.Tracks.Count; n++)
				{
					if (artist.Tracks[n].ID == selItem.ID)
					{
						artist.Tracks.RemoveAt(n);
						return;
					}
				} 
			}
		}

		public void ItemSelectionChange(Object parentControl) { }
		public void ItemDoubleClick(Object parentControl, SMListViewItem selectedItem) { }
		public void PanelGroupClick(string groupName) { }
		public void PanelGroupAdd(string groupName) { }
		public void PanelGroupRename(string groupNameOld, string groupNameNew) { }
		public void PanelGroupDelete(string groupName) { }
		public void DropItems(int insertIndex, SMListViewItem draggedItem, Object draggedTo, Object draggedToSubItem) { }

		// Группа жанра изменение элемента
		private void GentreGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// Определим жанровую группу
			var cb = sender as ComboBox;
			var gID = GetComboItemID((ComboBoxItem)cb.SelectedItem);
			if (gID == -1)
			{
				System.Console.WriteLine("Gentre group selection failed");
				return;
			}

			var gentre = gentresBase.GetGentreItem(gID);

			// Запомним для артиста
			artist.GentreGroup = gID;

			// Установим подробный жанр
			GentreClass.Items.Clear();
			foreach (SMElement gentreType in gentre.Gentres)
			{
				var newItem = new ComboBoxItem() { Content = gentreType.Name, Tag = gentreType.ID };
				GentreClass.Items.Add(newItem);
			}
			if (GentreClass.Items.Count > 0)
			{
				GentreClass.SelectedIndex = GetComboItemIndex(artist.GentreClass, GentreClass.Items);
			}

			// Направление
			GentreDirection.Items.Clear();
			foreach (SMElement gentreDir in gentre.Directions)
			{
				var newItem = new ComboBoxItem() { Content = gentreDir.Name, Tag = gentreDir.ID };
				GentreDirection.Items.Add(newItem);
			}
			if (GentreDirection.Items.Count > 0)
			{
				GentreDirection.SelectedIndex = GetComboItemIndex(artist.GentreDirection, GentreDirection.Items);
			}

			// Возраст
			GentreAge.Items.Clear();
			foreach (SMElement gentreAge in gentre.Ages)
			{
				var newItem = new ComboBoxItem() { Content = gentreAge.Name, Tag = gentreAge.ID };
				GentreAge.Items.Add(newItem);
			}
			if (GentreAge.Items.Count > 0)
			{
				GentreAge.SelectedIndex = GetComboItemIndex(artist.GentreAge, GentreAge.Items);
			}

			// Содержание
			GentreContent.Items.Clear();
			foreach (SMElement gentreCnt in gentre.Contents)
			{
				var newItem = new ComboBoxItem() { Content = gentreCnt.Name, Tag = gentreCnt.ID };
				GentreContent.Items.Add(newItem);
			}
			if (GentreContent.Items.Count > 0)
			{
				GentreContent.SelectedIndex = GetComboItemIndex(artist.GentreContent, GentreContent.Items);
			}

			// Категория
			GentreCategory.Items.Clear();
			foreach (SMElement gentreCat in gentre.Categories)
			{
				var newItem = new ComboBoxItem() { Content = gentreCat.Name, Tag = gentreCat.ID };
				GentreCategory.Items.Add(newItem);
			}
			if (GentreCategory.Items.Count > 0)
			{
				GentreCategory.SelectedIndex = GetComboItemIndex(artist.GentreCategory, GentreCategory.Items);
			}

		}

		private void GentreClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// Определим жанр
			var cb = sender as ComboBox;
			var gID = GetComboItemID((ComboBoxItem)cb.SelectedItem);
			if (gID == -1)
				return;
			// Запомним для артиста
			artist.GentreClass = gID;
		}

		private void GentreDirection_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// Определим направление
			var cb = sender as ComboBox;
			var gID = GetComboItemID((ComboBoxItem)cb.SelectedItem);
			if (gID == -1)
				return;
			// Запомним для артиста
			artist.GentreDirection = gID;
		}

		private void GentreContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// Определим состав
			var cb = sender as ComboBox;
			var gID = GetComboItemID((ComboBoxItem)cb.SelectedItem);
			if (gID == -1)
				return;
			// Запомним для артиста
			artist.GentreContent = gID;
		}

		private void GentreAge_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// Определим возрастную группу
			var cb = sender as ComboBox;
			var gID = GetComboItemID((ComboBoxItem)cb.SelectedItem);
			if (gID == -1)
				return;
			// Запомним для артиста
			artist.GentreAge = gID;
		}

		private void GentreCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// Определим категорию
			var cb = sender as ComboBox;
			var gID = GetComboItemID((ComboBoxItem)cb.SelectedItem);
			if (gID == -1)
				return;
			// Запомним для артиста
			artist.GentreCategory = gID;
		}

		private void SaveArtist_Click(object sender, RoutedEventArgs e)
		{
			// Проверка данных полей
			if (string.IsNullOrWhiteSpace(artist.Name))
			{
				MessageBox.Show("Не указано имя артиста/коллектива!", "Пустое поле", MessageBoxButton.OK, MessageBoxImage.Information);
				ArtistTab.IsSelected = true;
				TBArtistName.Focus();
				return;
			}

			// Заливка непривязанных элементов и жанров
			artist.PrepareTimeStart = PrepareTimeStart.Value;
			artist.PrepareTimeFinish = PrepareTimeFinish.Value;
			artist.PrepareTimeLength = PrepareTimeLength.Value;
			if (GentreGroup.SelectedItem != null)
			{
				artist.GentreGroup = GetComboItemID((ComboBoxItem)GentreGroup.SelectedItem);
			}
			if (GentreClass.SelectedItem != null)
			{
				artist.GentreClass = GetComboItemID((ComboBoxItem)GentreClass.SelectedItem);
			}
			if (GentreDirection.SelectedItem != null)
			{
				artist.GentreDirection = GetComboItemID((ComboBoxItem)GentreDirection.SelectedItem);
			}
			if (GentreContent.SelectedItem != null)
			{
				artist.GentreContent = GetComboItemID((ComboBoxItem)GentreContent.SelectedItem);
			}
			if (GentreAge.SelectedItem != null)
			{
				artist.GentreAge = GetComboItemID((ComboBoxItem)GentreAge.SelectedItem);
			}
			if (GentreCategory.SelectedItem != null)
			{
				artist.GentreCategory = GetComboItemID((ComboBoxItem)GentreCategory.SelectedItem);
			}
			DialogResult = true;
		}
	}
}
