using CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftKraft
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class ListCustomItem : ICommand
    {
		public string Command { get; } = "listcustomitem";

		public string[] Aliases { get; } = new string[] { "custlist", "clist" };

		public string Description { get; } = "Lists all registered custom items. Usage: \"clist\"";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = "Listing All Custom Items,\nThe ones with an \"_\" at the start are unobtainable naturally through picking up items \n(if you turn on conversion). \n\n";

			foreach (string s in CustomItem.itemNameToItemId.Keys)
            {
				response += " - " + s + "\n";
            }

			return true;
		}
	}
}
