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
	/// Логика взаимодействия для SMListViewItem.xaml
	/// </summary>
	public partial class SMListViewItem : UserControl, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private SMListView.ViewMode viewMode;

		public SMListViewItem(SMListView.ViewMode vm)
		{
			InitializeComponent();
			DataContext = this;
			viewMode = vm;

			switch (viewMode)
			{
				case SMListView.ViewMode.Gentre:
					// hide unused blocks
					MainTimeBlock.Visibility = Visibility.Collapsed;
					SubTimeBlock.Visibility = Visibility.Collapsed;
					TwoLineBlock.Visibility = Visibility.Collapsed;
					IconsBlock.Visibility = Visibility.Collapsed;

					// show used blocks
					OneLineBlock.Visibility = Visibility.Visible;
					break;

				case SMListView.ViewMode.ArtistName:
					// hide unused blocks
					MainTimeBlock.Visibility = Visibility.Collapsed;
					SubTimeBlock.Visibility = Visibility.Collapsed;
					OneLineBlock.Visibility = Visibility.Collapsed;

					// show used blocks
					TwoLineBlock.Visibility = Visibility.Visible;
					IconsBlock.Visibility = Visibility.Visible;
					break;
				case SMListView.ViewMode.ArtistTrack:
					// hide unused blocks
					MainTimeBlock.Visibility = Visibility.Collapsed;
					OneLineBlock.Visibility = Visibility.Collapsed;

					// show used blocks
					TwoLineBlock.Visibility = Visibility.Visible;
					SubTimeBlock.Visibility = Visibility.Visible;
					IconsBlock.Visibility = Visibility.Visible;
					break;

				case SMListView.ViewMode.OrderTrack:
				case SMListView.ViewMode.OrderArtist:
					// hide unused blocks
					OneLineBlock.Visibility = Visibility.Collapsed;

					// show used blocks
					MainTimeBlock.Visibility = Visibility.Visible;
					SubTimeBlock.Visibility = Visibility.Visible;
					TwoLineBlock.Visibility = Visibility.Visible;
					IconsBlock.Visibility = Visibility.Visible;
					break;
			}
		}

		// Text blocks
		private string mainTimeText;
		public string MainTimeText
		{
			set
			{
				this.mainTimeText = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MainTimeText"));
			}
			get
			{
				return this.mainTimeText;
			}
		}

		private string oneLineText;
		public string OneLineText
		{
			set
			{
				this.oneLineText = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OneLineText"));
			}
			get
			{
				return this.oneLineText;
			}
		}

		private string twoLineTopText;
		public string TwoLineTopText
		{
			set
			{
				this.twoLineTopText = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TwoLineTopText"));
			}
			get
			{
				return this.twoLineTopText;
			}
		}

		private string twoLineBotText;
		public string TwoLineBotText
		{
			set
			{
				this.twoLineBotText = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TwoLineBotText"));
			}
			get
			{
				return this.twoLineBotText;
			}
		}

		private string subTimeText;
		public string SubTimeText
		{
			set
			{
				this.subTimeText = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SubTimeText"));
			}
			get
			{
				return this.subTimeText;
			}
		}

		// Images
		private string mainImgPath;
		public string MainImagePath
		{
			set
			{
				this.mainImgPath = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MainImagePath"));
			}
			get
			{
				return this.mainImgPath;
			}
		}

		private string iconsPath0;
		private string iconsPath1;
		private string iconsPath2;
		private string iconsPath3;

		public string IconsPath0
		{
			set
			{
				this.iconsPath0 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IconsPath0"));
			}
			get
			{
				return this.iconsPath0;
			}
		}
		public string IconsPath1
		{
			set
			{
				this.iconsPath1 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IconsPath1"));
			}
			get
			{
				return this.iconsPath1;
			}
		}

		public string IconsPath2
		{
			set
			{
				this.iconsPath2 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IconsPath2"));
			}
			get
			{
				return this.iconsPath2;
			}
		}

		public string IconsPath3
		{
			set
			{
				this.iconsPath3 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IconsPath3"));
			}
			get
			{
				return this.iconsPath3;
			}
		}
	}
}
