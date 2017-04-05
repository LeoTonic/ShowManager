using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManager.Models
{
	public class SMElement
	{
		// Режимы отображения
		public enum Modes
		{
			Performer = 0,
			Tracks
		}

		public Modes workMode; // Текущий режим работы

		public List<SMElement> Items;

		public SMElement()
		{
			workMode = Modes.Performer;
			Items = new List<SMElement>();
		}
	}
}
