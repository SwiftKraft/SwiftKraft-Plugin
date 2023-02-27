using CommandSystem;
using InventorySystem.Items.Firearms;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftKraft
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Attachments : ICommand
    {
		public string Command { get; } = "attachments";

		public string[] Aliases { get; } = new string[] { "att", "atch" };

		public string Description { get; } = "Displays current weapon's attachment number. Usage: \"att\"";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!Player.TryGet(sender, out Player p)) 
			{
				response = "Couldn't find player! ";

				return false;
			}

			if (p.CurrentItem == null || p.CurrentItem.Category != ItemCategory.Firearm)
			{
				response = "Please hold a firearm! ";

				return false;
			}

			Firearm f = (Firearm)p.CurrentItem;

			response = "Attachments of " + f.name + ": " + f.Status.Attachments;

			return true;
		}
	}
}
