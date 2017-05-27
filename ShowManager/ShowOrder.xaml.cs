using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ShowManager.Controls;
using ShowManager.Models;

namespace ShowManager
{
	/// <summary>
	/// Логика взаимодействия для ShowOrder.xaml
	/// </summary>
	public partial class ShowOrder : Window, ICommandCatcher
	{
		private SMProject currentProject = null;
		private MainWindow parentWindow = null;

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
		public void PanelGroupClick(string groupName) { }
		public void PanelGroupAdd(string groupName) { }
		public void PanelGroupRename(string groupNameOld, string groupNameNew) { }
		public void PanelGroupDelete(string groupName) { }
		public void DropItems(int insertIndex, SMListViewItem draggedItem, Object draggedTo, Object draggedToSubItem) { }
		public void ToolBarAdd(SMToolbar tb) { }
		public void ToolBarEdit(SMToolbar tb) { }
		public void ToolBarRemove(SMToolbar tb) { }
	}
}
