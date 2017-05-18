using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ShowManager.Models;

namespace ShowManager.Controls
{
	/// <summary>
	/// Логика взаимодействия для TrackDialog.xaml
	/// </summary>
	public partial class TrackDialog : Window
	{
		private SMTrack track;

		private bool needRemoveMinus;

		public SMTrack Track
		{
			get
			{
				return this.track;
			}
		}
		private string minusFilePath = "";

		public TrackDialog(SMArtist artist, SMTrack fromTrack)
		{
			InitializeComponent();

			track = new SMTrack(artist);
			if (fromTrack != null)
			{
				track.Assign(fromTrack);
			}

			TrackName.Text = track.Name;
			TrackLength.Value = track.TrackLength;
			if (track.MinusID != 0)
			{
				TrackMinusID.Text = "Фонограмма имеется";
				RemoveMinus.IsEnabled = true;
			}
			else
			{
				TrackMinusID.Text = "";
				RemoveMinus.IsEnabled = false;
			}
			TrackMinusArtist.Text = track.MinusArtistName;
			TrackMinusTrack.Text = track.MinusTrackName;

			needRemoveMinus = false;
		}

		private void SaveData_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(TrackName.Text))
			{
				MessageBox.Show("Не задано имя произведения!", "Пустое поле", MessageBoxButton.OK, MessageBoxImage.Information);
				TrackName.Focus();
				return;
			}

			if (!string.IsNullOrWhiteSpace(minusFilePath) || track.MinusID != 0)
			{
				if (string.IsNullOrWhiteSpace(TrackMinusArtist.Text))
				{
					MessageBox.Show("Не задано имя исполнителя минусовки", "Пустое поле", MessageBoxButton.OK, MessageBoxImage.Information);
					TrackMinusArtist.Focus();
					return;
				}
				else if (string.IsNullOrWhiteSpace(TrackMinusTrack.Text))
				{
					MessageBox.Show("Не задано имя минусовки", "Пустое поле", MessageBoxButton.OK, MessageBoxImage.Information);
					TrackMinusTrack.Focus();
					return;
				}
			}

			track.Name = TrackName.Text;
			track.TrackLength = TrackLength.Value;

			// Убираем минусовку если пользователь удалил ее
			if (track.MinusID != 0 && needRemoveMinus)
			{
				track.MinusID = 0;
				track.MinusArtistName = "";
				track.MinusTrackName = "";
				track.MinusExtention = "";
			}
			else
			{
				track.MinusArtistName = TrackMinusArtist.Text;
				track.MinusTrackName = TrackMinusTrack.Text;
			}

			// Пишем минусовку в коллекцию
			if (Directory.Exists(track.ParentArtist.ParentProject.TrackFolderPath) && !string.IsNullOrWhiteSpace(minusFilePath))
			{
				var newID = new SMElement();
				track.MinusID = newID.ID;
				track.MinusExtention = GetFileExtention(minusFilePath);
				try
				{
					File.Copy(minusFilePath, string.Concat(track.ParentArtist.ParentProject.TrackFolderPath, System.IO.Path.DirectorySeparatorChar , track.MinusID.ToString()));
				}
				catch (Exception ex)
				{
					System.Console.WriteLine(ex.Message);
				}
			}

			DialogResult = true;
		}

		private void AddMinus_Click(object sender, RoutedEventArgs e)
		{
			var ofd = new OpenFileDialog()
			{
				Filter = "Все файлы (*.*)|*.*",
				Title = "Выберите файл фонограммы"
			};
			if (ofd.ShowDialog() == true)
			{
				minusFilePath = ofd.FileName;
				TrackMinusID.Text = "Фонограмма добавлена";
				RemoveMinus.IsEnabled = true;
				needRemoveMinus = false;
			}
		}

		private void RemoveMinus_Click(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Удаляем фонограмму?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				minusFilePath = "";
				TrackMinusID.Text = "";
				RemoveMinus.IsEnabled = false;
				needRemoveMinus = true;
			}
		}

		// Получение расширения файла
		private string GetFileExtention(string from)
		{
			string sExt = "";
			for (int n = from.Length-1; n >= 0; n--)
			{
				if (from[n] == '.')
				{
					break;
				}
				sExt = string.Concat(from[n], sExt);
			}
			return sExt;
		}
	}
}
