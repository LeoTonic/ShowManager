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
using System.ComponentModel;

namespace ShowManager.Controls
{
	/// <summary>
	/// Логика взаимодействия для SMGroupsItem.xaml
	/// </summary>
	public partial class SMGroupsItem : UserControl, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private const string NewGroupValue = "Новая группа";

		private SMGroupsPanel parentPanel = null;

		public enum GroupImageType
		{
			Closed = 0,
			Opened,
			AddNew
		}

		private string text;
		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs("Text"));
			}
		}
		private GroupImageType grpImage;
		public GroupImageType FolderImage
		{
			get
			{
				return grpImage;
			}
			set
			{
				string path = "folder.png";
				switch (value)
				{
					case GroupImageType.Closed:
						path = "folder.png";
						textLabel.FontWeight = FontWeights.Normal;
						break;
					case GroupImageType.Opened:
						path = "folder-active.png";
						textLabel.FontWeight = FontWeights.Bold;
						break;
					case GroupImageType.AddNew:
						path = "folder-add.png";
						textLabel.FontWeight = FontWeights.Normal;
						break;
				}
				this.grpImage = value;

				if (value == GroupImageType.AddNew)
				{
					ContextMenu.Visibility = Visibility.Collapsed;
				}
				else
				{
					ContextMenu.Visibility = Visibility.Visible;
					ContextMenu.IsOpen = false;
				}

				this.imgPath = "/ShowManager;component/Images/Tools/" + path;
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs("ImagePath"));
			}
		}

		private string imgPath;
		public string ImagePath
		{
			get
			{
				return this.imgPath;
			}
		}

		public SMGroupsItem()
		{
			InitializeComponent();
			DataContext = this;
		}

		// Присвоение родительского хэндлера
		public void AssignParent(SMGroupsPanel panel)
		{
			this.parentPanel = panel;
		}

		// Редактируем имя группы
		public void EditName()
		{
			editTextBox.Visibility = Visibility.Visible;
			textLabel.Visibility = Visibility.Collapsed;

			if (string.IsNullOrWhiteSpace(this.text))
			{
				editTextBox.Text = NewGroupValue;
			}
			else
			{
				editTextBox.Text = this.text;
			}
			editTextBox.Focus();
		}


		// Потеря фокуса для текстового бокса редактирования имени
		private void TextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			var tb = sender as TextBox;
			if (string.IsNullOrWhiteSpace(tb.Text))
			{
				MessageBox.Show("Нельзя присвоить группе пустое имя!", "Пустое имя!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				tb.Text = NewGroupValue;
			}
			this.Text = tb.Text;
			editTextBox.Visibility = Visibility.Collapsed;
			textLabel.Visibility = Visibility.Visible;

			if (this.parentPanel != null)
				this.parentPanel.GroupNameChanged(this);
		}
		// Получение фокуса - выделение содержимого
		private void TextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			var tb = sender as TextBox;
			tb.SelectAll();
		}

		// Отпуск клавиши
		private void TextBox_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				TextBox_LostFocus(sender, null);
			}
		}

		// Контекстное меню переименование группы
		private void Rename_Click(object sender, RoutedEventArgs e)
		{
			EditName();
		}

		// Контекстное меню удаление группы
		private void Delete_Click(object sender, RoutedEventArgs e)
		{
			if (this.parentPanel != null)
			{
				if (this.parentPanel.Items.Count <= 2)
				{
					MessageBox.Show("Нельзя удалить единственную группу!", "Единственная группа", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					return;
				}
			}

			// Подтверждение
			if (MessageBox.Show("Удаляем текущую группу?", "Удаляем?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				if (this.parentPanel != null)
				{
					this.parentPanel.GroupDelete(this);
				}
			}
		}
	}
}
