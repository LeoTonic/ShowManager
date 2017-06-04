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
using System.Text.RegularExpressions;

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
				catch (Exception)
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
			var cIndex = tb.CaretIndex;

			var onlyNum = Regex.Replace(text, @"[^\d]", "");
			if (onlyNum.Length > 6)
			{
				if (cIndex > 8)
				{
					onlyNum = onlyNum.Substring(onlyNum.Length - 6, 6);
				}
				else
				{
					onlyNum = onlyNum.Substring(0, 6);
				}
			}
			else if (onlyNum.Length < 6)
			{
				onlyNum = string.Concat(onlyNum, new string('0', 6 - onlyNum.Length));
			}
			// set dividers
			onlyNum = string.Concat(onlyNum.Substring(0, 2), ':', onlyNum.Substring(2, 2), ':', onlyNum.Substring(4, 2));
			tb.Text = onlyNum;
			if (lastText.Length > text.Length)
			{
				// deleting
				if (cIndex == 3 || cIndex == 6)
					cIndex--;
			}
			else
			{
				// adding
				if (cIndex == 2 || cIndex == 5)
					cIndex++;
			}
			tb.CaretIndex = cIndex;
			lastText = text;
		}
	}
}
