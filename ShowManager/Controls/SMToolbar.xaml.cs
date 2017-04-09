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

namespace ShowManager.Controls
{
	/// <summary>
	/// Логика взаимодействия для SMToolbar.xaml
	/// </summary>
	public partial class SMToolbar : UserControl
	{
		private ICommandCatcher iCommandTo = null;

		private string imageAdd;
		public string ImageAdd {
			get
			{
				return imageAdd;
			}
			set {
				imageAdd = SetPath(value);
			}
		}
		private string ttAdd;
		public string TooltipAdd
		{
			get
			{
				return ttAdd;
			}
			set
			{
				ttAdd = value;
			}
		}

		private string imageEdit;
		public string ImageEdit
		{
			get
			{
				return imageEdit;
			}
			set
			{
				imageEdit = SetPath(value);
			}
		}

		private string ttEdit;
		public string TooltipEdit
		{
			get
			{
				return ttEdit;
			}
			set
			{
				ttEdit = value;
			}
		}

		private string imageRemove;
		public string ImageRemove
		{
			get
			{
				return imageRemove;
			}
			set
			{
				imageRemove = SetPath(value);
			}
		}

		private string ttRemove;
		public string TooltipRemove
		{
			get
			{
				return ttRemove;
			}
			set
			{
				ttRemove = value;
			}
		}

		public SMToolbar()
		{
			InitializeComponent();
			DataContext = this;
		}

		private string SetPath(string image)
		{
			return "/ShowManager;component/Images/Tools/" + image + ".png";
		}

		public void SetValues(string imgAdd, string tooltipAdd, string imgEdit, string tooltipEdit, string imgRemove, string tooltipRemove, ICommandCatcher catchTo)
		{
			if (!string.IsNullOrWhiteSpace(imgAdd))
			{
				ImageAdd = imgAdd;
				TooltipAdd = tooltipAdd;
				btnAdd.Visibility = Visibility.Visible;
			}
			else
			{
				btnAdd.Visibility = Visibility.Collapsed;
			}

			if (!string.IsNullOrWhiteSpace(imgEdit))
			{
				ImageEdit = imgEdit;
				TooltipEdit = tooltipEdit;
				btnEdit.Visibility = Visibility.Visible;
			}
			else
			{
				btnEdit.Visibility = Visibility.Collapsed;
			}

			if (!string.IsNullOrWhiteSpace(imgRemove))
			{
				ImageRemove = imgRemove;
				TooltipRemove = tooltipRemove;
				btnRemove.Visibility = Visibility.Visible;
			}
			else
			{
				btnRemove.Visibility = Visibility.Collapsed;
			}
			iCommandTo = catchTo;
		}

		private void ButtonAdd_Click(object sender, RoutedEventArgs e)
		{
			if (iCommandTo != null)
			{
				iCommandTo.ToolBarAdd(this);
			}
		}

		private void ButtonEdit_Click(object sender, RoutedEventArgs e)
		{
			if (iCommandTo != null)
			{
				iCommandTo.ToolBarEdit(this);
			}
		}

		private void ButtonRemove_Click(object sender, RoutedEventArgs e)
		{
			if (iCommandTo != null)
			{
				iCommandTo.ToolBarRemove(this);
			}
		}
	}
}
