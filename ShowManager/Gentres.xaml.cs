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
		private List<SMListViewItem> helpList1 = new List<SMListViewItem>();
		private List<SMListViewItem> helpList2 = new List<SMListViewItem>();

		private SMGentre selectedGentreGroup = null;
		private List<SMElement> classesList = null;

		private List<string> genClasses;

		public Gentres(DragDropWindow wndDD, SMGentresBase gb)
		{
			InitializeComponent();

			this.Closed += Gentres_Closed;
			this.Loaded += Gentres_Loaded;

			gentreBase = gb;

			// Инициализация элементов управления
			gentresView.SetDragDropWindow(wndDD);
			gentresView.SetViewMode(SMListView.ViewMode.Gentre, this);
			gentresToolbar.SetValues("prop-add", "Добавить жанр", "prop-edit", "Изменить жанр", "prop-delete", "Удалить жанр", this);

			genClasses = new List<string>()
			{
				"Жанр",
				"Направление",
				"Состав",
				"Возраст",
				"Категория",
				"Оценка"
			};
			classesPanel.Initialize(genClasses, this, true);
			classesToolbar.SetValues("prop-add", "Добавить элемент", "prop-edit", "Изменить элемент", "prop-delete", "Удалить элемент", this);
			classesView.SetDragDropWindow(wndDD);
			classesView.SetViewMode(SMListView.ViewMode.Gentre, this);
		}

		private void Gentres_Loaded(object sender, RoutedEventArgs e)
		{
			// Заполняем жанры
			gentresView.Items.Clear();

			foreach (SMElement sme in gentreBase.GentreGroups)
			{
				gentresView.Add(sme.ID, sme.ImageKey, labelText1: sme.Name);
			}
		}

		private void Gentres_Closed(object sender, EventArgs e)
		{
			gentreBase.Save();
		}

		// Помощник по коллекции
		// Перемещает элементы из одного списка в другой при обработке перетаскивания элементов
		private void CreateHelperList(System.Collections.IList sourceList, List<SMListViewItem> destList, bool createItems)
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

		// Заполнение списка элементов в контроле классов
		private void FillGentreClassItems(List<SMElement> elemList)
		{
			classesView.Clear();

			foreach (SMElement sme in elemList)
			{
				classesView.Add(itemID: sme.ID, mainImgKey: sme.ImageKey, labelText1: sme.Name);
			}
		}

		//
		// НАСЛЕДОВАНИЕ ИНТЕРФЕЙСА ICommandCatcher
		//

		// Обработка панели инструментов
		public void ToolBarAdd(SMToolbar tb)
		{
			// Отображаем диалог элемента
			ElementDialog elemDlg = new ElementDialog(101, "Новый элемент");

			Point pnt;

			if (tb.GetHashCode() == gentresToolbar.GetHashCode())
			{
				pnt = gentresView.PointToScreen(new Point(0, 0));
			}
			else
			{
				pnt = classesView.PointToScreen(new Point(0, 0));

				if (classesList == null)
				{
					return;
				}
			}

			elemDlg.Left = pnt.X + 8;
			elemDlg.Top = pnt.Y + 8;

			elemDlg.ShowDialog();
			if (elemDlg.IsSet)
			{
				if (tb.GetHashCode() == gentresToolbar.GetHashCode())
				{
					// Добавляем новую группу
					long itemID = gentreBase.Add(elemDlg.ElementName, elemDlg.ImageKey);
					gentresView.Add(itemID, elemDlg.ImageKey, labelText1: elemDlg.ElementName);
				}
				else if (tb.GetHashCode() == classesToolbar.GetHashCode())
				{
					if (classesList == null)
						return;
					// Добавляем новый элемент в классе жанровой группы
					var newItem = new SMElement(elemDlg.ElementName, elemDlg.ImageKey);
					classesList.Add(newItem);
					classesView.Add(newItem.ID, newItem.ImageKey, labelText1: newItem.Name);
				}
			}
		}

		public void ToolBarEdit(SMToolbar tb)
		{
			// Если элемент не выбран - выходим
			if (tb.GetHashCode() == gentresToolbar.GetHashCode())
			{
				long itemID = gentresView.GetFirstSelectedID();
				if (itemID == -1)
					return;

				SMGentre gentre = gentreBase.GetGentreItem(itemID);
				if (gentre == null) return;

				ElementDialog elemDlg = new ElementDialog(gentre.ImageKey, gentre.Name);
				Point pnt = gentresView.PointToScreen(new Point(0, 0));
				elemDlg.Left = pnt.X + 8;
				elemDlg.Top = pnt.Y + 8;

				elemDlg.ShowDialog();
				if (elemDlg.IsSet)
				{
					gentreBase.Edit(itemID, elemDlg.ElementName, elemDlg.ImageKey);
					gentresView.Edit(itemID, elemDlg.ImageKey, labelText1: elemDlg.ElementName);
				}
			}
			else if (tb.GetHashCode() == classesToolbar.GetHashCode() && classesList != null)
			{
				if (classesView.ItemsSelected.Count > 0)
				{
					var lvi = classesView.ItemsSelected[0] as SMListViewItem;
					long id = lvi.ItemID;
					SMElement sme = gentreBase.GetClassItem(classesList, id);
					if (sme == null) return;

					ElementDialog elemDlg = new ElementDialog(sme.ImageKey, sme.Name);
					Point pnt = classesView.PointToScreen(new Point(0, 0));
					elemDlg.Left = pnt.X + 8;
					elemDlg.Top = pnt.Y + 8;

					elemDlg.ShowDialog();
					if (elemDlg.IsSet)
					{
						gentreBase.EditClassItem(classesList, id, elemDlg.ElementName, elemDlg.ImageKey);
						classesView.Edit(id, elemDlg.ImageKey, labelText1: elemDlg.ElementName);
					}
				}
			}
		}

		public void ToolBarRemove(SMToolbar tb)
		{
			// Если элемент не выбран - выходим
			if (tb.GetHashCode() == gentresToolbar.GetHashCode())
			{
				if (gentresView.ItemsSelected.Count > 0)
				{
					var lvi = gentresView.ItemsSelected[0] as SMListViewItem;
					long id = lvi.ItemID;
					gentreBase.Remove(id);
					gentresView.Remove(id);
				}
			}
			else if (tb.GetHashCode() == classesToolbar.GetHashCode() && classesList != null)
			{
				if (classesView.ItemsSelected.Count > 0)
				{
					var lvi = classesView.ItemsSelected[0] as SMListViewItem;
					long id = lvi.ItemID;
					gentreBase.DeleteClassItem(classesList, id);
					classesView.Remove(id);
				}
			}
		}

		public void DropItems(int insertIndex, SMListViewItem draggedItem, Object draggedTo)
		{
			var lView = draggedItem.dragFromControl as SMListView;
			var items = lView.Items as ObservableCollection<SMListViewItem>;

			if (draggedTo.GetHashCode() == gentresToolbar.GetHashCode())
			{
				// Сброс в корзину
				CreateHelperList(draggedItem.selectedItems, helpList1, false);

				// Удаление из контрола и элемента данных
				foreach (SMListViewItem lvi in helpList1)
				{
					if (lView.GetHashCode() == gentresView.GetHashCode())
					{
						// Жанровые группы
						gentreBase.Remove(lvi.ItemID);
					}
					else if (lView.GetHashCode() == classesView.GetHashCode() && classesList != null)
					{
						// Классы в жанровой группе
						gentreBase.DeleteClassItem(classesList, lvi.ItemID);
					}
					items.Remove(lvi);
				}

			}

			if (draggedTo.GetHashCode() == lView.GetHashCode())
			{
				// Перемещение внутри контрола
				CreateHelperList(draggedItem.selectedItems, helpList1, true);
				CreateHelperList(draggedItem.selectedItems, helpList2, false);
				foreach (SMListViewItem lvi in helpList2)
				{
					items.Remove(lvi);
				}

				foreach (SMListViewItem lvi in helpList1)
				{
					if (lView.GetHashCode() == gentresView.GetHashCode())
					{
						gentreBase.Move(lvi.ItemID, insertIndex);
					}
					else if (lView.GetHashCode() == classesView.GetHashCode() && classesList != null)
					{
						gentreBase.MoveClassItem(classesList, lvi.ItemID, insertIndex);
					}

					if (insertIndex >= 0)
					{
						items.Insert(insertIndex, lvi);
					}
					else
					{
						items.Add(lvi);
					}
				}
			}
		}

		// Выбор элементв в контроле
		public void ItemSelectionChange(Object parentControl)
		{
			if (parentControl.GetHashCode() == gentresView.GetHashCode())
			{
				// Обработка жанровой группы - заполнение элементов жанра в правом контроле
				long itemID = gentresView.GetFirstSelectedID();
				if (itemID == -1)
					return;

				selectedGentreGroup = gentreBase.GetGentreItem(itemID);
				if (selectedGentreGroup == null)
					return;
				classesPanel.SelectFirstGroup();
			}
		}

		// ОБРАБОТЧИКИ ПАНЕЛИ
		public void PanelGroupClick(string groupName)
		{
			if (selectedGentreGroup == null)
				return;

			if (groupName == genClasses[0])
			{
				classesList = selectedGentreGroup.Gentres;
			}
			else if (groupName == genClasses[1])
			{
				classesList = selectedGentreGroup.Directions;
			}
			else if (groupName == genClasses[2])
			{
				classesList = selectedGentreGroup.Contents;
			}
			else if (groupName == genClasses[3])
			{
				classesList = selectedGentreGroup.Ages;
			}
			else if (groupName == genClasses[4])
			{
				classesList = selectedGentreGroup.Categories;
			}
			else
			{
				classesList = selectedGentreGroup.EvaluateTypes;
			}
			FillGentreClassItems(classesList);
		}

		public void PanelGroupAdd(string groupName)
		{
		}
		public void PanelGroupRename(string groupNameOld, string groupNameNew)
		{
		}
		public void PanelGroupDelete(string groupName)
		{
		}
	}
}
