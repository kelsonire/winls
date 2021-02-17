using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace winls
{
	class Program
	{
		static void Main(string[] args)
		{
			new Program().Run(args);
		}

		void Run(string[] args)
		{
			var options = Option.Parse(args);
			var path = Option.GetTargetPath(options);

			try
			{
				// entry は相対パス、もしくは絶対パス
				var entries =
					Directory.EnumerateFileSystemEntries(path)
					.Select(entry => new Entry(entry))
					.OrderBy(entry => entry.sortOrder + entry.name)
					.ToArray();
				Print(entries, Console.WindowWidth);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Environment.Exit(1);
			}
		}

		void Print(Entry[] entries, int width)
		{
			foreach (var group in Grouping(entries, width))
			{
				foreach (var entry in group)
				{
					PrintEntryWithColor(entry);
				}
				Console.WriteLine("");
			}
		}

		IEnumerable<Entry[]> Grouping(Entry[] entries, int consoleWidth)
		{
			for (var numOfWordsInALine = entries.Length; numOfWordsInALine > 1; numOfWordsInALine--)
			{
				var grouped = DoGrouping(entries, numOfWordsInALine);
				SetSpaces(grouped, numOfWordsInALine);

				if (IsDisplayable(grouped, consoleWidth))
				{
					return grouped;
				}
			}

			return entries.Select(entry => new Entry[] { entry });
		}

		Entry[][] DoGrouping(IEnumerable<Entry> entries, int numOfWordsInALine)
		{
			return entries
				.Select((entry, index) => new { Entry = entry, Group = index / numOfWordsInALine })
				.GroupBy(obj => obj.Group)
				.Select(group => group.Select(obj => obj.Entry).ToArray())
				.ToArray();
		}

		bool IsDisplayable(IEnumerable<IEnumerable<Entry>> grouped, int consoleWidth)
		{
			return grouped.All(group => group.Sum(entry => entry.cellSize) <= consoleWidth);
		}

		void SetSpaces(Entry[][] grouped, int numOfWordsInALine)
		{
			var maxWordLengthEachColumn = CalcMaxWordLengthEachColumn(grouped, numOfWordsInALine);

			foreach (var group in grouped)
			{
				for (var i = 0; i < group.Length; i++)
				{
					group[i].SetCellSize(maxWordLengthEachColumn[i]);
				}
			}
		}

		int[] CalcMaxWordLengthEachColumn(Entry[][] grouped, int numOfWordsInALine)
		{
			var maxWordLengthEachColumn =
				Enumerable.Range(0, numOfWordsInALine)
					.Select(x =>
						Enumerable.Range(0, grouped.Length).Max(y =>
							// TODO: if の回数が少しもったいないので最適化する
							x < grouped[y].Length ? grouped[y][x].nameLength : 0
						)
					)
					.ToArray();

			// 各行の最後のエントリ以外はスペースをあけるようにする
			for (var i = 0; i < maxWordLengthEachColumn.Length - 1; i++)
			{
				maxWordLengthEachColumn[i] += 2;
			}

			return maxWordLengthEachColumn;
		}

		void PrintEntryWithColor(Entry entry)
		{
			var name = entry.GetSpacedName();
			switch (entry.type)
			{
				case Entry.Type.Directory:
					WriteWithColor(name, ConsoleColor.Blue);
					return;
				case Entry.Type.ExecutableFile:
					WriteWithColor(name, ConsoleColor.Green);
					break;
				case Entry.Type.ShortcutFile:
					WriteWithColor(name, ConsoleColor.Magenta);
					return;
				default:
					Console.Write(name);
					return;
			}

		}

		void WriteWithColor(string str, ConsoleColor color)
		{
			Console.ForegroundColor = color;
			Console.Write(str);
			Console.ResetColor();
		}
	}
}
