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
		private QueryContinueDragEventHandler queryHandler;

		private bool isDragging = false;
		private Point dragPoint;
		private Vector dragVector;
		private bool isMouseLeft = false;

		private Point selectPoint;
		private Rect selectRect = new Rect();

		private Point itemPoint;
		private Point nullPoint = new Point();
		private Rect itemRect = new Rect();

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

			queryHandler = new QueryContinueDragEventHandler(ItemQueryContinueDrag);
			DataContext = this;
		}

		protected void ListViewControl_MouseMove(object sender, MouseEventArgs e)
		{
			dragVector = dragPoint - e.GetPosition(null);
			if ((Math.Abs(dragVector.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(dragVector.Y) > SystemParameters.MinimumVerticalDragDistance) && !isDragging && draggedItem != null)
			{
				if (ddRef != null)
				{
					ddRef.ShowWindow();
				}
				draggedItem.QueryContinueDrag += queryHandler;
				DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
				isDragging = true;
			}

			if (isMouseLeft)
			{
				Point curPoint = e.GetPosition(ListViewControl);
				UpdateDragSelectionRect(selectPoint, curPoint);
			}
		}

		protected void ListViewControl_MouseLeftDown(object sender, MouseButtonEventArgs e)
		{
			isMouseLeft = true;
			selectPoint = e.GetPosition(ListViewControl);
			DragSelectionCanvas.Visibility = Visibility.Visible;
			UpdateDragSelectionRect(selectPoint, selectPoint);

			ListViewControl.Focus();
			ListViewControl.CaptureMouse();
			e.Handled = true;
		}

		protected void ListViewControl_MouseLeftUp(object sender, MouseButtonEventArgs e)
		{
			isMouseLeft = false;
			DragSelectionCanvas.Visibility = Visibility.Collapsed;
			ListViewControl.ReleaseMouseCapture();
			e.Handled = true;
		}

		protected void ListViewControl_Drop(object sender, DragEventArgs e)
		{
			EndDrag();
		}

		protected void SMListView_ItemMouseRightDown(object sender, MouseButtonEventArgs e)
		{
			EndDrag();
			if (sender is ListBoxItem)
			{
				draggedItem = sender as ListBoxItem;
				dragPoint = e.GetPosition(null);
				//var card = draggedItem.DataContext as SMListViewItem;

				//card.fromBoxControl = this;
				//card.fromBoxIndex = Items.IndexOf(card);
				//draggedItem.IsSelected = true;
				// create the visual feedback drag and drop item
			}
		}

		public void SetDragDropWindow(DragDropWindow from)
		{
			ddRef = from;
		}

		protected void SMListView_GiveFeedback(object sender, GiveFeedbackEventArgs e)
		{
			if (ddRef != null)
			{
				ddRef.UpdatePosition();
			}
		}

		protected void ItemQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
		{
			if (e.KeyStates == DragDropKeyStates.None)
			{
				if (draggedItem != null)
				{
					draggedItem.QueryContinueDrag -= queryHandler;
				}
				EndDrag();
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
			isDragging = false;
		}

		private void UpdateDragSelectionRect(Point pt1, Point pt2)
		{
			selectRect.X = Math.Min(pt1.X, pt2.X);
			selectRect.Y = Math.Min(pt1.Y, pt2.Y);
			selectRect.Width = Math.Abs(pt1.X - pt2.X);
			selectRect.Height = Math.Abs(pt1.Y - pt2.Y);

			Canvas.SetLeft(DragSelectionBorder, selectRect.X);
			Canvas.SetTop(DragSelectionBorder, selectRect.Y);
			DragSelectionBorder.Width = selectRect.Width;
			DragSelectionBorder.Height = selectRect.Height;

			// Выборка элементов
			foreach(object lvi in ListViewControl.Items)
			{
				var smi = lvi as SMListViewItem;
				itemPoint = smi.TranslatePoint(nullPoint, ListViewControl);
				itemRect.X = itemPoint.X;
				itemRect.Y = itemPoint.Y;
				itemRect.Width = smi.ActualWidth;
				itemRect.Height = smi.ActualHeight;

				if (selectRect.IntersectsWith(itemRect))
				{
					if (!ListViewControl.SelectedItems.Contains(lvi))
					{
						ListViewControl.SelectedItems.Add(lvi);
					}
				}
				else
				{
					if (ListViewControl.SelectedItems.Contains(lvi))
					{
						ListViewControl.SelectedItems.Remove(lvi);
					}
				}
			}
		}

	}
}
