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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShowManager.Controls
{
	/// <summary>
	/// Логика взаимодействия для SMGroupsPanel.xaml
	/// </summary>
	public partial class SMGroupsPanel : UserControl
	{
		public ObservableCollection<SMGroupsItem> Items { get; set; }

		private SMGroupsItem selectedFolder = null;
		private SMListView connectedView = null;

		public SMGroupsPanel()
		{
			InitializeComponent();

			Items = new ObservableCollection<SMGroupsItem>(new List<SMGroupsItem>
			{
				new SMGroupsItem { Text = "Основная группа", FolderImage = SMGroupsItem.GroupImageType.Opened },
			});
			Items[0].AssignParent(this);
			SelectGroup(Items[0]);
			AddNewGroupMarker();

			DataContext = this;
		}

		// Присвоение вида к панели
		public void AssignView(SMListView view)
		{
			connectedView = view;
		}

		protected void Group_MouseLeftDown(object sender, MouseButtonEventArgs e)
		{
			if (sender == null)
				return;

			var grpItem = sender as SMGroupsItem;
			if (grpItem.FolderImage == SMGroupsItem.GroupImageType.AddNew)
			{
				// Создаем новое имя
				EditGroupName(sender);
				return;
			}

			if (selectedFolder != grpItem)
			{
				SelectGroup(grpItem);
			}
		}
		protected void Group_MouseDblClick(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
				EditGroupName(sender);
		}

		// Редактируем имя
		private void EditGroupName(object sender)
		{
			if (sender == null)
				return;

			var grpItem = sender as SMGroupsItem;
			grpItem.EditName();
		}

		// Изменили имя элемента
		public void GroupNameChanged(SMGroupsItem item)
		{
			// Проверяем на именование новой вкладки
			if (item.FolderImage == SMGroupsItem.GroupImageType.AddNew)
			{
				SelectGroup(item);
				// Добавляем новую
				AddNewGroupMarker();
			}
		}

		// Добавляем маркер новой группы AddNew
		private void AddNewGroupMarker()
		{
			SMGroupsItem newItem = new SMGroupsItem { Text = "", FolderImage = SMGroupsItem.GroupImageType.AddNew };
			newItem.AssignParent(this);
			Items.Add(newItem);
		}

		// Удаление имени элемента
		public void GroupDelete(SMGroupsItem item)
		{
			Items.Remove(item);
			SelectGroup(Items[0]);
		}

		// Выбираем группу
		private void SelectGroup(SMGroupsItem item)
		{
			// Закрываем старую группу
			if (selectedFolder != null)
			{
				selectedFolder.FolderImage = SMGroupsItem.GroupImageType.Closed;
			}
			selectedFolder = item;
			selectedFolder.FolderImage = SMGroupsItem.GroupImageType.Opened;
		}
	}
}
