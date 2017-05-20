using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManager.Controls
{
	// Ловец команд элементов управления
	public interface ICommandCatcher
	{
		// Выборка элементов
		void ItemSelectionChange(Object parentControl); // Выбор элемента в контроле

		// Панель инструментов
		void ToolBarAdd(SMToolbar tb); // Новый элемент
		void ToolBarEdit(SMToolbar tb); // Редактируем элемент
		void ToolBarRemove(SMToolbar tb); // Удаляем элемент

		// Панель групп
		void PanelGroupClick(string groupName); // Клик на группе в панели
		void PanelGroupAdd(string groupName);	// Создание новой вкладки
		void PanelGroupRename(string groupNameOld, string groupNameNew); // Переименование панели
		void PanelGroupDelete(string groupName); // Удаление панели
	
		// Перенос элементов
		void DropItems(int insertIndex, SMListViewItem draggedItem, Object draggedTo);
	}
}
