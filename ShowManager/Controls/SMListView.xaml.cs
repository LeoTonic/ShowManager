﻿using System;
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
		public void SetViewMode(ViewMode newMode)
		{
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

		// Обновление прямоугольника выделения
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

		// Работа с элементами

		// Добавить новый элемент
		public void Add(string mainImgPath, string mainTimeText="", string subTimeText="", string labelText1="", string labelText2="", string ico0="", string ico1="", string ico2="", string ico3="")
		{
			SMListViewItem newItem = new SMListViewItem(viewMode)
			{
				MainImagePath = mainImgPath,
				MainTimeText = mainTimeText,
				SubTimeText = subTimeText,
				OneLineText = labelText1,
				TwoLineTopText = labelText1,
				TwoLineBotText = labelText2,
				IconsPath0 = ico0,
				IconsPath1 = ico1,
				IconsPath2 = ico2,
				IconsPath3 = ico3
			};

			Items.Add(newItem);
		}

		// Удалить элемент
		public void Remove(SMListViewItem item)
		{
			Items.Remove(item);
		}
	}
}
