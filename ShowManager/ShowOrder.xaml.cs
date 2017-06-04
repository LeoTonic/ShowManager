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

		// Имя текущей группы (в панели групп выступлений)
		private string selectedPanelName;

		private CultureInfo ci = new CultureInfo("en-US");

		// Списки для помощи при переносе итемов
		private List<SMListViewItem> helpList1 = new List<SMListViewItem>();
		private List<SMListViewItem> helpList2 = new List<SMListViewItem>();

		public ShowOrder(DragDropWindow wndDD, SMProject project, MainWindow parent)
		{
			currentProject = project;
			parentWindow = parent;

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
		}

		public void PanelGroupAdd(string groupName) {
			currentProject.GroupsShow.Add(new SMGroup(groupName));
		}

		public void PanelGroupRename(string groupNameOld, string groupNameNew) { }
		public void PanelGroupDelete(string groupName) { }
		public void DropItems(int insertIndex, SMListViewItem draggedItem, Object draggedTo, Object draggedToSubItem)
		{
			var lView = draggedItem.dragFromControl as SMListView;
			var items = lView.Items as ObservableCollection<SMListViewItem>;

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

				SMGroup getGroup = currentProject.GetGroup(SMProject.GroupType.Show, selectedPanelName);
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

			// Если элемент с которого тянем объекты не является artistView главного окна - выходим
			if (lView.GetHashCode() != parentWindow.ArtistView.GetHashCode())
				return;

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
			}
		}

		// Обновление бокса с временем
		private void UpdateTimeStart(TimeSpan value)
		{
			TimeStart.Text = value.ToString(@"hh\:mm\:ss", ci);
		}
	}
}
