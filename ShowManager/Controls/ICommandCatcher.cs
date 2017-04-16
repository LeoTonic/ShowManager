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
		// Панель инструментов
		void ToolBarAdd(SMToolbar tb); // Новый элемент
		void ToolBarEdit(SMToolbar tb); // Редактируем элемент
		void ToolBarRemove(SMToolbar tb); // Удаляем элемент

		// Перенос элементов
		void DropItems(int insertIndex, SMListViewItem draggedItem, Object draggedTo);
	}
}
