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
		}

		ObservableCollection<FilterNodeItem> Nodes;
		private MainWindow parentWindow;

		public FilterSelector(MainWindow parent)
		{
			Nodes = new ObservableCollection<FilterNodeItem>();
			InitializeComponent();
			parentWindow = parent;
			DataContext = Nodes;
		}

		private void ButtonApply_Click(object sender, RoutedEventArgs e)
		{
			// Применить фильтр
			parentWindow.isFilterSelectorActive = false;
			Close();
		}

		private void ButtonClear_Click(object sender, RoutedEventArgs e)
		{

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

		private void CheckBox_Check(object sender, RoutedEventArgs e) {
			var dObject = e.OriginalSource;
			while (dObject != null)
			{
				dObject = VisualTreeHelper.GetParent(dObject as DependencyObject);
				if (dObject is TreeViewItem)
				{
					if ((dObject as TreeViewItem).DataContext is FilterNodeItem dc)
					{
						UpdateParentCheckBox(dc);
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
					if ((dObject as TreeViewItem).DataContext is FilterNodeItem dc)
					{
						UpdateParentCheckBox(dc);
					}
					break;
				}
			}
		}

		private void CheckBox_Loaded(object sender, RoutedEventArgs e) {
			// work on loaded
			//Console.WriteLine("Loaded");
		}

		// Set filter
		public void SetFilter(FilterView fv)
		{
			// Сортировка
			cboSortingTypes.Items.Clear();
			foreach(KeyValuePair<int, string> kp in fv.sortTypes)
			{
				var newCI = new ComboBoxItem()
				{
					Content = kp.Value,
					Tag = kp.Key
				};
				cboSortingTypes.Items.Add(newCI);
				cboSortingTypes.Text = fv.sortTypes[fv.SortingType];
			}
			if (fv.sortAscend)
			{
				cboSortingOrder.SelectedIndex = 0;
			}
			else
			{
				cboSortingOrder.SelectedIndex = 1;
			}

			// Фильтрация
			Nodes.Clear();
			foreach(FilterView.FilterItem fi in fv.FilterItems)
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
			node.ID = filter.ID;
			node.Parent = parent;
		}

		private void TreeViewItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			var tvi = sender as TreeViewItem;
			Console.WriteLine(tvi.Name);
		}
	}

}
