using CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftKraft
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class Conversion : ICommand
    {
		public string Command { get; } = "conversion";

		public string[] Aliases { get; } = new string[] { "conv", "cnvs", "cv" };

		public string Description { get; } = "Toggles custom item conversion, 1 is true, 0 is false. Usage: \"cv <1/0>\"";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (arguments.Array.Length < 2)
			{
				response = "Custom Item Conversion: " + CustomItemConversion.IsOn;
				return true;
			}

			if (int.TryParse(arguments.Array[1], out int i))
            {
				if (i > 1 || i < 0)
                {
					response = "Must provide 1 (True) or 0 (False)! Usage: \"cv <1/0>\"";
					return false;
				}

				CustomItemConversion.IsOn = Convert.ToBoolean(i);

				response = "Custom Item Conversion: " + CustomItemConversion.IsOn;

				return true;
			}

			response = "Invalid input! Custom Item Conversion: " + CustomItemConversion.IsOn;

			return false;
		}
	}
}
