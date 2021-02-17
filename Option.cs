using System.Collections.Generic;
using System.IO;

namespace winls
{
	using Options = Dictionary<string, string>;

	class Option
	{
		public static Options Parse(string[] args)
		{
			var options = new Options();

			foreach (var arg in args)
			{
				// TODO: implement
				options["path"] = arg;
			}

			return options;
		}

		public static string GetTargetPath(Options options)
		{
			if (options.ContainsKey("path"))
			{
				return options["path"];
			}
			else
			{
				return Directory.GetCurrentDirectory();
			}
		}
	}
}
