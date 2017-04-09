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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ShowManager.Controls;

namespace ShowManager
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		DragDropWindow wndDragDrop;
		Gentres wndGentres;

		public MainWindow()
		{
			InitializeComponent();

			wndDragDrop = new DragDropWindow();
			wndGentres = new Gentres(wndDragDrop);

			wndDragDrop.HideWindow();
			ExampleView.SetDragDropWindow(wndDragDrop);
			ExamplePanel.AssignView(ExampleView);

			Closed += MainWindow_Closed;
		}

		private void MainWindow_Closed(object sender, EventArgs e)
		{
			wndDragDrop.Close();
			wndGentres.Close();
		}

		//
		// Обработка команд меню
		//

		//
		// Фестиваль
		//

		// Выход из приложения
		public void Menu_Show_Exit(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		//
		// Каталоги
		//

		// Жанры
		public void Menu_Catalogue_Gentres(object sender, RoutedEventArgs e)
		{
			wndGentres.Show();
		}
	}
}
