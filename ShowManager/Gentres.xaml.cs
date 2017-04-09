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
using ShowManager.Controls;

namespace ShowManager
{
	/// <summary>
	/// Логика взаимодействия для Gentres.xaml
	/// </summary>
	public partial class Gentres : Window, ICommandCatcher
	{
		public Gentres(DragDropWindow wndDD)
		{
			InitializeComponent();

			// Инициализация элементов управления
			lvwGentreGroups.SetDragDropWindow(wndDD);
			lvwGentreGroups.SetViewMode(Controls.SMListView.ViewMode.Gentre);
			gentresToolbar.SetValues("prop-add", "Добавить жанр", "prop-edit", "Изменить жанр", "prop-delete", "Удалить жанр", this);
		}

		// Обработка панели инструментов
		public void ToolBarAdd(SMToolbar tb)
		{
			MessageBox.Show("Add");
		}

		public void ToolBarEdit(SMToolbar tb)
		{

		}

		public void ToolBarRemove(SMToolbar tb)
		{

		}
	}
}
