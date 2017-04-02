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
using System.Runtime.InteropServices;

namespace ShowManager.Controls
{
	/// <summary>
	/// Логика взаимодействия для DragDropWindow.xaml
	/// </summary>
	public partial class DragDropWindow : Window
	{
		public DragDropWindow()
		{
			InitializeComponent();
			BitmapImage logo = new BitmapImage();
			logo.BeginInit();
			logo.UriSource = new Uri("pack://application:,,,/ShowManager;component/Images/Tools/drag.png");
			logo.EndInit();

			Rectangle r = new Rectangle();
			r.Width = logo.Width;
			r.Height = logo.Height;
			r.Fill = new ImageBrush(logo);
			this.Content = r;
		}

		// Update window position by cursor
		public void UpdatePosition()
		{
			// update the position of the visual feedback item
			Win32Point w32Mouse = new Win32Point();
			GetCursorPos(ref w32Mouse);
			this.Left = w32Mouse.X + 8;
			this.Top = w32Mouse.Y + 8;
		}

		public void ShowWindow()
		{
			UpdatePosition();
			this.Show();
		}
		public void HideWindow()
		{
			this.Hide();
		}

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetCursorPos(ref Win32Point pt);

		[StructLayout(LayoutKind.Sequential)]
		internal struct Win32Point
		{
			public Int32 X;
			public Int32 Y;
		};
	}
}
