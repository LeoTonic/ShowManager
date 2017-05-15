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
using ShowManager.Models;

namespace ShowManager.Controls
{
	/// <summary>
	/// Логика взаимодействия для TrackDialog.xaml
	/// </summary>
	public partial class TrackDialog : Window
	{
        private SMTrack track;

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

            TrackMinusID.Text = track.MinusID;
            TrackMinusArtist.Text = track.MinusArtistName;
            TrackMinusTrack.Text = track.MinusTrackName;
		}

		private void SaveData_Click(object sender, RoutedEventArgs e)
		{
            if (string.IsNullOrWhiteSpace(TrackName.Text))
            {
                MessageBox.Show("Не задано имя произведения!", "Пустое поле", MessageBoxButton.OK, MessageBoxImage.Information);
                TrackName.Focus();
                return;
            }

            if (!string.IsNullOrWhiteSpace(TrackMinusID.Text)) {
                if (string.IsNullOrWhiteSpace(TrackMinusArtist.Text)) {
                    MessageBox.Show("Не задано имя исполнителя минусовки", "Пустое поле", MessageBoxButton.OK, MessageBoxImage.Information);
                    TrackMinusArtist.Focus();
                    return;
                }
                else if (string.IsNullOrWhiteSpace(TrackMinusTrack.Text)) {
                    MessageBox.Show("Не задано имя минусовки", "Пустое поле", MessageBoxButton.OK, MessageBoxImage.Information);
                    TrackMinusTrack.Focus();
                    return;
                }
            }

            track.Name = TrackName.Text;
            track.TrackLength = TrackLength.Value;
            
            // Сюда написать обработку вставки минусовки в коллекцию

            DialogResult = true;
		}
	}
}
