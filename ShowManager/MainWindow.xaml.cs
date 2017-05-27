using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
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

		// Окно порядка выступлений
		ShowOrder wndOrdersShow;
		public bool isOrderShowActive = false;

		// Окно порядка репетиций
		PrepOrder wndOrdersPrep;
		public bool isOrderPrepActive = false;

		// Списки для помощи при переносе итемов
		private List<SMListViewItem> helpList1 = new List<SMListViewItem>();
		private List<SMListViewItem> helpList2 = new List<SMListViewItem>();

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

			OpenWindowShow(true);
			OpenWindowPrep(true);

			ArtistView.SetDragDropWindow(wndDragDrop);
			ArtistView.SetViewMode(SMListView.ViewMode.ArtistName, this);
			ArtistPanel.Initialize(null, this, false, currentProject.GroupsArtist);

			TrackView.SetDragDropWindow(wndDragDrop);
			TrackView.SetViewMode(SMListView.ViewMode.ArtistTrack, this);

			ArtistToolBar.SetValues("user-add", "Добавить исполнителя", "user-edit", "Изменить исполнителя", "user-delete", "Удалить исполнителя", this);

			Closed += MainWindow_Closed;
		}

		// Отображение окна выступлений
		private void OpenWindowShow(bool mode)
		{
			if (!isOrderShowActive && mode)
			{
				wndOrdersShow = new ShowOrder(wndDragDrop, currentProject, this);
				wndOrdersShow.Show();
				isOrderShowActive = true;
				MenuWindowShow.IsChecked = true;
			}
			else if (isOrderShowActive && !mode)
			{
				wndOrdersShow.Close();
				wndOrdersShow = null;
				isOrderShowActive = false;
				MenuWindowShow.IsChecked = false;
			}
		}

		// Отображение окна репетиций
		private void OpenWindowPrep(bool mode)
		{
			if (!isOrderPrepActive && mode)
			{
				wndOrdersPrep = new PrepOrder(wndDragDrop, currentProject, this);
				wndOrdersPrep.Show();
				isOrderPrepActive = true;
				MenuWindowPrep.IsChecked = true;
			}
			else if (isOrderPrepActive && !mode)
			{
				wndOrdersPrep.Close();
				wndOrdersPrep = null;
				isOrderPrepActive = false;
				MenuWindowPrep.IsChecked = false;
			}
		}


		private void MainWindow_Closed(object sender, EventArgs e)
		{
			OpenWindowShow(false);
			OpenWindowPrep(false);

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

		// Окна
		private void MenuWindowShow_Click(object sender, RoutedEventArgs e)
		{
			if (MenuWindowShow.IsChecked)
			{
				OpenWindowShow(false);
			}
			else
			{
				OpenWindowShow(true);
			}
		}
		private void MenuWindowPrep_Click(object sender, RoutedEventArgs e)
		{
			if (MenuWindowPrep.IsChecked)
			{
				OpenWindowPrep(false);
			}
			else
			{
				OpenWindowPrep(true);
			}
		}

		// Упорядочивание окон
		private void MenuWindowArrange_Click(object sender, RoutedEventArgs e)
		{
			// Установка положения окна исполнителей
			this.Left = SystemParameters.WorkArea.Left;
			this.Top = SystemParameters.WorkArea.Top;
			this.Height = SystemParameters.WorkArea.Height;
			this.Width = SystemParameters.WorkArea.Width * 0.5;

			double showHeight, prepHeight;
			double showTop, prepTop;
			if (isOrderShowActive)
			{
				showTop = SystemParameters.WorkArea.Top;
				showHeight = SystemParameters.WorkArea.Height;
				if (isOrderPrepActive)
					showHeight *= 0.5;

				wndOrdersShow.Left = SystemParameters.WorkArea.Width * 0.5;
				wndOrdersShow.Top = showTop;
				wndOrdersShow.Width = SystemParameters.WorkArea.Width * 0.5;
				wndOrdersShow.Height = showHeight;
			}

			if (isOrderPrepActive)
			{
				prepTop = SystemParameters.WorkArea.Height * 0.5;
				prepHeight = SystemParameters.WorkArea.Height * 0.5;

				if (!isOrderShowActive)
				{
					prepTop = SystemParameters.WorkArea.Top;
					prepHeight = SystemParameters.WorkArea.Height;
				}

				wndOrdersPrep.Left = SystemParameters.WorkArea.Width * 0.5;
				wndOrdersPrep.Top = prepTop;
				wndOrdersPrep.Width = SystemParameters.WorkArea.Width * 0.5;
				wndOrdersPrep.Height = prepHeight;
			}

		}


		#endregion

		#region CommandCather definitions
		public void ItemSelectionChange(object sender)
		{
			if (sender.GetHashCode() == ArtistView.GetHashCode())
			{
				// Сюда код отображения треков
				TrackView.Clear();

				foreach (SMListViewItem selItem in ArtistView.ItemsSelected)
				{
					long artistID = selItem.ItemID;
					SMArtist getArtist = currentProject.GetArtistByID(artistID);
					if (getArtist == null)
						continue;

					// Добавляем треки
					foreach (SMTrack track in getArtist.Tracks)
					{
						TrackView.Add(
							track.ID,
							gentres.GetImageKey(getArtist.GentreGroup, SMGentresBase.GentreClassType.GentreGroup, getArtist.GentreGroup),
							subTimeText: track.TrackLength.ToString(),
							labelText1: track.Name,
							labelText2: getArtist.Name,
							ico0: gentres.GetImageKey(getArtist.GentreGroup, SMGentresBase.GentreClassType.Age, getArtist.GentreAge),
							ico1: gentres.GetImageKey(getArtist.GentreGroup, SMGentresBase.GentreClassType.Category, getArtist.GentreCategory),
							ico2: gentres.GetImageKey(getArtist.GentreGroup, SMGentresBase.GentreClassType.Content, getArtist.GentreContent)
							);
					}
				}
			}
		}

		// Новый артист
		public void ToolBarAdd(SMToolbar tb)
		{
			ShowArtistDialog(null);
		}

		public void ToolBarEdit(SMToolbar tb)
		{
			long artistID = ArtistView.GetFirstSelectedID();
			if (artistID == -1)
				return;
			SMArtist getArtist = currentProject.GetArtistByID(artistID);
			if (getArtist == null)
				return;
			ShowArtistDialog(getArtist);
		}

		public void ToolBarRemove(SMToolbar tb)
		{
			long artistID = ArtistView.GetFirstSelectedID();
			if (artistID == -1)
				return;
			SMArtist getArtist = currentProject.GetArtistByID(artistID);
			if (getArtist == null)
				return;

			if (MessageBox.Show("Удаляем выбранного исполнителя?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				currentProject.Artists.Remove(getArtist);
				SMGroup getGroup = currentProject.GetGroup(SMProject.GroupType.Artist, selectedPanelName);
				if (getGroup != null)
				{
					getGroup.Remove(artistID);
					ArtistView.Remove(artistID);
					TrackView.Clear();
				}
			}
		}

		// Двойной клик по элементу
		public void ItemDoubleClick(Object parentControl, SMListViewItem selectedItem)
		{
			if (parentControl.GetHashCode() == ArtistView.GetHashCode())
			{
				long artistID = selectedItem.ItemID;
				SMArtist getArtist = currentProject.GetArtistByID(artistID);
				if (getArtist == null)
					return;
				ShowArtistDialog(getArtist);
			}
		}

		// Отображение диалога исполнителя
		private void ShowArtistDialog(SMArtist artist)
		{
			var artistWindow = new Artist(currentProject, gentres, artist);
			if (artistWindow.ShowDialog() == true)
			{
				if (artist == null)
				{
					// Добавляем в массив
					var newArtist = new SMArtist(currentProject, gentres, artistWindow.ArtistObj);
					currentProject.Artists.Add(newArtist);
					// И в группу
					SMGroup getGroup = currentProject.GetGroup(SMProject.GroupType.Artist, selectedPanelName);
					if (getGroup != null)
					{
						getGroup.Add(newArtist.ID);
						ArtistView.Add(
							newArtist.ID,
							gentres.GetImageKey(newArtist.GentreGroup, SMGentresBase.GentreClassType.GentreGroup, 0),
							labelText1: newArtist.Name,
							labelText2: newArtist.CompanyName,
							ico0: gentres.GetImageKey(newArtist.GentreGroup, SMGentresBase.GentreClassType.Age, newArtist.GentreAge),
							ico1: gentres.GetImageKey(newArtist.GentreGroup, SMGentresBase.GentreClassType.Category, newArtist.GentreCategory),
							ico2: gentres.GetImageKey(newArtist.GentreGroup, SMGentresBase.GentreClassType.Content, newArtist.GentreContent)
							);
					}
				}
				else
				{
					// Редактируем элемент
					artist.Assign(artistWindow.ArtistObj);
					ArtistView.Edit(
						artist.ID,
						gentres.GetImageKey(artist.GentreGroup, SMGentresBase.GentreClassType.GentreGroup, 0),
						labelText1: artist.Name,
						labelText2: artist.CompanyName,
						ico0: gentres.GetImageKey(artist.GentreGroup, SMGentresBase.GentreClassType.Age, artist.GentreAge),
						ico1: gentres.GetImageKey(artist.GentreGroup, SMGentresBase.GentreClassType.Category, artist.GentreCategory),
						ico2: gentres.GetImageKey(artist.GentreGroup, SMGentresBase.GentreClassType.Content, artist.GentreContent)
						);
					ItemSelectionChange(ArtistView);
				}
			}
		}

		public void PanelGroupClick(string panelName)
		{
			selectedPanelName = panelName;
			// Сюда добавить код отображения артистов группы
			RefreshArtistView();
		}
		public void PanelGroupAdd(string panelName)
		{
			currentProject.GroupsArtist.Add(new SMGroup(panelName));
		}

		public void PanelGroupRename(string oldName, string newName)
		{
			// Ищем группу
			SMGroup getGroup = currentProject.GetGroup(SMProject.GroupType.Artist, oldName);
			if (getGroup != null)
				getGroup.Name = newName;
		}

		public void PanelGroupDelete(string panelName)
		{
			SMGroup getGroup = currentProject.GetGroup(SMProject.GroupType.Artist, panelName);
			if (getGroup != null)
			{
				getGroup.Clear();
				currentProject.GroupsArtist.Remove(getGroup);
			}
		}

		public void DropItems(int insertIndex, SMListViewItem draggedItem, Object draggedTo, Object draggedToSubItem)
		{
			var lView = draggedItem.dragFromControl as SMListView;
			var items = lView.Items as ObservableCollection<SMListViewItem>;

			// Удаление в корзину
			if (draggedTo.GetHashCode() == ArtistToolBar.GetHashCode())
			{
				SMListView.CreateHelperList(draggedItem.selectedItems, helpList1, false);

				// Удаление из контрола и элемента данных
				foreach (SMListViewItem lvi in helpList1)
				{
					if (lView.GetHashCode() == ArtistView.GetHashCode())
					{
						// Жанровые группы
						long artistID = lvi.ItemID;
						SMArtist getArtist = currentProject.GetArtistByID(artistID);
						if (getArtist != null)
							currentProject.Artists.Remove(getArtist);

						SMGroup getGroup = currentProject.GetGroup(SMProject.GroupType.Artist, selectedPanelName);
						if (getGroup != null)
						{
							getGroup.Remove(artistID);
							TrackView.Clear();
						}
					}
					items.Remove(lvi);
				}

			}
			// Перемещение внутри контрола
			else if (draggedTo.GetHashCode() == lView.GetHashCode())
			{
				// Перемещение внутри контрола
				SMListView.CreateHelperList(draggedItem.selectedItems, helpList1, true);
				SMListView.CreateHelperList(draggedItem.selectedItems, helpList2, false);
				foreach (SMListViewItem lvi in helpList2)
				{
					items.Remove(lvi);
				}

				SMGroup getGroup = currentProject.GetGroup(SMProject.GroupType.Artist, selectedPanelName);
				foreach (SMListViewItem lvi in helpList1)
				{
					if (lView.GetHashCode() == ArtistView.GetHashCode())
					{
						if (getGroup != null)
						{
							getGroup.Move(lvi.ItemID, insertIndex);
						}
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
			// Перемещение в панель группы
			else if (draggedTo.GetHashCode() == ArtistPanel.GetHashCode())
			{
				var dropPanel = draggedToSubItem as SMGroupsItem;

				// Создаем помощников
				SMListView.CreateHelperList(draggedItem.selectedItems, helpList2, false);

				SMGroup getGroup = currentProject.GetGroup(SMProject.GroupType.Artist, selectedPanelName);
				SMGroup newGroup = currentProject.GetGroup(SMProject.GroupType.Artist, dropPanel.Text);

				// Удаляем из текущей панели и группы и добавляем в новую
				foreach (SMListViewItem lvi in helpList2)
				{
					if (getGroup != null)
					{
						getGroup.Remove(lvi.ItemID);
					}
					if (newGroup != null)
					{
						newGroup.Add(lvi.ItemID);
					}
					items.Remove(lvi);

					// Если та же группа - вставляем для отображения
					if (getGroup.GetHashCode() == newGroup.GetHashCode())
					{
						items.Add(lvi);
					}
				}
			}
		}
		#endregion

		// Отображение элементов при изменении группы
		private void RefreshArtistView()
		{
			ArtistView.Clear();
			TrackView.Clear();

			SMGroup getGroup = currentProject.GetGroup(SMProject.GroupType.Artist, selectedPanelName);
			if (getGroup == null)
				return;
			foreach (long artistID in getGroup.IDList)
			{
				SMArtist getArtist = currentProject.GetArtistByID(artistID);
				ArtistView.Add(
					getArtist.ID,
					gentres.GetImageKey(getArtist.GentreGroup,SMGentresBase.GentreClassType.GentreGroup, 0),
					labelText1: getArtist.Name,
					labelText2: getArtist.CompanyName,
					ico0: gentres.GetImageKey(getArtist.GentreGroup, SMGentresBase.GentreClassType.Age, getArtist.GentreAge),
					ico1: gentres.GetImageKey(getArtist.GentreGroup, SMGentresBase.GentreClassType.Category, getArtist.GentreCategory),
					ico2: gentres.GetImageKey(getArtist.GentreGroup, SMGentresBase.GentreClassType.Content, getArtist.GentreContent)
					);
			}
		}

		// Инициализация библиотеки изображений
		private void ImgLibInit()
		{
			App curApp = (App)Application.Current;
			curApp.ImgPath = new Dictionary<int, string>();
			curApp.ImgDesc = new Dictionary<int, string>();


			// Жанры
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


			// Возрастные группы
			curApp.ImgPath.Add(131, "/ShowManager;component/Images/Gentres/age-0.png");
			curApp.ImgPath.Add(132, "/ShowManager;component/Images/Gentres/age-6.png");
			curApp.ImgPath.Add(135, "/ShowManager;component/Images/Gentres/age-10.png");
			curApp.ImgPath.Add(137, "/ShowManager;component/Images/Gentres/age-16.png");
			curApp.ImgPath.Add(139, "/ShowManager;component/Images/Gentres/age-20.png");
			curApp.ImgPath.Add(140, "/ShowManager;component/Images/Gentres/age-26.png");
			curApp.ImgPath.Add(141, "/ShowManager;component/Images/Gentres/age-40.png");
			curApp.ImgPath.Add(142, "/ShowManager;component/Images/Gentres/age-mix.png");

			curApp.ImgDesc.Add(131, "Возраст 0+");
			curApp.ImgDesc.Add(132, "Возраст 6+");
			curApp.ImgDesc.Add(135, "Возраст 10+");
			curApp.ImgDesc.Add(137, "Возраст 16+");
			curApp.ImgDesc.Add(139, "Возраст 20+");
			curApp.ImgDesc.Add(140, "Возраст 26+");
			curApp.ImgDesc.Add(141, "Возраст 40+");
			curApp.ImgDesc.Add(142, "Возраст смешанный");

			// Категории
			curApp.ImgPath.Add(151, "/ShowManager;component/Images/Gentres/category-0.png");
			curApp.ImgPath.Add(152, "/ShowManager;component/Images/Gentres/category-1.png");
			curApp.ImgPath.Add(153, "/ShowManager;component/Images/Gentres/category-2.png");

			curApp.ImgDesc.Add(151, "Кат. начинающий");
			curApp.ImgDesc.Add(152, "Кат. любитель");
			curApp.ImgDesc.Add(153, "Кат. профессионал");

			// Состав
			curApp.ImgPath.Add(171, "/ShowManager;component/Images/Gentres/content-solo.png");
			curApp.ImgPath.Add(172, "/ShowManager;component/Images/Gentres/content-double.png");
			curApp.ImgPath.Add(173, "/ShowManager;component/Images/Gentres/content-triple.png");
			curApp.ImgPath.Add(174, "/ShowManager;component/Images/Gentres/content-quad.png");
			curApp.ImgPath.Add(175, "/ShowManager;component/Images/Gentres/content-ensemble.png");

			curApp.ImgDesc.Add(171, "Состав соло");
			curApp.ImgDesc.Add(172, "Состав дуэт");
			curApp.ImgDesc.Add(173, "Состав трио");
			curApp.ImgDesc.Add(174, "Состав квартет");
			curApp.ImgDesc.Add(175, "Состав ансамбль");

			// Дополнительно
			curApp.ImgPath.Add(201, "/ShowManager;component/Images/View/user.png");
			curApp.ImgPath.Add(202, "/ShowManager;component/Images/View/music.png");

			curApp.ImgDesc.Add(201, "Пользователь");
			curApp.ImgDesc.Add(202, "Музыка");

			curApp.ImgPath.Add(221, "/ShowManager;component/Images/View/in-show-no.png");
			curApp.ImgPath.Add(222, "/ShowManager;component/Images/View/in-show-half.png");
			curApp.ImgPath.Add(223, "/ShowManager;component/Images/View/in-show-yes.png");

			curApp.ImgDesc.Add(221, "Выступление нет");
			curApp.ImgDesc.Add(222, "Выступление частично");
			curApp.ImgDesc.Add(223, "Выступление да");

		}

	}
}
