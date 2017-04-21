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
using System.Windows.Shapes;

namespace ShowManager.Controls
{
	/// <summary>
	/// Логика взаимодействия для ImageSelector.xaml
	/// </summary>
	public partial class ImageSelector : Window
	{
		public ObservableCollection<ImageItem> Items { get; set; }

		public ImageItem selectedItem = null;

		public ImageSelector(int imageKey)
		{
			InitializeComponent();

			Items = new ObservableCollection<ImageItem>();
			App curApp = (App)Application.Current;
			foreach(int key in curApp.ImgPath.Keys)
			{
				ImageItem newItem = new ImageItem { ImagePath = curApp.ImgPath[key], ImageName = curApp.ImgDesc[key], ImageKey = key };
				Items.Add(newItem);
			}

			ImagesView.ItemsSource = Items;

			selectedItem = GetItem(curApp.ImgPath[imageKey]);
			selectedItem.StackColor = "LightBlue";
		}

		private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			var item = sender as Image;
			selectedItem = GetItem(item.Tag.ToString());
			this.Close();
		}

		private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
		{
			var item = sender as TextBlock;
			selectedItem = GetItem(item.Tag.ToString());
			this.Close();
		}

		// Получение элемента
		private ImageItem GetItem(string imgPath)
		{
			foreach (ImageItem item in Items)
			{
				if (item.ImagePath == imgPath)
				{
					return item;
				}
			}

			return Items.First();
		}

		// Элемент управления StackPanel
		public class ImageItem
		{
			private string imgName;
			public string ImageName
			{
				get
				{
					return this.imgName;
				}
				set
				{
					this.imgName = value;
				}
			}

			private string imgPath;
			public string ImagePath
			{
				get
				{
					return imgPath;
				}
				set
				{
					this.imgPath = value;
				}
			}

			private string stackColor;
			public string StackColor
			{
				get
				{
					return this.stackColor;
				}
				set
				{
					this.stackColor = value;
				}
			}

			private int imgKey;
			public int ImageKey
			{
				get
				{
					return imgKey;
				}
				set
				{
					imgKey = value;
				}
			}

			public ImageItem()
			{
				this.stackColor = "White";
			}
		}
	}
}
