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

		public ShowOrder(DragDropWindow wndDD, SMProject project, MainWindow parent, SMGentresBase gnt)
		{
			currentProject = project;
			parentWindow = parent;
			gentres = gnt;

			InitializeComponent();

			ShowOrderView.SetDragDropWindow(wndDD);
			ShowOrderView.SetViewMode(SMListView.ViewMode.OrderTrack, this);
			ShowOrderPanel.Initialize(null, this, false, currentProject.GroupsShow);

			this.Closed += ShowOrder_Closed;
		}
		private void ShowOrder_Closed(object sender, EventArgs e)
		{
			if (parentWindow != null)
			{
				parentWindow.isOrderShowActive = false;
				parentWindow.MenuWindowShow.IsChecked = false;
			}
		}

		public void ItemSelectionChange(Object parentControl) { }
		public void ItemDoubleClick(Object parentControl, SMListViewItem selectedItem) { }
		public void PanelGroupClick(string groupName) {
			selectedPanelName = groupName;
			var group = currentProject.GetGroup(SMProject.GroupType.Show, selectedPanelName);
			if (group == null)
				return;
			UpdateTimeStart(group.TimeStart);
			RefreshView();
		}

		public void PanelGroupAdd(string groupName) {
			currentProject.GroupsShow.Add(new SMGroup(groupName));
		}

		public void PanelGroupRename(string groupNameOld, string groupNameNew)
		{
			SMGroup getGroup = currentProject.GetGroup(SMProject.GroupType.Show, groupNameOld);
			if (getGroup != null)
				getGroup.Name = groupNameNew;
		}
		public void PanelGroupDelete(string groupName)
		{
			SMGroup getGroup = currentProject.GetGroup(SMProject.GroupType.Show, groupName);
			if (getGroup != null)
			{
				getGroup.Clear();
				currentProject.GroupsShow.Remove(getGroup);
			}
		}

		public void DropItems(int insertIndex, SMListViewItem draggedItem, Object draggedTo, Object draggedToSubItem)
		{
			var lView = draggedItem.dragFromControl as SMListView;
			var items = lView.Items as ObservableCollection<SMListViewItem>;
			SMGroup getGroup = currentProject.GetGroup(SMProject.GroupType.Show, selectedPanelName);

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

			// Перенос из вьюшки с артистом - будем вставлять все треки артиста
			if (lView.GetHashCode() == parentWindow.ArtistView.GetHashCode())
			{
				SMListView.CreateHelperList(draggedItem.selectedItems, helpList2, false);

				foreach (SMListViewItem lvi in helpList2)
				{
					var getArtist = currentProject.GetArtistByID(lvi.ItemID);
					if (getArtist != null)
					{
						foreach(SMTrack track in getArtist.Tracks)
						{
							if (getGroup != null)
							{
								getGroup.Add(track.ID, insertIndex);
							}

							ShowOrderView.Add(
								track.ID,
								gentres.GetImageKey(getArtist.GentreGroup, SMGentresBase.GentreClassType.GentreGroup, getArtist.GentreGroup),
								labelText1: track.Name,
								labelText2: getArtist.Name,
								ico0: gentres.GetImageKey(getArtist.GentreGroup, SMGentresBase.GentreClassType.Age, getArtist.GentreAge),
								ico1: gentres.GetImageKey(getArtist.GentreGroup, SMGentresBase.GentreClassType.Category, getArtist.GentreCategory),
								ico2: gentres.GetImageKey(getArtist.GentreGroup, SMGentresBase.GentreClassType.Content, getArtist.GentreContent),
								insIndex: insertIndex
								);

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

						var getArtist = getTrack.ParentArtist;
						ShowOrderView.Add(
							getTrack.ID,
							gentres.GetImageKey(getArtist.GentreGroup, SMGentresBase.GentreClassType.GentreGroup, getArtist.GentreGroup),
							labelText1: getTrack.Name,
							labelText2: getArtist.Name,
							ico0: gentres.GetImageKey(getArtist.GentreGroup, SMGentresBase.GentreClassType.Age, getArtist.GentreAge),
							ico1: gentres.GetImageKey(getArtist.GentreGroup, SMGentresBase.GentreClassType.Category, getArtist.GentreCategory),
							ico2: gentres.GetImageKey(getArtist.GentreGroup, SMGentresBase.GentreClassType.Content, getArtist.GentreContent),
							insIndex: insertIndex
							);
					}
				}
			}

			// Перенос в панель заголовка
			if (draggedTo.GetHashCode() == ShowOrderPanel.GetHashCode())
			{
				var dropPanel = draggedToSubItem as SMGroupsItem;

				// Создаем помощников
				SMListView.CreateHelperList(draggedItem.selectedItems, helpList2, false);

				SMGroup newGroup = currentProject.GetGroup(SMProject.GroupType.Show, dropPanel.Text);

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
			UpdateTimeLine();
		}
		public void ToolBarAdd(SMToolbar tb) { }
		public void ToolBarEdit(SMToolbar tb) { }
		public void ToolBarRemove(SMToolbar tb) { }

		private void TimeStart_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var group = currentProject.GetGroup(SMProject.GroupType.Show, selectedPanelName);
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
				UpdateTimeLine();
			}
		}

		// Обновление бокса с временем
		private void UpdateTimeStart(TimeSpan value)
		{
			TimeStart.Text = value.ToString(@"hh\:mm\:ss", ci);
		}

		// Обновление хронометража
		public void UpdateTimeLine()
		{
			var group = currentProject.GetGroup(SMProject.GroupType.Show, selectedPanelName);
			if (group == null)
				return;
			var timeCursor = group.TimeStart;
			foreach (long id in group.IDList)
			{
				var track = currentProject.GetTrackByID(id);
				if (track != null)
				{
					var getArtist = track.ParentArtist;
					ShowOrderView.Edit(
						track.ID,
						gentres.GetImageKey(getArtist.GentreGroup, SMGentresBase.GentreClassType.GentreGroup, getArtist.GentreGroup),
						timeCursor.ToString(@"hh\:mm\:ss", ci),
						track.TrackLength.ToString(@"hh\:mm\:ss", ci),
						labelText1: track.Name,
						labelText2: getArtist.Name,
						ico0: gentres.GetImageKey(getArtist.GentreGroup, SMGentresBase.GentreClassType.Age, getArtist.GentreAge),
						ico1: gentres.GetImageKey(getArtist.GentreGroup, SMGentresBase.GentreClassType.Category, getArtist.GentreCategory),
						ico2: gentres.GetImageKey(getArtist.GentreGroup, SMGentresBase.GentreClassType.Content, getArtist.GentreContent));
					timeCursor += track.TrackLength;
				}
			}
			TimeEnd.Text = timeCursor.ToString(@"hh\:mm\:ss", ci);
			TimeLength.Text = (timeCursor - group.TimeStart).ToString(@"hh\:mm\:ss", ci);
		}

		// Обновление отображения вьюшки
		private void RefreshView()
		{
			ShowOrderView.Clear();
			var group = currentProject.GetGroup(SMProject.GroupType.Show, selectedPanelName);
			if (group == null)
				return;

			foreach (long id in group.IDList)
			{
				var track = currentProject.GetTrackByID(id);
				if (track != null)
				{
					var getArtist = track.ParentArtist;
					ShowOrderView.Add(
						track.ID,
						gentres.GetImageKey(getArtist.GentreGroup, SMGentresBase.GentreClassType.GentreGroup, getArtist.GentreGroup),
						labelText1: track.Name,
						labelText2: getArtist.Name,
						ico0: gentres.GetImageKey(getArtist.GentreGroup, SMGentresBase.GentreClassType.Age, getArtist.GentreAge),
						ico1: gentres.GetImageKey(getArtist.GentreGroup, SMGentresBase.GentreClassType.Category, getArtist.GentreCategory),
						ico2: gentres.GetImageKey(getArtist.GentreGroup, SMGentresBase.GentreClassType.Content, getArtist.GentreContent));
				}
			}

			UpdateTimeLine();
		}
	}
}
