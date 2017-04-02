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

			listStyle = new Style(typeof(ListBoxItem));
			listStyle.Setters.Add(new Setter(ListBoxItem.AllowDropProperty, true));
			listStyle.Setters.Add(new EventSetter(ListBoxItem.PreviewMouseRightButtonDownEvent, new MouseButtonEventHandler(SMListView_ItemPreviewMouseRightDown)));
			listStyle.Setters.Add(new EventSetter(ListBoxItem.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(SMListView_ItemMouseDown)));
			listStyle.Setters.Add(new EventSetter(ListBoxItem.PreviewMouseLeftButtonUpEvent, new MouseButtonEventHandler(SMListView_ItemMouseUp)));
			listStyle.Setters.Add(new EventSetter(ListBoxItem.GiveFeedbackEvent, new GiveFeedbackEventHandler(SMListView_GiveFeedback)));
			listStyle.Setters.Add(new EventSetter(ListBoxItem.DropEvent, new DragEventHandler(SMListViewItem_Drop)));
			ListViewControl.ItemContainerStyle = listStyle;

			queryHandler = new QueryContinueDragEventHandler(ItemQueryContinueDrag);

			ListViewControl.Drop += ListViewControl_Drop;
			ListViewControl.MouseLeftButtonDown += ListViewControl_MouseLeftButtonDown;
			ListViewControl.MouseLeftButtonUp += ListViewControl_MouseLeftButtonUp;
			ListViewControl.MouseMove += ListViewControl_MouseMove;

			DataContext = this;
		}

		private void ListViewControl_MouseMove(object sender, MouseEventArgs e)
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

		private void ListViewControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			isMouseLeft = true;
			selectPoint = e.GetPosition(ListViewControl);
			DragSelectionCanvas.Visibility = Visibility.Visible;
			UpdateDragSelectionRect(selectPoint, selectPoint);

			ListViewControl.CaptureMouse();
			e.Handled = true;
		}

		private void ListViewControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			isMouseLeft = false;
			DragSelectionCanvas.Visibility = Visibility.Collapsed;
			ListViewControl.ReleaseMouseCapture();
			e.Handled = true;
		}

		private void SMListView_ItemMouseDown(object sender, MouseButtonEventArgs e)
		{
			ListViewControl_MouseLeftButtonDown(sender, e);
		}
		private void SMListView_ItemMouseUp(object sender, MouseButtonEventArgs e)
		{
			ListViewControl_MouseLeftButtonUp(sender, e);
		}

		private void ListViewControl_Drop(object sender, DragEventArgs e)
		{
			EndDrag();
		}

		protected void SMListView_ItemPreviewMouseRightDown(object sender, MouseButtonEventArgs e)
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

		private void SMListView_GiveFeedback(object sender, GiveFeedbackEventArgs e)
		{
			if (ddRef != null)
			{
				ddRef.UpdatePosition();
			}
		}

		private void ItemQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
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
