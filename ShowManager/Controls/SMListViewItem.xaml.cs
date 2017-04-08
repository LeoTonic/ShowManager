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
	public enum GentreIcon
	{
		Vocal = 0,
		Dance
	}

	/// <summary>
	/// Логика взаимодействия для SMListViewItem.xaml
	/// </summary>
	public partial class SMListViewItem : UserControl, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public SMListViewItem()
		{
			InitializeComponent();
			DataContext = this;
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

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Text"));
			}
		}
		private double age;
		public double Age
		{
			get
			{
				return this.age;
			}
			set
			{
				this.age = value;

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Age"));
			}
		}

		private GentreIcon gentreIcon;

		public GentreIcon Icon
		{
			get
			{
				return this.gentreIcon;
			}
			set
			{
				this.gentreIcon = value;
				string path = "vocal.png";
				switch (value)
				{
					case GentreIcon.Vocal:
						path = "vocal.png";
						break;
					case GentreIcon.Dance:
						path = "dance.png";
						break;
				}
				this.imgPath = "/ShowManager;component/Images/Gentres/" + path;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImagePath"));
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
	}
}
