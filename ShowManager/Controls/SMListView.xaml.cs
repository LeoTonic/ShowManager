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
using System.Globalization;

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

		private CultureInfo ci = new CultureInfo("en-US");

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

		// Добавить новый элемент - режим для списка артистов и репетиций
		public bool Add(SMArtist artist, SMGentresBase gentres, int insertIndex = -1, bool checkExist = false)
		{
			if (checkExist && GetItemIndexByID(artist.ID) >= 0)
			{
				return false;
			}

			var timeLen = artist.PrepareTimeStart + artist.PrepareTimeLength + artist.PrepareTimeFinish;
			SMListViewItem newItem = new SMListViewItem(viewMode, artist.ID)
			{
				MainImageKey = gentres.GetImageKey(artist.GentreGroup, SMGentresBase.GentreClassType.GentreGroup, artist.GentreGroup),
				MainTimeText = "",
				SubTimeText = timeLen.ToString(@"hh\:mm\:ss", ci),
				OneLineText = "",
				TwoLineTopText = artist.Name,
				TwoLineBotText = artist.CompanyName,
				Ico0Key = gentres.GetImageKey(artist.GentreGroup, SMGentresBase.GentreClassType.Age, artist.GentreAge),
				Ico1Key = gentres.GetImageKey(artist.GentreGroup, SMGentresBase.GentreClassType.Category, artist.GentreCategory),
				Ico2Key = gentres.GetImageKey(artist.GentreGroup, SMGentresBase.GentreClassType.Content, artist.GentreContent),
				Ico3Key = 101
			};

			AddItem(newItem, insertIndex);
			return true;
		}

		// Добавить новый элемент - режим для списка выступлений и списка треков
		public bool Add(SMTrack track, SMGentresBase gentres, int insertIndex =-1, bool checkExist = false)
		{
			if (checkExist && GetItemIndexByID(track.ID) >= 0)
			{
				return false;
			}

			SMArtist artist = track.ParentArtist;
			SMListViewItem newItem = new SMListViewItem(viewMode, track.ID)
			{
				MainImageKey = gentres.GetImageKey(artist.GentreGroup, SMGentresBase.GentreClassType.GentreGroup, artist.GentreGroup),
				MainTimeText = "",
				SubTimeText = track.TrackLength.ToString(@"hh\:mm\:ss", ci),
				OneLineText = "",
				TwoLineTopText = track.Name,
				TwoLineBotText = artist.Name,
				Ico0Key = gentres.GetImageKey(artist.GentreGroup, SMGentresBase.GentreClassType.Age, artist.GentreAge),
				Ico1Key = gentres.GetImageKey(artist.GentreGroup, SMGentresBase.GentreClassType.Category, artist.GentreCategory),
				Ico2Key = gentres.GetImageKey(artist.GentreGroup, SMGentresBase.GentreClassType.Content, artist.GentreContent),
				Ico3Key = 101
			};

			AddItem(newItem, insertIndex);
			return true;
		}

		// Добавить новый элемент
		public bool Add(SMElement element, int insertIndex = -1, bool checkExist = false)
		{
			if (checkExist && GetItemIndexByID(element.ID) >= 0)
			{
				return false;
			}

			SMListViewItem newItem = new SMListViewItem(viewMode, element.ID)
			{
				MainImageKey = element.ImageKey,
				MainTimeText = "",
				SubTimeText = "",
				OneLineText = element.Name,
				TwoLineTopText = "",
				TwoLineBotText = "",
				Ico0Key = 101,
				Ico1Key = 101,
				Ico2Key = 101,
				Ico3Key = 101
			};

			AddItem(newItem, insertIndex);
			return true;
		}

		// Добавление элемента
		private void AddItem(SMListViewItem item, int insertIndex)
		{
			if (insertIndex == -1)
				Items.Add(item);
			else
				Items.Insert(insertIndex, item);
		}

		// Редактирование артиста
		public void Edit(long itemID, SMArtist artist, SMGentresBase gentres)
		{
			int itemIndex = GetItemIndexByID(itemID);
			if (itemIndex >= 0)
			{
				var timeLen = artist.PrepareTimeStart + artist.PrepareTimeLength + artist.PrepareTimeFinish;

				SMListViewItem item = Items[itemIndex];
				item.MainImageKey = gentres.GetImageKey(artist.GentreGroup, SMGentresBase.GentreClassType.GentreGroup, artist.GentreGroup);
				item.MainTimeText = "";
				item.SubTimeText = timeLen.ToString(@"hh\:mm\:ss", ci);
				item.OneLineText = "";
				item.TwoLineTopText = artist.Name;
				item.TwoLineBotText = artist.CompanyName;
				item.Ico0Key = gentres.GetImageKey(artist.GentreGroup, SMGentresBase.GentreClassType.Age, artist.GentreAge);
				item.Ico1Key = gentres.GetImageKey(artist.GentreGroup, SMGentresBase.GentreClassType.Category, artist.GentreCategory);
				item.Ico2Key = gentres.GetImageKey(artist.GentreGroup, SMGentresBase.GentreClassType.Content, artist.GentreContent);
				item.Ico3Key = 101;
			}
		}

		// Редактирование трека
		public void Edit(long itemID, SMTrack track, SMGentresBase gentres)
		{
			int itemIndex = GetItemIndexByID(itemID);
			if (itemIndex >= 0)
			{
				SMArtist artist = track.ParentArtist;
				SMListViewItem item = Items[itemIndex];
				item.MainImageKey = gentres.GetImageKey(artist.GentreGroup, SMGentresBase.GentreClassType.GentreGroup, artist.GentreGroup);
				item.MainTimeText = "";
				item.SubTimeText = track.TrackLength.ToString(@"hh\:mm\:ss", ci);
				item.OneLineText = "";
				item.TwoLineTopText = track.Name;
				item.TwoLineBotText = artist.Name;
				item.Ico0Key = gentres.GetImageKey(artist.GentreGroup, SMGentresBase.GentreClassType.Age, artist.GentreAge);
				item.Ico1Key = gentres.GetImageKey(artist.GentreGroup, SMGentresBase.GentreClassType.Category, artist.GentreCategory);
				item.Ico2Key = gentres.GetImageKey(artist.GentreGroup, SMGentresBase.GentreClassType.Content, artist.GentreContent);
				item.Ico3Key = 101;
			}

		}

		// Редактирование элемента
		public void Edit(long itemID, SMElement element)
		{
			int itemIndex = GetItemIndexByID(itemID);
			if (itemIndex >= 0)
			{
				SMListViewItem item = Items[itemIndex];
				item.MainImageKey = element.ImageKey;
				item.MainTimeText = "";
				item.SubTimeText = "";
				item.OneLineText = element.Name;
				item.TwoLineTopText = "";
				item.TwoLineBotText = "";
				item.Ico0Key = 101;
				item.Ico1Key = 101;
				item.Ico2Key = 101;
				item.Ico3Key = 101;
			}
		}

		// Редактирование главного времени
		public void EditMainTime(long itemID, TimeSpan time)
		{
			int itemIndex = GetItemIndexByID(itemID);
			if (itemIndex >= 0)
			{
				SMListViewItem item = Items[itemIndex];
				item.MainTimeText = time.ToString(@"hh\:mm\:ss", ci);
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
