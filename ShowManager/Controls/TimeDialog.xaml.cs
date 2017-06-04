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

namespace ShowManager.Controls
{
	/// <summary>
	/// Логика взаимодействия для TimeDialog.xaml
	/// </summary>
	public partial class TimeDialog : Window
	{
		public TimeSpan TimeValue;
		public TimeDialog(TimeSpan fromValue)
		{
			InitializeComponent();
			TimeControl.Value = fromValue;
			TimeControl.TextBoxControl.Focus();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			TimeValue = TimeControl.Value;
			DialogResult = true;
		}

		private void TimeControl_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				OKButton.Focus();
				Button_Click(OKButton, null);
			}
		}
	}
}
