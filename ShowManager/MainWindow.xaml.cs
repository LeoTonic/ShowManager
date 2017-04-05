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
		DragDropWindow ddWindow = new DragDropWindow();

		public MainWindow()
		{
			InitializeComponent();

			ddWindow.HideWindow();
			ExampleView.SetDragDropWindow(ddWindow);
			ExamplePanel.AssignView(ExampleView);

			Closed += MainWindow_Closed;
		}

		private void MainWindow_Closed(object sender, EventArgs e)
		{
			ddWindow.Close();
		}
	}
}
