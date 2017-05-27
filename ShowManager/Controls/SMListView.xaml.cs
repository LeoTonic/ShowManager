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
using ShowManager.Models;

namespace ShowManager.Controls
{
	/// <summary>
	/// Логика взаимодействия для SMListView.xaml
	/// </summary>
	public partial class SMListView : UserControl
	{
		// Режимы отображения
		public enum ViewMode
		{
			ArtistName = 0, // Режим отображения артиста
			ArtistTrack, // Трек у артиста
			OrderTrack, // Трек в режиме разметки выступления
			OrderArtist, // Артист в режиме разметки репетиции
			Gentre // Режим жанра
		}

		public ObservableCollection<SMListViewItem> Items { get; set; }

		public System.Collections.IList ItemsSelected;

		// Ссылка на интерфейс обработчика операций
		private ICommandCatcher iCommandTo = null;

		private ListBoxItem draggedItem = null;
		private DragDropWindow ddRef = null;
		private QueryContinueDragEventHandler queryHandler;
		private ViewMode viewMode;

		private bool isDragging = false;
		private Point dragPoint;
		private Vector dragVector;
		private bool isMouseLeft = false;

		private bool allowSelection = false;
		private Point selectPoint;
		private Rect selectRect = new Rect();

		private Point itemPoint;
		private Point nullPoint = new Point();
		private Rect itemRect = new Rect();

		public SMListView()
		{
			InitializeComponent();
			Items = new ObservableCollection<SMListViewItem>();

			queryHandler = new QueryContinueDragEventHandler(ItemQueryContinueDrag);
			DataContext = this;

			ItemsSelected = ListViewControl.SelectedItems;
		}

		// Установка режима отображения
		public void SetViewMode(ViewMode newMode, ICommandCatcher cTo)
		{
			iCommandTo = cTo;

			switch (newMode)
			{
				case ViewMode.ArtistName:
				case ViewMode.ArtistTrack:
				case ViewMode.OrderArtist:
				case ViewMode.OrderTrack:
					// Включаем выделение и мультирежим
					allowSelection = true;
					ListViewControl.SelectionMode = SelectionMode.Extended;
					break;
				case ViewMode.Gentre:
					// Выключаем выделение и мультирежим
					allowSelection = false;
					ListViewControl.SelectionMode = SelectionMode.Single;
					break;
			}
			viewMode = newMode;

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

			if (isMouseLeft && allowSelection)
			{
				Point curPoint = e.GetPosition(ListViewControl);
				UpdateDragSelectionRect(selectPoint, curPoint);
			}
		}

		protected void ListViewControl_MouseLeftDown(object sender, MouseButtonEventArgs e)
		{
			isMouseLeft = true;

			if (allowSelection)
			{
				selectPoint = e.GetPosition(ListViewControl);
				DragSelectionCanvas.Visibility = Visibility.Visible;
				UpdateDragSelectionRect(selectPoint, selectPoint);

				ListViewControl.Focus();
				ListViewControl.CaptureMouse();
			}

			e.Handled = true;
		}

		protected void ListViewControl_MouseLeftUp(object sender, MouseButtonEventArgs e)
		{
			isMouseLeft = false;
			if (allowSelection)
			{
				DragSelectionCanvas.Visibility = Visibility.Collapsed;
				ListViewControl.ReleaseMouseCapture();
			}
			e.Handled = true;
		}

		protected void ListViewControl_Drop(object sender, DragEventArgs e)
		{
			if (iCommandTo != null)
			{
				iCommandTo.DropItems(-1, e.Data.GetData(typeof(SMListViewItem)) as SMListViewItem, this, null);
			}

			EndDrag();
		}

		protected void SMListViewItem_Drop(object sender, DragEventArgs e)
		{
			if (iCommandTo != null)
			{
				var lbi = sender as ListBoxItem;
				var smItem = lbi.DataContext as SMListViewItem;
				int itemIndex = Items.IndexOf(smItem);
				iCommandTo.DropItems(itemIndex, e.Data.GetData(typeof(SMListViewItem)) as SMListViewItem, this, null);
			}

			e.Handled = true;
			EndDrag();
		}

		protected void SMListView_ItemMouseLeftDown(object sender, MouseButtonEventArgs e)
		{
			if (sender is ListBoxItem && iCommandTo != null)
			{
				var lbi = sender as ListBoxItem;
				var smi = lbi.DataContext as SMListViewItem;
				// Раньше здесь была отправка изменений выделения элемента
			}
		}

		protected void SMListView_ItemMouseRightDown(object sender, MouseButtonEventArgs e)
		{
			EndDrag();
			if (sender is ListBoxItem)
			{
				draggedItem = sender as ListBoxItem;
				dragPoint = e.GetPosition(null);
				var smItem = draggedItem.DataContext as SMListViewItem;
				smItem.selectedItems = ItemsSelected;
				smItem.dragFromControl = this;
			}
		}

		protected void SMListView_ItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (iCommandTo != null)
			{
				var lbItem = sender as ListBoxItem;
				var smItem = lbItem.DataContext as SMListViewItem;
				iCommandTo.ItemDoubleClick(this, smItem);
			}
		}

		private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (iCommandTo != null)
			{
				iCommandTo.ItemSelectionChange(this);
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

		// Обновление прямоугольника выделения
		private void UpdateDragSelectionRect(Point pt1, Point pt2)
		{
			selectRect.X = Math.Min(pt1.X, pt2.X);
			selectRect.Y = Math.Min(pt1.Y, pt2.Y);
			selectRect.Width = Math.Abs(pt1.X - pt2.X);
			selectRect.Height = Math.Abs(pt1.Y - pt2.Y);

			Size lvcSize = ListViewControl.RenderSize;
			
			if (selectRect.X + selectRect.Width > lvcSize.Width)
			{
				selectRect.Width -= ((selectRect.X + selectRect.Width) - lvcSize.Width);
			}
			if (selectRect.Y + selectRect.Height > lvcSize.Height)
			{
				selectRect.Height -= ((selectRect.Y + selectRect.Height) - lvcSize.Height);
			}

			if (selectRect.X < 0)
			{
				selectRect.Width += selectRect.X;
				selectRect.X = 0;
			}
			if (selectRect.Y < 0)
			{
				selectRect.Height += selectRect.Y;
				selectRect.Y = 0;
			}
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

		// Работа с элементами

    // Получение по ID
		public long GetFirstSelectedID()
		{
			if (ItemsSelected.Count == 0)
				return -1;

			var firstItem = ItemsSelected[0] as SMListViewItem;
			return firstItem.ItemID;
		}

		// Очистить список
		public void Clear()
		{
			Items.Clear();
		}

		// Добавить новый элемент
		public void Add(long itemID, int mainImgKey, string mainTimeText="", string subTimeText="", string labelText1="", string labelText2="", int ico0=101, int ico1=101, int ico2=101, int ico3=101)
		{
			SMListViewItem newItem = new SMListViewItem(viewMode, itemID)
			{
				MainImageKey = mainImgKey,
				MainTimeText = mainTimeText,
				SubTimeText = subTimeText,
				OneLineText = labelText1,
				TwoLineTopText = labelText1,
				TwoLineBotText = labelText2,
				Ico0Key = ico0,
				Ico1Key = ico1,
				Ico2Key = ico2,
				Ico3Key = ico3
			};

			Items.Add(newItem);
		}

		// Редактирование данных элемента
		public void Edit(long itemID, int mainImgKey, string mainTimeText = "", string subTimeText = "", string labelText1 = "", string labelText2 = "", int ico0=101, int ico1 = 101, int ico2 = 101, int ico3=101)
		{
			int itemIndex = GetItemIndexByID(itemID);
			if (itemIndex >= 0)
			{
				SMListViewItem item = Items[itemIndex];
				item.MainImageKey = mainImgKey;
				item.MainTimeText = mainTimeText;
				item.SubTimeText = subTimeText;
				item.OneLineText = labelText1;
				item.TwoLineTopText = labelText1;
				item.TwoLineBotText = labelText2;
				item.Ico0Key = ico0;
				item.Ico1Key = ico1;
				item.Ico2Key = ico2;
				item.Ico3Key = ico3;
			}
		}

		// Удалить элемент
		public void Remove(long itemID)
		{
			int itemIndex = GetItemIndexByID(itemID);
			if (itemIndex >= 0)
			{
				Items.RemoveAt(itemIndex);
			}
		}

		// Поиск элемента по ID
		private int GetItemIndexByID(long id)
		{
			for (int n = 0; n < Items.Count; n++)
			{
				if (Items[n].ItemID == id)
				{
					return n;
				}
			}
			return -1;
		}

		//Создание списка помощника
		// Перемещает элементы из одного списка в другой при обработке перетаскивания элементов
		static public void CreateHelperList(System.Collections.IList sourceList, List<SMListViewItem> destList, bool createItems)
		{
			destList.Clear();
			foreach (SMListViewItem lvi in sourceList)
			{
				if (createItems)
				{
					SMListViewItem newItem = new SMListViewItem(lvi);
					destList.Add(newItem);
				}
				else
				{
					destList.Add(lvi);
				}
			}
		}
	}
}
