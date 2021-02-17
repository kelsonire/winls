using System;
using System.IO;
using System.Linq;

namespace winls
{
	class Entry
	{
		public enum Type
		{
			Directory,
			ExecutableFile,
			ShortcutFile,
			Other
		}

		public readonly string path;
		public readonly string name;
		// nameLength には全角を考慮した値がはいるので、name.Length は使わない。
		public readonly int nameLength;
		public readonly Type type;
		public readonly int sortOrder = 0;
		public int cellSize;

		private int spaceCount = 0;

		public Entry(string path)
		{
			this.path = path;

			if (IsDirectory(path))
			{
				this.type = Type.Directory;
				this.name = "./" + Path.GetFileName(path);
				this.sortOrder = 1;
			}
			else
			{
				var ext = Path.GetExtension(path).ToLower();

				switch (ext)
				{
					case ".exe":
						this.type = Type.ExecutableFile;
						break;
					case ".lnk":
						this.type = Type.ShortcutFile;
						break;
					default:
						this.type = Type.Other;
						break;
				}

				this.name = Path.GetFileName(path);
				this.sortOrder = 2;
			}

			var fullWidthCount = name.Count(c => c.IsFullWidth());
			this.nameLength = fullWidthCount * 2 + (this.name.Length - fullWidthCount);
			this.cellSize = nameLength;
		}

		public void SetCellSize(int cellSize)
		{
			this.spaceCount = Math.Max(cellSize - nameLength, 0);
			this.cellSize = cellSize;
		}

		public void ResetSpaceCount()
		{
			this.spaceCount = 0;
			this.cellSize = 0;
		}

		// spaceCount は何度か変更されるので、書き出すタイミングにだけ string build する
		public string GetSpacedName()
		{
			return name + new string(' ', spaceCount);
		}

		private bool IsDirectory(string path)
		{
			return Directory.Exists(path);
		}
	}
}
