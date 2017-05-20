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
	public partial class MainWindow : Window, ICommandCatcher
	{
		// Окна приложения
		DragDropWindow wndDragDrop;

		// Объекты приложения
		SMGentresBase gentres;

		// Текущий проект
		SMProject currentProject;

		// Имя текущей группы (в панели групп артистов)
		private string selectedPanelName;

		// Идентификатор выбранного артиста
		private long selectedArtistID = -1;

		public MainWindow()
		{
			InitializeComponent();
			ImgLibInit();

			// Инициализация объектов
			gentres = new SMGentresBase();

			// Инициализация проекта
			currentProject = new SMProject()
			{
				TrackFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
			};

			// Инициализация окон
			wndDragDrop = new DragDropWindow();

			wndDragDrop.HideWindow();
			ArtistView.SetDragDropWindow(wndDragDrop);
			ArtistView.SetViewMode(SMListView.ViewMode.ArtistName, this);
			ArtistPanel.Initialize(null, this, false, currentProject.GroupsArtist);

			TrackView.SetDragDropWindow(wndDragDrop);
			TrackView.SetViewMode(SMListView.ViewMode.ArtistTrack, this);

			ArtistToolBar.SetValues("user-add", "Добавить исполнителя", "user-edit", "Изменить исполнителя", "user-delete", "Удалить исполнителя", this);

			Closed += MainWindow_Closed;
		}

		private void MainWindow_Closed(object sender, EventArgs e)
		{
			wndDragDrop.Close();
		}

		//
		// Обработка команд меню
		//
#region Menu Calls
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
			var wndGentres = new Gentres(wndDragDrop, gentres);
			wndGentres.ShowDialog();
		}

		#endregion

#region CommandCather definitions
		public void ItemSelect(object sender, long itemID)
		{
			if (sender.GetHashCode() == ArtistView.GetHashCode())
			{
				selectedArtistID = itemID;
				// Сюда код отображения треков
			}
		}

		// Новый артист
		public void ToolBarAdd(SMToolbar tb)
		{
			var artistWindow = new Artist(currentProject, gentres, null);
			if (artistWindow.ShowDialog() == true)
			{
				// Добавляем в массив
				var newArtist = new SMArtist(currentProject, gentres, artistWindow.ArtistObj);
				currentProject.Artists.Add(newArtist);

				// И в группу
				SMGroup getGroup = currentProject.GetGroup(SMProject.GroupType.Artist, selectedPanelName);
				if (getGroup != null)
				{
					getGroup.Add(newArtist.ID);
					ArtistView.Add(newArtist.ID, 106, labelText1: newArtist.Name, labelText2: newArtist.CompanyName);
				}
			}
		}
		public void ToolBarEdit(SMToolbar tb)
		{
			if (selectedArtistID == -1)
				return;
			SMArtist getArtist = currentProject.GetArtistByID(selectedArtistID);
			if (getArtist == null)
				return;
			var artistWindow = new Artist(currentProject, gentres, getArtist);
			if (artistWindow.ShowDialog() == true)
			{
				// Редактируем элемент
				getArtist.Assign(artistWindow.ArtistObj);
				ArtistView.Edit(getArtist.ID, 106, labelText1: getArtist.Name, labelText2: getArtist.CompanyName);
			}
		}
		public void ToolBarRemove(SMToolbar tb)
		{
			if (selectedArtistID == -1)
				return;
			SMArtist getArtist = currentProject.GetArtistByID(selectedArtistID);
			if (getArtist == null)
				return;

			if (MessageBox.Show("Удаляем выбранного исполнителя?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				currentProject.Artists.Remove(getArtist);
				SMGroup getGroup = currentProject.GetGroup(SMProject.GroupType.Artist, selectedPanelName);
				if (getGroup != null)
				{
					getGroup.Remove(selectedArtistID);
					ArtistView.Remove(selectedArtistID);
					selectedArtistID = -1;
				}
			}
		}

		public void PanelGroupClick(string panelName)
		{
			selectedPanelName = panelName;
			// Сюда добавить код отображения артистов группы
			RefreshArtistView();
		}
		public void PanelGroupAdd(string panelName) {
			currentProject.GroupsArtist.Add(new SMGroup(panelName));
		}

		public void PanelGroupRename(string oldName, string newName) {
			// Ищем группу
			SMGroup getGroup = currentProject.GetGroup(SMProject.GroupType.Artist, oldName);
			getGroup.Name = newName;
		}

		public void PanelGroupDelete(string panelName) {
			// Написать обработку удаления или переноса элементов в основную группу
		}

		public void DropItems(int insertIndex, SMListViewItem draggedItem, Object draggedTo)
		{

		}
#endregion

		// Отображение элементов при изменении группы
		private void RefreshArtistView()
		{
			ArtistView.Clear();
			SMGroup getGroup = currentProject.GetGroup(SMProject.GroupType.Artist, selectedPanelName);
			if (getGroup == null)
				return;
			foreach(long artistID in getGroup.IDList)
			{
				SMArtist getArtist = currentProject.GetArtistByID(artistID);
				ArtistView.Add(getArtist.ID, 106, labelText1: getArtist.Name, labelText2: getArtist.CompanyName);
			}
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
