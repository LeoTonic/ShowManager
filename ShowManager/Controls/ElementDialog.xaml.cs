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
using System.ComponentModel;

namespace ShowManager.Controls
{
	/// <summary>
	/// Логика взаимодействия для ElementDialog.xaml
	/// </summary>
	public partial class ElementDialog : Window, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public bool IsSet = false;

		private string imagePath;
		public string ImagePath
		{
			get
			{
				return imagePath;
			}
			set
			{
				imagePath = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImagePath"));
			}
		}

		private string elemName;
		public string ElementName
		{
			get
			{
				return elemName;
			}
			set
			{
				elemName = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ElementName"));

			}
		}

		private int imageKey;
		public int ImageKey
		{
			get
			{
				return imageKey;
			}
			set
			{
				imageKey = value;
				App curApp = (App)Application.Current;
				ImagePath = curApp.ImgPath[imageKey];
			}
		}

		public ElementDialog(int key, string eName)
		{
			InitializeComponent();

			DataContext = this;
			ImageKey = key;

			ElementName = eName;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			IsSet = true;
			this.Close();
		}

		private void TextBox_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				if (string.IsNullOrWhiteSpace(ElementName))
				{
					return;
				}
				else
				{
					IsSet = true;
					this.Close();
				}
			}
		}

		private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			var element = sender as UIElement;

			// Отображение ImageSelectora
			ImageSelector imgSelect = new ImageSelector(ImageKey);
			Point pnt = element.PointToScreen(new Point(0,0));
			imgSelect.Left = pnt.X + 40;
			imgSelect.Top = pnt.Y + 8;
			imgSelect.ShowDialog();
			if (imgSelect.selectedItem != null) {
				ImageKey = imgSelect.selectedItem.ImageKey;
			}
		}
	}
}
