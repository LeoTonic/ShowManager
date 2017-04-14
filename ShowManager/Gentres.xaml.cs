using System;
using System.Collections.Generic;
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

namespace ShowManager
{
	/// <summary>
	/// Логика взаимодействия для Gentres.xaml
	/// </summary>
	public partial class Gentres : Window, ICommandCatcher
	{
		public Gentres(DragDropWindow wndDD)
		{
			InitializeComponent();

			// Инициализация элементов управления
			lvwGentreGroups.SetDragDropWindow(wndDD);
			lvwGentreGroups.SetViewMode(Controls.SMListView.ViewMode.Gentre);
			gentresToolbar.SetValues("prop-add", "Добавить жанр", "prop-edit", "Изменить жанр", "prop-delete", "Удалить жанр", this);
		}

		// Обработка панели инструментов
		public void ToolBarAdd(SMToolbar tb)
		{
			// Отображаем диалог элемента
			ElementDialog elemDlg = new ElementDialog("", "Новый элемент");

			Point pnt = lvwGentreGroups.PointToScreen(new Point(0, 0));
			elemDlg.Left = pnt.X + 8;
			elemDlg.Top = pnt.Y + 8;

			elemDlg.ShowDialog();
			if (elemDlg.IsSet)
			{
				lvwGentreGroups.Add(elemDlg.ImagePath, labelText1: elemDlg.ElementName);
			}
		}

		public void ToolBarEdit(SMToolbar tb)
		{
			// Если элемент не выбран - выходим
			if (lvwGentreGroups.ItemsSelected.Count > 0)
			{
				var lvi = lvwGentreGroups.ItemsSelected[0] as SMListViewItem;
				ElementDialog elemDlg = new ElementDialog(lvi.MainImagePath, lvi.OneLineText);
				Point pnt = lvwGentreGroups.PointToScreen(new Point(0, 0));
				elemDlg.Left = pnt.X + 8;
				elemDlg.Top = pnt.Y + 8;

				elemDlg.ShowDialog();
				if (elemDlg.IsSet)
				{
					lvi.MainImagePath = elemDlg.ImagePath;
					lvi.OneLineText = elemDlg.ElementName;
				}
			}
		}

		public void ToolBarRemove(SMToolbar tb)
		{
			// Если элемент не выбран - выходим
			if (lvwGentreGroups.ItemsSelected.Count > 0)
			{
				var lvi = lvwGentreGroups.ItemsSelected[0] as SMListViewItem;
				lvwGentreGroups.Remove(lvi);
			}
		}
	}
}
