using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ShowManager.Controls;
using ShowManager.Models;
using System.Globalization;

namespace ShowManager
{
	/// <summary>
	/// Логика взаимодействия для ShowOrder.xaml
	/// </summary>
	public partial class ShowOrder : Window, ICommandCatcher
	{
		private SMProject currentProject = null;
		private MainWindow parentWindow = null;
		private SMGentresBase gentres = null;

		// Имя текущей группы (в панели групп выступлений)
		public string selectedPanelName;

		private CultureInfo ci = new CultureInfo("en-US");

		// Списки для помощи при переносе итемов
		private List<SMListViewItem> helpList1 = new List<SMListViewItem>();
		private List<SMListViewItem> helpList2 = new List<SMListViewItem>();

		private SMListView.ViewMode viewMode;

		public ShowOrder(DragDropWindow wndDD, SMProject project, MainWindow parent, SMGentresBase gnt, SMListView.ViewMode mode)
		{
			currentProject = project;
			parentWindow = parent;
			gentres = gnt;
			viewMode = mode;

			InitializeComponent();

			ShowOrderView.SetDragDropWindow(wndDD);
			ShowOrderView.SetViewMode(viewMode, this);
			if (viewMode == SMListView.ViewMode.OrderTrack)
				ShowOrderPanel.Initialize(null, this, false, currentProject.GroupsShow);
			else
				ShowOrderPanel.Initialize(null, this, false, currentProject.GroupsPrepare);

			if (viewMode == SMListView.ViewMode.OrderTrack)
			{
				this.Title = "Порядок выступлений";
			}
			else if (viewMode == SMListView.ViewMode.OrderArtist)
			{
				this.Title = "Порядок репетиций";
			}

			this.Closed += ShowOrder_Closed;
		}

		private void ShowOrder_Closed(object sender, EventArgs e)
		{
			if (parentWindow != null)
			{
				if (viewMode == SMListView.ViewMode.OrderTrack)
				{
					parentWindow.isOrderShowActive = false;
					parentWindow.MenuWindowShow.IsChecked = false;
				}
				else
				{
					parentWindow.isOrderPrepActive = false;
					parentWindow.MenuWindowPrep.IsChecked = false;
				}
			}
		}

		public void ItemSelectionChange(Object parentControl) { }
		public void ItemDoubleClick(Object parentControl, SMListViewItem selectedItem) { }
		public void PanelGroupClick(string groupName) {
			selectedPanelName = groupName;
			SMGroup group = GetGroup(selectedPanelName);

			if (group == null)
				return;

			UpdateTimeStart(group.TimeStart);
			RefreshView();
		}

		public void PanelGroupAdd(string groupName) {
			if (viewMode == SMListView.ViewMode.OrderTrack)
				currentProject.GroupsShow.Add(new SMGroup(groupName));
			else
				currentProject.GroupsPrepare.Add(new SMGroup(groupName));
		}

		public void PanelGroupRename(string groupNameOld, string groupNameNew)
		{
			SMGroup getGroup = GetGroup(groupNameOld);

			if (getGroup != null)
			{
				getGroup.Name = groupNameNew;
				// Если текущая панель была выделена
				if (selectedPanelName == groupNameOld)
					selectedPanelName = groupNameNew;
			}
		}
		public void PanelGroupDelete(string groupName)
		{
			SMGroup getGroup = GetGroup(groupName);
			if (getGroup != null)
			{
				if (viewMode == SMListView.ViewMode.OrderTrack)
				{
					// Не забываем обновить статусы
					// Формируем вспомогательный список
					List<long> idList = new List<long>();
					getGroup.CloneList(idList);
					currentProject.GroupsShow.Remove(getGroup);
					foreach(long id in idList)
					{
						// Проверяем на наличие во всех группах
						if (!currentProject.IsTrackAppliedInShow(id))
						{
							// Если не обнаружен - убираем свойство наличия на выступлении
							SMTrack track = currentProject.GetTrackByID(id);
							if (track != null)
							{
								track.IsApplied = false;
								parentWindow.UpdateTrackStatus(track, track.ParentArtist);
							}
						}
					}
				}
				else if (viewMode == SMListView.ViewMode.OrderArtist)
					currentProject.GroupsPrepare.Remove(getGroup);
			}
		}

		// Добавление в группу
		// dragFromMode, dragToMode - OrderTrack,OrderArtist
		// если режимы равны - простой перенос
		private bool AddItemToGroup(SMGroup group, long itemID, int insertIndex, SMListView.ViewMode dragFromMode, SMListView.ViewMode dragToMode)
		{
			if (group == null)
				return false;
			if (dragToMode == dragFromMode)
			{
				// Добавляем переносом из соседней группы
				if (!group.IDList.Contains(itemID))
					group.Add(itemID, insertIndex);
			}
			else if (dragFromMode == SMListView.ViewMode.OrderArtist && dragToMode == SMListView.ViewMode.OrderTrack)
			{
				// Добавляем все треки артиста
				SMArtist getArtist = currentProject.GetArtistByID(itemID);
				if (getArtist == null)
					return false;
				foreach (SMTrack track in getArtist.Tracks)
				{
					if (!group.IDList.Contains(track.ID))
						group.Add(track.ID, insertIndex);
				}
			}
			else if (dragFromMode == SMListView.ViewMode.OrderTrack && dragToMode == SMListView.ViewMode.OrderArtist)
			{
				// Добавляем артиста
				SMTrack getTrack = currentProject.GetTrackByID(itemID);
				if (getTrack == null)
					return false;

				if (!group.IDList.Contains(getTrack.ParentArtist.ID))
					group.Add(getTrack.ParentArtist.ID, insertIndex);
			}

			return true;
		}

		// Добавление во вьюшку
		// dragFromMode, dragToMode - OrderTrack,OrderArtist
		// если режимы равны - простой перенос
		private bool AddItemToView(long itemID, int insertIndex, SMListView.ViewMode dragFromMode, SMListView.ViewMode dragToMode)
		{
			SMArtist artist = null;
			SMTrack track = null;

			if (dragFromMode == SMListView.ViewMode.OrderArtist)
				artist = currentProject.GetArtistByID(itemID);
			else if (dragFromMode == SMListView.ViewMode.OrderTrack)
				track = currentProject.GetTrackByID(itemID);

			if (dragToMode == dragFromMode)
			{
				if (dragFromMode == SMListView.ViewMode.OrderArtist)
				{
					// Добавляем как артиста
					if (artist != null)
						return ShowOrderView.Add(artist, gentres, insertIndex, true);
				}
				else if (dragFromMode == SMListView.ViewMode.OrderTrack)
				{
					// Добавляем как трек
					if (track != null)
						return ShowOrderView.Add(track, gentres, insertIndex, true);
				}
			}
			else if (dragFromMode == SMListView.ViewMode.OrderArtist && dragToMode == SMListView.ViewMode.OrderTrack)
			{
				// Добавляем все треки артиста
				if (artist != null)
				{
					foreach (SMTrack getTrack in artist.Tracks)
					{
						ShowOrderView.Add(getTrack, gentres, insertIndex, true);
					}
				}
			}
			else if (dragFromMode == SMListView.ViewMode.OrderTrack && dragToMode == SMListView.ViewMode.OrderArtist)
			{
				// Добавляем артиста
				if (track != null)
				{
					SMArtist getArtist = track.ParentArtist;
					return ShowOrderView.Add(getArtist, gentres, insertIndex, true);
				}
			}
			return false;
		}

		// Добавление статуса применения трека - в режиме добавки в выступление
		// dragFromMode
		// OrderTrack - обновляем трек
		// OrderArtist - обновляем все треки артиста
		private void UpdateAppliedStatusTracks(long itemID, SMListView.ViewMode dragFromMode)
		{
			if (viewMode != SMListView.ViewMode.OrderTrack)
				return;

			if (dragFromMode == SMListView.ViewMode.OrderArtist)
			{
				// Помечаем все треки артиста
				SMArtist getArtist = currentProject.GetArtistByID(itemID);
				if (getArtist == null)
					return;
				foreach(SMTrack track in getArtist.Tracks)
				{
					track.IsApplied = true;
					parentWindow.UpdateTrackStatus(track, getArtist);
				}
			}
			else if (dragFromMode == SMListView.ViewMode.OrderTrack)
			{
				// Помечаем трек
				SMTrack getTrack = currentProject.GetTrackByID(itemID);
				if (getTrack == null)
					return;
				getTrack.IsApplied = true;
				parentWindow.UpdateTrackStatus(getTrack, getTrack.ParentArtist);
			}
			return;
		}

		public void DropItems(int insertIndex, SMListViewItem draggedItem, Object draggedTo, Object draggedToSubItem)
		{
			var lView = draggedItem.dragFromControl as SMListView;
			var items = lView.Items as ObservableCollection<SMListViewItem>;
			SMListView.CreateHelperList(draggedItem.selectedItems, helpList2, false);

			// Обработка переноса внутри контрола
			if (lView.GetHashCode() == draggedTo.GetHashCode())
			{
				// Перемещение внутри контрола
				SMListView.CreateHelperList(draggedItem.selectedItems, helpList1, true);
				foreach (SMListViewItem lvi in helpList2)
				{
					items.Remove(lvi);
				}
				var getGroup = GetGroup(selectedPanelName);
				foreach (SMListViewItem lvi in helpList1)
				{
					if (getGroup != null)
					{
						getGroup.Move(lvi.ItemID, insertIndex);
					}

					if (insertIndex >= 0)
					{
						items.Insert(insertIndex, lvi);
					}
					else
					{
						items.Add(lvi);
					}
				}
				UpdateTimeLine(getGroup);
			}
			// Обработка сброса в панель заголовка из вьюшек другого окна
			else if (draggedTo.GetHashCode() == ShowOrderPanel.GetHashCode() && lView.GetHashCode() != ShowOrderView.GetHashCode())
			{
				SMListView.ViewMode dragFromMode;
				if (lView.GetHashCode() == parentWindow.TrackView.GetHashCode())
					dragFromMode = SMListView.ViewMode.OrderTrack;
				else if (lView.GetHashCode() == parentWindow.ArtistView.GetHashCode())
					dragFromMode = SMListView.ViewMode.OrderArtist;
				else
					return;

				foreach(SMListViewItem lvi in helpList2)
				{
					var panelItem = draggedToSubItem as SMGroupsItem;
					if (panelItem.Text != selectedPanelName)
					{
						// Кидаем в скрытую панель

						// Отправляем в группу
						SMGroup getGroup = GetGroup(panelItem.Text);
						if (AddItemToGroup(getGroup, lvi.ItemID, insertIndex, dragFromMode, viewMode))
						{
							UpdateAppliedStatusTracks(lvi.ItemID, dragFromMode);
						}
					}
					else
					{
						// Кидаем в открытую панель

						// Отправляем в группу и во вьюшку
						SMGroup getGroup = GetGroup(selectedPanelName);
						if (AddItemToGroup(getGroup, lvi.ItemID, insertIndex, dragFromMode, viewMode))
						{
							UpdateAppliedStatusTracks(lvi.ItemID, dragFromMode);
							// Во вьюшку
							AddItemToView(lvi.ItemID, insertIndex, dragFromMode, viewMode);
							UpdateTimeLine(getGroup);
						}
					}
				}
			}
			// Сброс из текущей вьюшки в другую группу
			else if (draggedTo.GetHashCode() == ShowOrderPanel.GetHashCode() && lView.GetHashCode() == ShowOrderView.GetHashCode())
			{
				var dropPanel = draggedToSubItem as SMGroupsItem;
				SMGroup newGroup = GetGroup(dropPanel.Text);
				SMGroup getGroup = GetGroup(selectedPanelName);

				if (dropPanel.Text == selectedPanelName)
					return;

				// Удаляем из текущей панели и группы и добавляем в новую
				foreach (SMListViewItem lvi in helpList2)
				{
					if (getGroup != null)
						getGroup.Remove(lvi.ItemID);

					if (newGroup != null)
						AddItemToGroup(newGroup, lvi.ItemID, -1, viewMode, viewMode);

					items.Remove(lvi);
				}
			}
			// Кидаем из другого окна во вьюшку и в группу
			else
			{
				SMListView.ViewMode dragFromMode;
				if (lView.GetHashCode() == parentWindow.TrackView.GetHashCode())
					dragFromMode = SMListView.ViewMode.OrderTrack;
				else if (lView.GetHashCode() == parentWindow.ArtistView.GetHashCode())
					dragFromMode = SMListView.ViewMode.OrderArtist;
				else
					return;

				SMGroup getGroup = GetGroup(selectedPanelName);
				if (getGroup == null)
					return;

				foreach (SMListViewItem lvi in helpList2)
				{

					if (AddItemToGroup(getGroup, lvi.ItemID, insertIndex, dragFromMode, viewMode))
					{
						UpdateAppliedStatusTracks(lvi.ItemID, dragFromMode);
						// Во вьюшку
						AddItemToView(lvi.ItemID, insertIndex, dragFromMode, viewMode);
					}
				}
				UpdateTimeLine(getGroup);
			}
		}
		public void ToolBarAdd(SMToolbar tb) { }
		public void ToolBarEdit(SMToolbar tb) { }
		public void ToolBarRemove(SMToolbar tb) { }

		private void TimeStart_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var group = GetGroup(selectedPanelName);
			if (group == null)
				return;

			Point pnt = TimeStart.PointToScreen(new Point(0, 0));
			var tDlg = new TimeDialog(group.TimeStart)
			{
				Left = pnt.X + 8,
				Top = pnt.Y + 8
			};
			if (tDlg.ShowDialog() == true)
			{
				group.TimeStart = tDlg.TimeValue;
				UpdateTimeStart(tDlg.TimeValue);
				UpdateTimeLine(group);
			}
		}

		// Обновление бокса с временем
		private void UpdateTimeStart(TimeSpan value)
		{
			TimeStart.Text = value.ToString(@"hh\:mm\:ss", ci);
		}

		// Обновление хронометража
		public void UpdateTimeLine(SMGroup group)
		{
			if (group == null)
				return;

			var timeCursor = group.TimeStart;
			foreach (long id in group.IDList)
			{
				if (viewMode == SMListView.ViewMode.OrderTrack)
				{
					var track = currentProject.GetTrackByID(id);
					if (track != null)
					{
						ShowOrderView.EditMainTime(track.ID, timeCursor);
						timeCursor += track.TrackLength;
					}
				}
				else
				{
					var artist = currentProject.GetArtistByID(id);
					if (artist != null)
					{
						ShowOrderView.EditMainTime(artist.ID, timeCursor);
						timeCursor += (artist.PrepareTimeStart + artist.PrepareTimeLength + artist.PrepareTimeFinish);
					}
				}

			}
			TimeEnd.Text = timeCursor.ToString(@"hh\:mm\:ss", ci);
			TimeLength.Text = (timeCursor - group.TimeStart).ToString(@"hh\:mm\:ss", ci);
		}

		// Обновление отображения вьюшки
		public void RefreshView()
		{
			ShowOrderView.Clear();
			var group = GetGroup(selectedPanelName);
			if (group == null)
				return;

			foreach (long id in group.IDList)
			{
				if (viewMode == SMListView.ViewMode.OrderTrack)
				{
					var track = currentProject.GetTrackByID(id);
					if (track != null)
					{
						ShowOrderView.Add(track, gentres);
					}
				}
				else
				{
					var artist = currentProject.GetArtistByID(id);
					if (artist != null)
					{
						ShowOrderView.Add(artist, gentres);
					}
				}
			}

			UpdateTimeLine(group);
		}

		// Получение ссылки на группу
		private SMGroup GetGroup(string groupName)
		{
			if (viewMode == SMListView.ViewMode.OrderTrack)
				return currentProject.GetGroup(SMProject.GroupType.Show, groupName);
			else
				return currentProject.GetGroup(SMProject.GroupType.Prepare, groupName);
		}
	}
}
