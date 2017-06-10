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
				getGroup.Name = groupNameNew;
		}
		public void PanelGroupDelete(string groupName)
		{
			SMGroup getGroup = GetGroup(groupName);
			if (getGroup != null)
			{
				getGroup.Clear();
				if (viewMode == SMListView.ViewMode.OrderTrack)
					currentProject.GroupsShow.Remove(getGroup);
				else
					currentProject.GroupsPrepare.Remove(getGroup);
			}
		}

		public void DropItems(int insertIndex, SMListViewItem draggedItem, Object draggedTo, Object draggedToSubItem)
		{
			var lView = draggedItem.dragFromControl as SMListView;
			var items = lView.Items as ObservableCollection<SMListViewItem>;
			SMGroup getGroup = GetGroup(selectedPanelName);

			// Обработка переноса внутри контрола
			if (lView.GetHashCode() == draggedTo.GetHashCode())
			{
				// Перемещение внутри контрола
				SMListView.CreateHelperList(draggedItem.selectedItems, helpList1, true);
				SMListView.CreateHelperList(draggedItem.selectedItems, helpList2, false);
				foreach (SMListViewItem lvi in helpList2)
				{
					items.Remove(lvi);
				}

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
			}

			// Работаем в зависимости от режима
			if (viewMode == SMListView.ViewMode.OrderTrack)
			{
				// Перенос из вьюшки с артистом - будем вставлять все треки артиста
				if (lView.GetHashCode() == parentWindow.ArtistView.GetHashCode())
				{
					SMListView.CreateHelperList(draggedItem.selectedItems, helpList2, false);

					foreach (SMListViewItem lvi in helpList2)
					{
						var getArtist = currentProject.GetArtistByID(lvi.ItemID);
						if (getArtist != null)
						{
							foreach (SMTrack track in getArtist.Tracks)
							{
								if (getGroup != null)
								{
									getGroup.Add(track.ID, insertIndex);
								}

								if (ShowOrderView.Add(track, gentres, insertIndex, true) == false)
								{
									MessageBox.Show("Нельзя добавить дважды один и тот же трек в одну группу!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
								}
							}
						}
					}
				}
				// Перенос из вьюшки с треками - вставляем то что тащим
				if (lView.GetHashCode() == parentWindow.TrackView.GetHashCode())
				{
					SMListView.CreateHelperList(draggedItem.selectedItems, helpList2, false);
					foreach (SMListViewItem lvi in helpList2)
					{
						var getTrack = currentProject.GetTrackByID(lvi.ItemID);
						if (getTrack != null)
						{
							if (getGroup != null)
							{
								getGroup.Add(getTrack.ID, insertIndex);
							}

							if (ShowOrderView.Add(getTrack, gentres, insertIndex, true) == false)
							{
								MessageBox.Show("Нельзя добавить дважды один и тот же трек в одну группу!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
							}
						}
					}
				}
			}
			else
			{
				// Перенос из вьюшки с артистом - вставляем в репетицию
				if (lView.GetHashCode() == parentWindow.ArtistView.GetHashCode())
				{
					SMListView.CreateHelperList(draggedItem.selectedItems, helpList2, false);

					foreach (SMListViewItem lvi in helpList2)
					{
						var getArtist = currentProject.GetArtistByID(lvi.ItemID);
						if (getArtist != null)
						{
							if (getGroup != null)
							{
								getGroup.Add(getArtist.ID, insertIndex);
							}

							if (ShowOrderView.Add(getArtist, gentres, insertIndex, true) == false)
							{
								MessageBox.Show("Нельзя добавить дважды одного и того же исполнителя в одну группу!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
							}
						}
					}
				}
				// Перенос из вьюшки с треками - вставляем артиста в репетицию
				if (lView.GetHashCode() == parentWindow.TrackView.GetHashCode())
				{
					SMListView.CreateHelperList(draggedItem.selectedItems, helpList2, false);
					foreach (SMListViewItem lvi in helpList2)
					{
						var getTrack = currentProject.GetTrackByID(lvi.ItemID);
						if (getTrack != null)
						{
							var getArtist = getTrack.ParentArtist;
							if (getGroup != null)
							{
								getGroup.Add(getArtist.ID, insertIndex);
							}
							ShowOrderView.Add(getArtist, gentres, insertIndex, true);
						}
					}

				}
			}


			// Перенос в панель заголовка
			if (draggedTo.GetHashCode() == ShowOrderPanel.GetHashCode())
			{
				var dropPanel = draggedToSubItem as SMGroupsItem;

				// Создаем помощников
				SMListView.CreateHelperList(draggedItem.selectedItems, helpList2, false);

				SMGroup newGroup = GetGroup(dropPanel.Text);

				// Удаляем из текущей панели и группы и добавляем в новую
				foreach (SMListViewItem lvi in helpList2)
				{
					if (getGroup != null)
					{
						getGroup.Remove(lvi.ItemID);
					}
					if (newGroup != null)
					{
						newGroup.Add(lvi.ItemID, -1);
					}
					items.Remove(lvi);

					// Если та же группа - вставляем для отображения
					if (getGroup.GetHashCode() == newGroup.GetHashCode())
					{
						items.Add(lvi);
					}
				}
			}
			if (getGroup != null)
			{
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
