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
using System.Globalization;
using System.ComponentModel;

namespace ShowManager.Controls
{
	/// <summary>
	/// Логика взаимодействия для TimePicker.xaml
	/// </summary>
	public partial class TimePicker : UserControl, INotifyPropertyChanged
	{
		private string lastText = "";
		public event PropertyChangedEventHandler PropertyChanged;

		private CultureInfo ci = new CultureInfo("en-US");

		public TimePicker()
		{
			InitializeComponent();
			DataContext = this;
		}

		public TimeSpan Value;

		public string StringTime
		{
			get
			{
				return Value.ToString(@"hh\:mm\:ss", ci);
			}
			set
			{
				try
				{
					Value = TimeSpan.Parse(value);
				}
				catch (FormatException)
				{
					Value = TimeSpan.Zero;
				}
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StringTime"));
			}
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			var tb = sender as TextBox;
			var text = tb.Text;

			if (text.Length > lastText.Length)
			{
				if (text.Length == 2 || text.Length==5)
				{
					text = string.Concat(text, ':');
					tb.Text = text;
					tb.CaretIndex = text.Length;
				}
			}
			lastText = text;
		}
	}
}
