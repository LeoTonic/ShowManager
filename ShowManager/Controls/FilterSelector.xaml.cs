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
using System.Collections.ObjectModel;
using System.ComponentModel;
using ShowManager.Tools;

namespace ShowManager.Controls
{
	/// <summary>
	/// Логика взаимодействия для FilterSelector.xaml
	/// </summary>
	public partial class FilterSelector : Window
	{
		// Node element
		class FilterNodeItem: INotifyPropertyChanged
		{
			public event PropertyChangedEventHandler PropertyChanged;

			private string name;
			public string Name {
				get {
					return this.name;
				}
				set {
					this.name = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
				}
			}
			private bool? vchecked;
			public bool? Checked {
				get
				{
					return vchecked;
				}
				set
				{
					this.vchecked = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Checked"));
				}
			}
			public ObservableCollection<FilterNodeItem> Children { get; set; }
			public FilterNodeItem Parent { get; set; }
			public long ID { get; set; }

			public FilterView.FilterItem TwinItem { get; set; }
		}

		ObservableCollection<FilterNodeItem> Nodes;
		private MainWindow parentWindow;
		private bool skipCheckUpdate = false;
		public FilterView filterView;

		public FilterSelector(MainWindow parent)
		{
			Nodes = new ObservableCollection<FilterNodeItem>();
			InitializeComponent();
			parentWindow = parent;
			DataContext = Nodes;

			filterView = new FilterView(parent.filterView);
			GetFilter();
		}

		private void ButtonApply_Click(object sender, RoutedEventArgs e)
		{
			// Применить фильтр
			SetFilter();
			parentWindow.filterView.Assign(filterView);
			parentWindow.isFilterSelectorActive = false;
      parentWindow.RefreshArtistView(true);
      Close();
		}

		private void ButtonClear_Click(object sender, RoutedEventArgs e)
		{
			// Очистка фильтра
			cboSortingTypes.SelectedIndex = 0;
			cboSortingOrder.SelectedIndex = 0;

			foreach(FilterNodeItem fi in Nodes)
			{
				ClearFilterNode(fi);
			}
		}

		private void ClearFilterNode(FilterNodeItem node)
		{
			if (node.Children != null)
			{
				foreach(FilterNodeItem fi in node.Children)
				{
					ClearFilterNode(fi);
				}
			}
			node.Checked = false;
		}

		// Обновление чекбокса родительского узла
		private void UpdateParentCheckBox(FilterNodeItem node)
		{
			var parentNode = node.Parent;
			if (parentNode == null)
				return;

			// Проверка состояний всех соседних узлов
			int checkedCount = 0;
			foreach(FilterNodeItem ch in parentNode.Children)
			{
				if (ch.Checked == true)
					checkedCount++;
			}
			if (checkedCount == 0)
			{
				parentNode.Checked = false;
			}
			else if (checkedCount == parentNode.Children.Count)
			{
				parentNode.Checked = true;
			}
			else
			{
				parentNode.Checked = null;
			}
		}

		// Обновление чекбоксов дочерних узлов
		private void UpdateChildrenCheckBox(FilterNodeItem node) {
			if (node.Children == null) return;

			foreach(FilterNodeItem ch in node.Children)
			{
				UpdateChildrenCheckBox(ch);
				ch.Checked = node.Checked;
			}
		}

		private void CheckBox_Check(object sender, RoutedEventArgs e) {
			var dObject = e.OriginalSource;
			while (dObject != null)
			{
				dObject = VisualTreeHelper.GetParent(dObject as DependencyObject);
				if (dObject is TreeViewItem)
				{
					if ((dObject as TreeViewItem).DataContext is FilterNodeItem dc && !skipCheckUpdate)
					{
						skipCheckUpdate = true;
						UpdateChildrenCheckBox(dc);
						UpdateParentCheckBox(dc);
						skipCheckUpdate = false;
					}
					break;
				}
			}
		}
		private void ChechBox_Uncheck(object sender, RoutedEventArgs e)
		{
			var dObject = e.OriginalSource;
			while (dObject != null)
			{
				dObject = VisualTreeHelper.GetParent(dObject as DependencyObject);
				if (dObject is TreeViewItem)
				{
					if ((dObject as TreeViewItem).DataContext is FilterNodeItem dc && !skipCheckUpdate)
					{
						skipCheckUpdate = true;
						UpdateChildrenCheckBox(dc);
						UpdateParentCheckBox(dc);
						skipCheckUpdate = false;
					}
					break;
				}
			}
		}

		// Установка фильтра
		private void SetFilter()
		{
			var selected = cboSortingTypes.SelectedItem as ComboBoxItem;
			filterView.SortingType = (int)selected.Tag;
			filterView.sortAscend = (cboSortingOrder.SelectedIndex == 0);

			foreach(FilterNodeItem fi in Nodes)
			{
				SetFilterValue(fi);
			}
		}

		private void SetFilterValue(FilterNodeItem node)
		{
			if (node.Children != null)
			{
				foreach (FilterNodeItem fi in node.Children)
				{
					SetFilterValue(fi);
				}
			}
			node.TwinItem.Checked = node.Checked;
		}

		// Получение фильтра
		private void GetFilter()
		{
			// Сортировка
			cboSortingTypes.Items.Clear();
			foreach(KeyValuePair<int, string> kp in filterView.sortTypes)
			{
				var newCI = new ComboBoxItem()
				{
					Content = kp.Value,
					Tag = kp.Key
				};
				cboSortingTypes.Items.Add(newCI);
			}
			cboSortingTypes.Text = filterView.sortTypes[filterView.SortingType];

			if (filterView.sortAscend)
			{
				cboSortingOrder.SelectedIndex = 0;
			}
			else
			{
				cboSortingOrder.SelectedIndex = 1;
			}

			// Фильтрация
			Nodes.Clear();
			foreach(FilterView.FilterItem fi in filterView.FilterItems)
			{
				var newNode = new FilterNodeItem();
				Nodes.Add(newNode);
				AddItems(fi, newNode, null);
			}
		}
		private void AddItems(FilterView.FilterItem filter, FilterNodeItem node, FilterNodeItem parent)
		{
			if (filter.Children != null)
			{
				node.Children = new ObservableCollection<FilterNodeItem>();
				foreach(FilterView.FilterItem fItem in filter.Children)
				{
					var newNode = new FilterNodeItem();
					node.Children.Add(newNode);
					AddItems(fItem, newNode, node);
				}
			}
			node.Name = filter.Name;
			node.Checked = filter.Checked;
			node.ID = filter.ID[0];
			node.Parent = parent;
			node.TwinItem = filter;
		}
	}

}
