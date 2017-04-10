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

		public ImageSelector(string curImage)
		{
			InitializeComponent();
			this.LostFocus += ImageSelector_LostFocus;

			Items = new ObservableCollection<ImageItem>(new List<ImageItem>
			{
				new ImageItem{ ImagePath = "vocal", ImageName = "Вокал" },
				new ImageItem{ ImagePath = "dance", ImageName = "Хореография" },
				new ImageItem{ ImagePath = "theatre", ImageName = "Театр" },
				new ImageItem{ ImagePath = "music", ImageName = "Инструмент." },
				new ImageItem{ ImagePath = "art", ImageName = "ИЗО" },
				new ImageItem{ ImagePath = "concert-master", ImageName = "Концертмейстер" },
				new ImageItem{ ImagePath = "patriot", ImageName = "Патриот. песня" },
				new ImageItem{ ImagePath = "pop", ImageName = "Эстрада" },
				new ImageItem{ ImagePath = "guitar", ImageName = "Авторская песня" },
				new ImageItem{ ImagePath = "palette", ImageName = "ДПИ" },
				new ImageItem{ ImagePath = "director", ImageName = "Конферансье" },
				new ImageItem{ ImagePath = "cinema", ImageName = "Кино и анимация" },
				new ImageItem{ ImagePath = "sport", ImageName = "Худ.гимнастика" },
				new ImageItem{ ImagePath = "circus", ImageName = "Цирк" }
			});
			ImagesView.ItemsSource = Items;

			selectedItem = GetItem(curImage);
			selectedItem.StackColor = "LightBlue";
		}

		private void ImageSelector_LostFocus(object sender, RoutedEventArgs e)
		{
			this.Close();
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

			return Items[0];
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
					this.imgPath = "/ShowManager;component/Images/Gentres/" + value + ".png";
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
			public ImageItem()
			{
				this.stackColor = "White";
			}
		}
	}
}
