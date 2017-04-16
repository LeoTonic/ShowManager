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
using ShowManager.Models;

namespace ShowManager
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		// Окна приложения
		DragDropWindow wndDragDrop;
		Gentres wndGentres;

		// Объекты приложения
		SMGentresBase gentres;

		public MainWindow()
		{
			InitializeComponent();
			ImgLibInit();

			// Инициализация объектов
			gentres = new SMGentresBase();

			// Инициализация окон
			wndDragDrop = new DragDropWindow();
			wndGentres = new Gentres(wndDragDrop, gentres);


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

		// Инициализация библиотеки изображений
		private void ImgLibInit()
		{
			App curApp = (App)Application.Current;
			curApp.ImgPath = new Dictionary<int, string>();
			curApp.ImgDesc = new Dictionary<int, string>();

			curApp.ImgPath.Add(101, "/ShowManager;component/Images/Gentres/art.png");
			curApp.ImgPath.Add(102, "/ShowManager;component/Images/Gentres/cinema.png");
			curApp.ImgPath.Add(103, "/ShowManager;component/Images/Gentres/circus.png");
			curApp.ImgPath.Add(104, "/ShowManager;component/Images/Gentres/concert-master.png");
			curApp.ImgPath.Add(105, "/ShowManager;component/Images/Gentres/dance.png");
			curApp.ImgPath.Add(106, "/ShowManager;component/Images/Gentres/director.png");
			curApp.ImgPath.Add(107, "/ShowManager;component/Images/Gentres/guitar.png");
			curApp.ImgPath.Add(108, "/ShowManager;component/Images/Gentres/music.png");
			curApp.ImgPath.Add(109, "/ShowManager;component/Images/Gentres/palette.png");
			curApp.ImgPath.Add(110, "/ShowManager;component/Images/Gentres/patriot.png");
			curApp.ImgPath.Add(111, "/ShowManager;component/Images/Gentres/pop.png");
			curApp.ImgPath.Add(112, "/ShowManager;component/Images/Gentres/sport.png");
			curApp.ImgPath.Add(113, "/ShowManager;component/Images/Gentres/theatre.png");
			curApp.ImgPath.Add(114, "/ShowManager;component/Images/Gentres/vocal.png");

			curApp.ImgDesc.Add(101, "ИЗО");
			curApp.ImgDesc.Add(102, "Кино и анимация");
			curApp.ImgDesc.Add(103, "Цирк");
			curApp.ImgDesc.Add(104, "Концертмейстер");
			curApp.ImgDesc.Add(105, "Хореография");
			curApp.ImgDesc.Add(106, "Конферансье");
			curApp.ImgDesc.Add(107, "Авторская песня");
			curApp.ImgDesc.Add(108, "Инструмент.");
			curApp.ImgDesc.Add(109, "ДПИ");
			curApp.ImgDesc.Add(110, "Патриот. песня");
			curApp.ImgDesc.Add(111, "Эстрада");
			curApp.ImgDesc.Add(112, "Худ.гимнастика");
			curApp.ImgDesc.Add(113, "Театр");
			curApp.ImgDesc.Add(114, "Вокал");
		}
	}
}
