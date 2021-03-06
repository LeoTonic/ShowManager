﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using ShowManager.Controls;
using ShowManager.Models;
using ShowManager.Tools;
using Microsoft.Win32;

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
		ShowOrder wndOrdersPrep;
		public bool isOrderPrepActive = false;

		// Окно фильтрации
		FilterSelector wndFilterSelector;
		public bool isFilterSelectorActive = false;

		public FilterView filterView; // Настройки фильтра

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
			currentProject = new SMProject(gentres)
			{
				TrackFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
			};
			Title = currentProject.Name;

			filterView = new FilterView(gentres);

			// Инициализация окон
			wndDragDrop = new DragDropWindow();
			wndDragDrop.HideWindow();

			ArtistView.SetDragDropWindow(wndDragDrop);
			ArtistView.SetViewMode(SMListView.ViewMode.ArtistName, this);
			ArtistPanel.Initialize(null, this, false, currentProject.GroupsArtist);
			ArtistToolBar.SetValues("user-add", "Добавить исполнителя", "user-edit", "Изменить исполнителя", "user-delete", "Удалить исполнителя", this);

			TrackView.SetDragDropWindow(wndDragDrop);
			TrackView.SetViewMode(SMListView.ViewMode.ArtistTrack, this);

			OpenWindowShow(true);
			OpenWindowPrep(true);

			Closed += MainWindow_Closed;

			MenuWindowArrange_Click(null, null);
		}

		// Отображение окна выступлений
		private void OpenWindowShow(bool mode)
		{
			if (!isOrderShowActive && mode)
			{
				wndOrdersShow = new ShowOrder(wndDragDrop, currentProject, this, gentres, SMListView.ViewMode.OrderTrack);
				wndOrdersShow.Show();
				isOrderShowActive = true;
				MenuWindowShow.IsChecked = true;
			}
			else if (isOrderShowActive && !mode)
			{
				if (wndOrdersShow != null)
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
				wndOrdersPrep = new ShowOrder(wndDragDrop, currentProject, this, gentres, SMListView.ViewMode.OrderArtist);
				wndOrdersPrep.Show();
				isOrderPrepActive = true;
				MenuWindowPrep.IsChecked = true;
			}
			else if (isOrderPrepActive && !mode)
			{
				if (wndOrdersPrep != null)
					wndOrdersPrep.Close();
				wndOrdersPrep = null;
				isOrderPrepActive = false;
				MenuWindowPrep.IsChecked = false;
			}
		}

		// Отображение окна фильтра
		private void OpenFilterSelector(bool mode)
		{
			if (!isFilterSelectorActive && mode)
			{
				wndFilterSelector = new FilterSelector(this);

				Point pnt = FilterButton.PointToScreen(new Point(0, 0));
				wndFilterSelector.Left = pnt.X + FilterButton.Width - wndFilterSelector.Width;
				wndFilterSelector.Top = pnt.Y + FilterButton.Height;
				wndFilterSelector.Show();
				isFilterSelectorActive = true;
			}
			else if (isFilterSelectorActive && !mode)
			{
				if (wndFilterSelector != null)
				{
					wndFilterSelector.Close();
					wndFilterSelector = null;
					isFilterSelectorActive = false;
				}
			}

		}

		private void MainWindow_Closed(object sender, EventArgs e)
		{
			OpenWindowShow(false);
			OpenWindowPrep(false);

			wndDragDrop.Close();
			if (wndFilterSelector != null)
				wndFilterSelector.Close();
		}

    // Подтверждение сохрания текущего проекта
    private MessageBoxResult ConfirmClose() { return MessageBox.Show("Текущий проект не сохранен! Закрыть его без сохранения?", "Проект не сохранен", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No); }

		#region Menu Calls

		// Открытие проекта
		private void Menu_File_Open(object sender, RoutedEventArgs e)
		{
      if (currentProject.IsDirty && ConfirmClose() != MessageBoxResult.Yes) { return; }

			var ofd = new OpenFileDialog()
			{
				Filter = "Файлы фестивалей (*.smp)|*.smp",
				Title = "Открыть проект фестиваля"
			};
			if (ofd.ShowDialog() == true)
			{
				var filePath = ofd.FileName;

				var dio = new Tools.DataIO();
				if (dio.OpenRead(filePath))
				{
					if (currentProject.IOLoad(dio))
					{
						ArtistPanel.SetGroupTabs(currentProject.GroupsArtist, false);
						ArtistPanel.SelectFirstGroup();

						wndOrdersShow.ShowOrderPanel.SetGroupTabs(currentProject.GroupsShow, false);
						wndOrdersShow.ShowOrderPanel.SelectFirstGroup();

						wndOrdersPrep.ShowOrderPanel.SetGroupTabs(currentProject.GroupsPrepare, false);
						wndOrdersPrep.ShowOrderPanel.SelectFirstGroup();
						Title = currentProject.Name;
            currentProject.FilePath = filePath;
					}
				}
				else
				{
					MessageBox.Show("System Error!");
				}
			}
		}

    // Сохранение проекта - процесс
    private void SaveProject()
    {
      var dio = new Tools.DataIO();
      if (dio.OpenWrite(currentProject.FilePath))
      {
        currentProject.IOSave(dio);
        dio.CloseWrite();
      }
      currentProject.SetDirty(false);
    }

    // Сохранение проекта
    private void Menu_File_Save(object sender, RoutedEventArgs e)
		{
      if (String.IsNullOrWhiteSpace(currentProject.FilePath))
      {
        // Отправка в SaveAs
        Menu_File_SaveAs(sender, e);
      }
      else
      {
        SaveProject();
      }
		}

    private void Menu_File_SaveAs(object sender, RoutedEventArgs e)
    {
      var sfd = new SaveFileDialog()
      {
        Filter = "Файлы фестивалей (*.smp)|*.smp",
        FileName = currentProject.Name,
        Title = "Сохранить проект фестиваля"
      };
      if (sfd.ShowDialog() == true)
      {
        currentProject.FilePath = sfd.FileName;
        SaveProject();
      }
    }

    // Запуск окна свойств фестиваля
    private void Menu_File_Properties(object sender, RoutedEventArgs e)
		{
			var propsWindow = new ShowProperties(currentProject);
			if (propsWindow.ShowDialog() == true)
			{
				;
			}
		}

		// Выход из приложения
		public void Menu_Show_Exit(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
		
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
						TrackView.Add(track, gentres, -1, true);
					}
				}

				// Обновление данных в порядках репетиций и выступлений
				if (wndOrdersShow != null)
				{
					wndOrdersShow.RefreshView();
				}
				if (wndOrdersPrep != null)
				{
					wndOrdersPrep.RefreshView();
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
        currentProject.SetDirty();

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
          currentProject.SetDirty();

					// И в группу
					SMGroup getGroup = currentProject.GetGroup(SMProject.GroupType.Artist, selectedPanelName);
					if (getGroup != null)
					{
						getGroup.Add(newArtist.ID, -1);
						ArtistView.Add(newArtist, gentres);
					}
				}
				else
				{
					// Редактируем элемент
					artist.Assign(artistWindow.ArtistObj);
          currentProject.SetDirty();
					ArtistView.Edit(artist.ID, artist, gentres);
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
      currentProject.SetDirty();
		}

		public void PanelGroupRename(string oldName, string newName)
		{
			// Ищем группу
			SMGroup getGroup = currentProject.GetGroup(SMProject.GroupType.Artist, oldName);
			if (getGroup != null)
			{
				getGroup.Name = newName;
        currentProject.SetDirty();

				// Если текущая панель была выделена
				if (selectedPanelName == oldName)
					selectedPanelName = newName;

			}
		}

		public void PanelGroupDelete(string panelName)
		{
			SMGroup getGroup = currentProject.GetGroup(SMProject.GroupType.Artist, panelName);
			if (getGroup != null)
			{
				// Очистим группы выступлений репетиций

				// Создаем вспомогательный список идентификаторов
				List<long> idList = new List<long>();
				getGroup.CloneList(idList);
				foreach (long artistID in idList)
				{
					currentProject.RemoveArtist(artistID);
				}
				currentProject.GroupsArtist.Remove(getGroup);
        currentProject.SetDirty();

				wndOrdersShow.RefreshView();
				wndOrdersPrep.RefreshView();
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
						// Удаление из списка артистов
						long artistID = lvi.ItemID;
						SMArtist getArtist = currentProject.GetArtistByID(artistID);
						if (getArtist != null)
            {
              currentProject.Artists.Remove(getArtist);
              currentProject.SetDirty();
            }

            SMGroup getGroup = currentProject.GetGroup(SMProject.GroupType.Artist, selectedPanelName);
						if (getGroup != null)
						{
							getGroup.Remove(artistID);
              currentProject.SetDirty();
							TrackView.Clear();
						}
					}
					else if (lView.GetHashCode() == TrackView.GetHashCode())
					{
						// Удаление трека у артиста
						long trackID = lvi.ItemID;
						SMTrack getTrack = currentProject.GetTrackByID(trackID);
						if (getTrack != null)
						{
							currentProject.RemoveTrack(trackID);
              currentProject.SetDirty();

							// Удаляем трек у артиста
							SMArtist getArtist = getTrack.ParentArtist;
							getArtist.Tracks.Remove(getTrack);
							// Обновляем вид
							ArtistView.Edit(getArtist.ID, getArtist, gentres);
							wndOrdersShow.RefreshView();
							wndOrdersPrep.RefreshView();
						}
					}
					else if (lView.GetHashCode() == wndOrdersShow.ShowOrderView.GetHashCode())
					{
						// Удаление из порядка выступлений
						SMGroup getGroup = currentProject.GetGroup(SMProject.GroupType.Show, wndOrdersShow.selectedPanelName);
						if (getGroup != null)
						{
							var trackID = lvi.ItemID;
							getGroup.Remove(trackID);
              currentProject.SetDirty();
							// Проверяем на наличие во всех группах
							if (!currentProject.IsTrackAppliedInShow(trackID))
							{
								// Если не обнаружен - убираем свойство наличия на выступлении
								SMTrack track = currentProject.GetTrackByID(trackID);
								if (track != null)
								{
									track.IsApplied = false;
									UpdateTrackStatus(track, track.ParentArtist);
								}
							}
						}
					}
					else if (lView.GetHashCode() == wndOrdersPrep.ShowOrderView.GetHashCode())
					{
						// Удаление из порядка репетиций
						SMGroup getGroup = currentProject.GetGroup(SMProject.GroupType.Prepare, wndOrdersPrep.selectedPanelName);
						if (getGroup != null)
						{
							var artistID = lvi.ItemID;
							getGroup.Remove(artistID);
              currentProject.SetDirty();
						}
					}
					items.Remove(lvi);
				}

				// Обновление хронометражей
				if (lView.GetHashCode() == wndOrdersShow.ShowOrderView.GetHashCode())
				{
					SMGroup getGroup = currentProject.GetGroup(SMProject.GroupType.Show, wndOrdersShow.selectedPanelName);
					wndOrdersShow.UpdateTimeLine(getGroup);
				}
				else if (lView.GetHashCode() == wndOrdersPrep.ShowOrderView.GetHashCode())
				{
					SMGroup getGroup = currentProject.GetGroup(SMProject.GroupType.Prepare, wndOrdersPrep.selectedPanelName);
					wndOrdersPrep.UpdateTimeLine(getGroup);
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
              currentProject.SetDirty();
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
            currentProject.SetDirty();
					}
					if (newGroup != null)
					{
						newGroup.Add(lvi.ItemID, -1);
            currentProject.SetDirty();
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

		// Нажатие кнопки настройки фильтра
		private void Filter_Click(object sender, RoutedEventArgs e)
		{
			OpenFilterSelector(!isFilterSelectorActive);
		}

		// Отображение элементов при изменении группы
		public void RefreshArtistView(bool useFilter = false)
		{
			ArtistView.Clear();
			TrackView.Clear();

			SMGroup getGroup = currentProject.GetGroup(SMProject.GroupType.Artist, selectedPanelName);
			if (getGroup == null)
				return;

			foreach (long artistID in getGroup.IDList)
			{
				SMArtist getArtist = currentProject.GetArtistByID(artistID);
        if (useFilter)
          ArtistView.Add(getArtist, gentres, filterView);
        else
  				ArtistView.Add(getArtist, gentres);
			}
		}

    // Обновление статуса треков в контролах
    public void UpdateTrackStatus(SMTrack track, SMArtist artist)
		{
			ArtistView.Edit(artist.ID, artist, gentres);
			TrackView.Edit(track.ID, track, gentres);
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
