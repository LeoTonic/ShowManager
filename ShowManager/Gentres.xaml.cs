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
using System.Windows.Shapes;
using ShowManager.Controls;
using ShowManager.Models;

namespace ShowManager
{
	/// <summary>
	/// Логика взаимодействия для Gentres.xaml
	/// </summary>
	public partial class Gentres : Window, ICommandCatcher
	{
		private SMGentresBase gentreBase;

		public Gentres(DragDropWindow wndDD, SMGentresBase gb)
		{
			InitializeComponent();

			gentreBase = gb;

			// Инициализация элементов управления
			lvwGentreGroups.SetDragDropWindow(wndDD);
			lvwGentreGroups.SetViewMode(Controls.SMListView.ViewMode.Gentre, this);
			gentresToolbar.SetValues("prop-add", "Добавить жанр", "prop-edit", "Изменить жанр", "prop-delete", "Удалить жанр", this);
		}

		// Обработка панели инструментов
		public void ToolBarAdd(SMToolbar tb)
		{
			// Отображаем диалог элемента
			ElementDialog elemDlg = new ElementDialog(101, "Новый элемент");

			Point pnt = lvwGentreGroups.PointToScreen(new Point(0, 0));
			elemDlg.Left = pnt.X + 8;
			elemDlg.Top = pnt.Y + 8;

			elemDlg.ShowDialog();
			if (elemDlg.IsSet)
			{
				long itemID = gentreBase.Add(elemDlg.ElementName, elemDlg.ImageKey);
				lvwGentreGroups.Add(itemID, elemDlg.ImageKey, labelText1: elemDlg.ElementName);
			}
		}

		public void ToolBarEdit(SMToolbar tb)
		{
			// Если элемент не выбран - выходим
			if (lvwGentreGroups.ItemsSelected.Count > 0)
			{
				var lvi = lvwGentreGroups.ItemsSelected[0] as SMListViewItem;
				long id = lvi.ItemID;
				SMGentre gentre = gentreBase.GetGentreItem(id);
				if (gentre == null) return;

				ElementDialog elemDlg = new ElementDialog(gentre.ImageKey, gentre.Name);
				Point pnt = lvwGentreGroups.PointToScreen(new Point(0, 0));
				elemDlg.Left = pnt.X + 8;
				elemDlg.Top = pnt.Y + 8;

				elemDlg.ShowDialog();
				if (elemDlg.IsSet)
				{
					gentreBase.Edit(id, elemDlg.ElementName, elemDlg.ImageKey);
					lvwGentreGroups.Edit(id, elemDlg.ImageKey, labelText1: elemDlg.ElementName);
				}
			}
		}

		public void ToolBarRemove(SMToolbar tb)
		{
			// Если элемент не выбран - выходим
			if (lvwGentreGroups.ItemsSelected.Count > 0)
			{
				var lvi = lvwGentreGroups.ItemsSelected[0] as SMListViewItem;
				long id = lvi.ItemID;
				gentreBase.Remove(id);
				lvwGentreGroups.Remove(id);
			}
		}

		public void DropItems(int insertIndex, SMListViewItem draggedItem, Object draggedTo)
		{
			var lView = draggedItem.dragFromControl as SMListView;
			var items = lView.Items as ObservableCollection<SMListViewItem>;

			if (draggedTo.GetHashCode() == gentresToolbar.GetHashCode())
			{
				// Сброс в корзину

				// Удаление из контрола
				List<SMListViewItem> wList = new List<SMListViewItem>();
				foreach(SMListViewItem lvi in draggedItem.selectedItems)
				{
					wList.Add(lvi);
				}
				foreach(SMListViewItem lvi in wList)
				{
					items.Remove(lvi);
				}
				wList.Clear();
			}

			if (draggedTo.GetHashCode() == lView.GetHashCode())
			{
				// Перемещение внутри контрола
				List<SMListViewItem> wList = new List<SMListViewItem>();
				foreach (SMListViewItem lvi in draggedItem.selectedItems)
				{
					SMListViewItem newItem = new SMListViewItem(lvi);
					wList.Add(newItem);
				}

				List<SMListViewItem> wList2 = new List<SMListViewItem>();
				foreach (SMListViewItem lvi in draggedItem.selectedItems)
				{
					wList2.Add(lvi);
				}
				foreach (SMListViewItem lvi in wList2)
				{
					items.Remove(lvi);
				}

				foreach (SMListViewItem lvi in wList)
				{
					if (insertIndex >= 0)
					{
						items.Insert(insertIndex, lvi);
					}
					else
					{
						items.Add(lvi);
					}
				}
				wList.Clear();
				wList2.Clear();
			}
		}
	}
}
