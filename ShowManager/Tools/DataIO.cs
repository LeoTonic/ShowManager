using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace ShowManager.Tools
{
	public class DataIO
	{
		// Класс для файловых операций

		// Объекты потоков
		StreamReader sRead = null;
		StreamWriter sWrite = null;

		// Формат времени
		CultureInfo ci = new CultureInfo("en-US");

		// Маркеры объектов файла
		public const string IN_ELEMENT			= ">>SMITEM";
		public const string OUT_ELEMENT			= "<<SMITEM";
		public const string IN_TRACK				= ">>SMTRACK";
		public const string OUT_TRACK				= "<<SMTRACK";
		public const string IN_ARTIST				= ">>SMARTIST";
		public const string OUT_ARTIST			= "<<SMARTIST";
		public const string IN_GROUP				= ">>SMGROUP";
		public const string OUT_GROUP				= "<<SMGROUP";
		public const string IN_GENTRE				= ">>SMGENTREITEM";
		public const string OUT_GENTRE			= "<<SMGENTREITEM";
		public const string IN_GENTREBASE		= ">>SMGENTREBASE";
		public const string OUT_GENTREBASE	= "<<SMGENTREBASE";
		public const string IN_PROJECT			= ">>SMPROJECT";
		public const string OUT_PROJECT			= "<<SMPROJECT";

		public const string IN_ARRAY				= ">>ARRAY";
		public const string OUT_ARRAY				= "<<ARRAY";

		public bool OpenRead(string path)
		{
			// Открытие потока на чтение

			if (sRead != null)
				CloseRead();

			try
			{
				sRead = new StreamReader(path, Encoding.UTF8);
			}
			catch (IOException ioex)
			{
				Console.WriteLine(string.Concat("Read Error: ", path));
				Console.WriteLine(ioex.Message);
				return false;
			}
			return true;
		}
		public void CloseRead()
		{
			// Закрытие потока на чтение

			if (sRead != null)
			{
				try
				{
					sRead.Close();
				}
				catch (IOException ioex)
				{
					Console.WriteLine(ioex.Message);
				}
				sRead = null;
			}
		}
		public bool OpenWrite(string path)
		{
			// Открытие потока на запись

			if (sWrite != null)
				CloseWrite();

			try
			{
				sWrite = new StreamWriter(path, false, Encoding.UTF8);
			}
			catch (IOException ioex)
			{
				Console.WriteLine(string.Concat("Write Error: ", path));
				Console.WriteLine(ioex.Message);
			}
			return true;
		}
		public void CloseWrite()
		{
			// Закрытие потока на запись

			if (sWrite != null)
			{
				try
				{
					sWrite.Close();
				}
				catch (IOException ioex)
				{
					Console.WriteLine(ioex.Message);
				}
				sWrite = null;
			}
		}
		public string ReadString()
		{
			// Чтение строки
			string s;
			try
			{
				s = sRead.ReadLine();
				if (s == null)
					s = "";
			}
			catch
			{
				s = "";
			}
			return s;
		}
		public long ReadLong()
		{
			// Чтение строки и преобразование в целое
			string s = ReadString();
			long l;
			try
			{
				l = long.Parse(s);
			}
			catch
			{
				l = 0;
			}
			return l;
		}
		public int ReadInt()
		{
			// Чтение строки и преобразование в короткое целое
			string s = ReadString();
			int n;
			try
			{
				n = int.Parse(s);
			}
			catch
			{
				n = 0;
			}
			return n;
		}
		public TimeSpan ReadTime()
		{
			// Чтение строки и преобразование в представление времени
			string s = ReadString();
			TimeSpan ts;
			try
			{
				ts = TimeSpan.Parse(s, ci);
			}
			catch
			{
				ts = TimeSpan.Zero;
			}
			return ts;
		}
		public bool ReadBool()
		{
			// Чтение двоичного значения
			string s = ReadString();
			return (s == "1");
		}
		public void WriteString(string s)
		{
			// Запись строки
			if (sWrite == null)
				return;

			try
			{
				sWrite.WriteLine(s);
			}
			catch (IOException ioex)
			{
				Console.WriteLine(ioex.Message);
			}
		}
		public void WriteLong(long l)
		{
			// Запись целого
			if (sWrite == null)
				return;
			try
			{
				sWrite.WriteLine(l);
			}
			catch (IOException ioex)
			{
				Console.WriteLine(ioex.Message);
			}
		}
		public void WriteInt(int n)
		{
			// Запись целого короткого
			if (sWrite == null)
				return;
			try
			{
				sWrite.WriteLine(n);
			}
			catch (IOException ioex)
			{
				Console.WriteLine(ioex.Message);
			}
		}
		public void WriteTime(TimeSpan ts)
		{
			// Запись времени
			string s = ts.ToString(@"hh\:mm\:ss", ci);
			WriteString(s);
		}
		public void WriteBool(bool b)
		{
			// Запись двоичного
			if (sWrite == null)
				return;
			try
			{
				string s;
				if (b)
					s = "1";
				else
					s = "0";
				sWrite.WriteLine(s);
			}
			catch (IOException ioex)
			{
				Console.WriteLine(ioex.Message);
			}
		}
		public int SeekTo(string mItem, string mParent = "")
		{
			// Следование до маркера
			// mItem - маркер элемента
			// mParent - маркер родителя
			// Возвращает 1 если найден маркер элемента
			// Возвращает 0 если найден маркер родителя
			// Возвращает -1 если достигнут конец файла
			
			if (sRead == null)
				return -1;

			string sr;
			while((sr = sRead.ReadLine()) != null)
			{
				if (sr == mItem)
					return 1;
				else if (mParent != "" && sr == mParent)
					return 0;
			}
			return -1;
		}
	}
}
