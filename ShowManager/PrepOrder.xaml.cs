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
	/// Логика взаимодействия для PrepOrder.xaml
	/// </summary>
	public partial class PrepOrder : Window, ICommandCatcher
	{
		private SMProject currentProject = null;
		private MainWindow parentWindow = null;

		public PrepOrder(DragDropWindow wndDD, SMProject project, MainWindow parent)
		{
			currentProject = project;
			parentWindow = parent;

			InitializeComponent();

			PrepOrderView.SetDragDropWindow(wndDD);
			PrepOrderView.SetViewMode(SMListView.ViewMode.OrderArtist, this);
			PrepOrderPanel.Initialize(null, this, false, currentProject.GroupsPrepare);

			this.Closed += PrepOrder_Closed;
		}

		private void PrepOrder_Closed(object sender, EventArgs e)
		{
			if (parentWindow != null)
			{
				parentWindow.isOrderPrepActive = false;
				parentWindow.MenuWindowPrep.IsChecked = false;
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
