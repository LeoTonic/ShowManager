﻿using System;
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
using ShowManager.Models;

namespace ShowManager.Controls
{
	/// <summary>
	/// Логика взаимодействия для SMGroupsPanel.xaml
	/// </summary>
	public partial class SMGroupsPanel : UserControl
	{
		public ObservableCollection<SMGroupsItem> Items { get; set; }

		private SMGroupsItem selectedFolder = null;

		private ICommandCatcher iCommandTo = null;
		private string groupNameOld = null;
		private bool modeReadOnly;

		public SMGroupsPanel()
		{
			InitializeComponent();

			Items = new ObservableCollection<SMGroupsItem>();

			DataContext = this;
		}

		public void Initialize(List<string> groupNames, ICommandCatcher iTo, bool readOnly, List<SMGroup> connectList = null)
		{
			iCommandTo = iTo;
			modeReadOnly = readOnly;

			if (groupNames == null)
			{
				Items.Clear();

				if (connectList != null)
				{
					SetGroupTabs(connectList, readOnly);
				}
				else
				{
					Items.Add(new SMGroupsItem { Text = "Основная группа", FolderImage = SMGroupsItem.GroupImageType.Opened });
					Items[0].AssignParent(this, readOnly);
				}
			}
			else
			{
				foreach(string s in groupNames)
				{
					SMGroupsItem newItem = new SMGroupsItem { Text = s, FolderImage = SMGroupsItem.GroupImageType.Closed };
					newItem.AssignParent(this, readOnly);
					Items.Add(newItem);
				}
			}
			if (!readOnly)
			{
				AddNewGroupMarker();
			}

			SelectGroup(Items[0]);
		}

		// Формирование вкладок по списку
		public void SetGroupTabs(List<SMGroup> connectList, bool readOnly)
		{
			Items.Clear();
			foreach (SMGroup grp in connectList)
			{
				var newItem = new SMGroupsItem { Text = grp.Name, FolderImage = SMGroupsItem.GroupImageType.Closed };
				newItem.AssignParent(this, readOnly);
				// Добавляем возможность дропа элементов
				newItem.AllowDrop = true;
				newItem.Drop += PanelItemDrop;
				Items.Add(newItem);
			}

			if (!readOnly)
			{
				AddNewGroupMarker();
			}

		}

		// Сброс элементов в заголовок панели
		private void PanelItemDrop(object sender, DragEventArgs e)
		{
			if (iCommandTo != null)
			{
				var panelItem = sender as SMGroupsItem;
				iCommandTo.DropItems(-1, e.Data.GetData(typeof(SMListViewItem)) as SMListViewItem, this, panelItem);
			}
		}

		protected void Group_MouseLeftDown(object sender, MouseButtonEventArgs e)
		{
			if (sender == null)
				return;

			var grpItem = sender as SMGroupsItem;
			if (grpItem.FolderImage == SMGroupsItem.GroupImageType.AddNew)
			{
				grpItem.AssignParent(this, false);
				grpItem.AllowDrop = true;
				grpItem.Drop += PanelItemDrop;

				// Создаем новое имя
				EditGroupName(sender, true);
				return;
			}

			if (selectedFolder != grpItem)
			{
				SelectGroup(grpItem);
			}
		}

		protected void Group_MouseDblClick(object sender, MouseButtonEventArgs e)
		{
			var grpItem = sender as SMGroupsItem;
			if (grpItem.FolderImage == SMGroupsItem.GroupImageType.AddNew || modeReadOnly)
				return;

			if (e.LeftButton == MouseButtonState.Pressed)
				EditGroupName(sender, false);
		}

		// Редактируем имя
		public void EditGroupName(object sender, bool newGroup)
		{
			if (sender == null)
				return;

			var grpItem = sender as SMGroupsItem;
			groupNameOld = grpItem.Text;
			grpItem.EditName(newGroup);
		}

		// Изменили имя элемента
		public void GroupNameChanged(SMGroupsItem item, bool newGroup)
		{
			if (iCommandTo != null)
			{
				if (newGroup)
				{
					iCommandTo.PanelGroupAdd(item.Text);
				}
				else
				{					
					iCommandTo.PanelGroupRename(groupNameOld, item.Text);
				}
			}
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
			// Проверка на наличие маркера новой группы
			var existFlag = false;
			foreach(SMGroupsItem item in Items)
			{
				if (item.FolderImage == SMGroupsItem.GroupImageType.AddNew)
					existFlag = true;
			}
			if (!existFlag)
			{
				SMGroupsItem newItem = new SMGroupsItem { Text = "", FolderImage = SMGroupsItem.GroupImageType.AddNew };
				newItem.AssignParent(this, true);
				newItem.AllowDrop = false;
				Items.Add(newItem);
			}
		}

		// Удаление имени элемента
		public void GroupDelete(SMGroupsItem item)
		{
			string removeName = item.Text;
			Items.Remove(item);
			if (iCommandTo != null)
			{
				iCommandTo.PanelGroupDelete(removeName);
			}

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
			if (iCommandTo != null)
			{
				iCommandTo.PanelGroupClick(item.Text);
			}
		}

		// Внешний вызов - выбор первой группы
		public void SelectFirstGroup()
		{
			if (Items.Count == 0)
				return;
			SelectGroup(Items[0]);
		}
	}
}
