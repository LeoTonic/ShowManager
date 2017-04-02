using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShowManager.Controls
{
	/// <summary>
	/// Логика взаимодействия для SMListView.xaml
	/// </summary>
	public partial class SMListView : UserControl
	{
		public ObservableCollection<SMListViewItem> Items { get; set; }
		private ListBoxItem draggedItem = null;
		private DragDropWindow ddRef = null;
		private readonly Style listStyle = null;

		public SMListView()
		{
			InitializeComponent();
			Items = new ObservableCollection<SMListViewItem>(new List<SMListViewItem>
			{
				new SMListViewItem { Text = "Task #01", Age = 234, Icon = GentreIcon.Vocal },
				new SMListViewItem { Text = "Task #02", Age = 160, Icon = GentreIcon.Dance },
				new SMListViewItem { Text = "Task #03", Age = 123, Icon = GentreIcon.Vocal },
				new SMListViewItem { Text = "Task #04", Age = 112, Icon = GentreIcon.Vocal },
				new SMListViewItem { Text = "Task #05", Age = 96, Icon = GentreIcon.Dance },
			});

			listStyle = new Style(typeof(ListBoxItem));
			listStyle.Setters.Add(new Setter(ListBoxItem.AllowDropProperty, true));
			listStyle.Setters.Add(new EventSetter(ListBoxItem.PreviewMouseRightButtonDownEvent, new MouseButtonEventHandler(SMListView_ItemPreviewMouseRightDown)));
			listStyle.Setters.Add(new EventSetter(ListBoxItem.GiveFeedbackEvent, new GiveFeedbackEventHandler(SMListView_GiveFeedback)));
			listStyle.Setters.Add(new EventSetter(ListBoxItem.DropEvent, new DragEventHandler(SMListViewItem_Drop)));
			ListViewControl.ItemContainerStyle = listStyle;
			DataContext = this;
		}

		protected void SMListView_ItemPreviewMouseRightDown(object sender, MouseButtonEventArgs e)
		{
			EndDrag();

			if (sender is ListBoxItem)
			{
				draggedItem = sender as ListBoxItem;
				//var card = draggedItem.DataContext as SMListViewItem;

				//card.fromBoxControl = this;
				//card.fromBoxIndex = Items.IndexOf(card);
				//draggedItem.IsSelected = true;

				// create the visual feedback drag and drop item
				if (ddRef != null)
				{
					ddRef.ShowWindow();
				}

				DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
			}
		}

		public void SetDragDropWindow(DragDropWindow from)
		{
			ddRef = from;
		}

		private void SMListView_GiveFeedback(object sender, GiveFeedbackEventArgs e)
		{
			if (ddRef != null)
			{
				ddRef.UpdatePosition();
			}
		}

		protected void SMListViewItem_Drop(object sender, DragEventArgs e)
		{
			e.Handled = true;
			EndDrag();
		}

		private void EndDrag()
		{
			if (draggedItem != null)
			{
				draggedItem = null;
			}

			if (ddRef != null)
			{
				ddRef.HideWindow();
			}
		}
	}
}
