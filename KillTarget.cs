﻿using CommandSystem;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SwiftKraft
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class KillTarget : ICommand
    {
		public string Command { get; } = "killtarget";

		public string[] Aliases { get; } = new string[] { "ktarget", "target", "kt" };

		public string Description { get; } = "Set kill target. Usage: \"kt <Player Name/Player ID>\"";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (arguments.Array.Length < 2)
			{
				response = "Need to provide player name or player id! Usage: \"kt <Player Name/Player ID>\"";
				return false;
			}

			if (Player.TryGetByName(arguments.Array[1], out Player p))
            {
				response = "Kill target set to " + p.Nickname + " (" + p.PlayerId + ")";
				Plugin.killTarget = p.PlayerId;

				return true;
			}
			else if (int.TryParse(arguments.Array[1], out int i) && Player.TryGet(i, out Player _p))
            {
				response = "Kill target set to " + _p.Nickname + " (" + _p.PlayerId + ")";
				Plugin.killTarget = _p.PlayerId;

				return true;
			}

			response = "No player was found with name/id: " + arguments.Array[1];

			return false;
		}
	}
}