using System;
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

namespace ShowManager
{
	/// <summary>
	/// Логика взаимодействия для Artist.xaml
	/// </summary>
	public partial class Artist : Window, ICommandCatcher
	{
		private List<TrackItem> trackItems;
		public Artist()
		{
			InitializeComponent();
			TracksToolbar.SetValues("item-add", "Добавить произведение", "item-edit", "Изменить произведение", "item-delete", "Удалить произведение", this);
			TracksToolbar.HideRecycle();

			trackItems = new List<TrackItem>();
			tracksView.ItemsSource = trackItems;

			trackItems.Add(new TrackItem() { TrackName = "Кукушка", TrackLength = "00:03:00", TrackMusic = "Кукушка" });
			trackItems.Add(new TrackItem() { TrackName = "Кукушка2", TrackLength = "00:03:20", TrackMusic = "Кукушка2" });
		}

		public void ToolBarAdd(SMToolbar tb)
		{
			var tDlg = new TrackDialog();
			tDlg.ShowDialog();
		}
		public void ToolBarEdit(SMToolbar tb)
		{

		}
		public void ToolBarRemove(SMToolbar tb)
		{

		}

		public void ItemSelect(object parentControl, long itemID) { }
		public void PanelGroupClick(string groupName) { }
		public void PanelGroupAdd(string groupName) { }
		public void PanelGroupRename(string groupNameOld, string groupNameNew) { }
		public void PanelGroupDelete(string groupName) { }
		public void DropItems(int insertIndex, SMListViewItem draggedItem, Object draggedTo) { }

		// Элемент в списке произведений раздел "программа"
		public class TrackItem
		{
			public string TrackName { get; set; }
			public string TrackLength { get; set; }
			public string TrackMusic { get; set; }
		}
	}
}
