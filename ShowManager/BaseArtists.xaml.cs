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
using ShowManager.Models;

namespace ShowManager
{
  /// <summary>
  /// Логика взаимодействия для BaseArtists.xaml
  /// </summary>
  public partial class BaseArtists : Window, ICommandCatcher
  {
    MainWindow parentWindow = null;
    DragDropWindow wndDragDrop = null;
    SMGentresBase gentres = null;

    public BaseArtists(MainWindow pWnd, DragDropWindow wndDD, SMGentresBase gb)
    {
      parentWindow = pWnd;
      wndDragDrop = wndDD;
      gentres = gb;

      InitializeComponent();

      BaseArtistView.SetDragDropWindow(wndDragDrop);
      BaseArtistView.SetViewMode(SMListView.ViewMode.ArtistName, this);
      //BaseArtistPanel.Initialize(null, this, false, currentProject.GroupsArtist);
      BaseArtistToolBar.SetValues("user-add", "Добавить исполнителя", "user-edit", "Изменить исполнителя", "user-delete", "Удалить исполнителя", this);

      Closed += BaseArtists_Closed;
    }

    private void BaseArtists_Closed(object sender, EventArgs e)
    {
      if (parentWindow != null) { parentWindow.isBaseArtistsActive = false; }
    }

    #region CommandCatch definitions
    public void ItemSelectionChange(Object parentControl) { }
    public void ItemDoubleClick(Object parentControl, SMListViewItem selectedItem) { }
    public void ToolBarAdd(SMToolbar tb) { }
    public void ToolBarEdit(SMToolbar tb) { }
    public void ToolBarRemove(SMToolbar tb) { }

    public void PanelGroupClick(string groupName) { }
    public void PanelGroupAdd(string groupName) { }
    public void PanelGroupRename(string groupNameOld, string groupNameNew) { }
    public void PanelGroupDelete(string groupName) { }
    public void DropItems(int insertIndex, SMListViewItem draggedItem, Object draggedTo, Object draggedToSubItem) { }
    #endregion

    private void Filter_Click(object sender, RoutedEventArgs e) { }
  }
}
