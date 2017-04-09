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

		private string imgPath;
		public string ImagePath
		{
			get
			{
				return imgPath;
			}
			set
			{
				imgPath = value;
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
				ElementName = elemName;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ElementName"));

			}
		}
		public ElementDialog()
		{
			InitializeComponent();
			DataContext = this;
		}
	}
}
