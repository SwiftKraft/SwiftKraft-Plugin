using CommandSystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using PlayerRoles;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace SwiftKraft
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class CustomItem : ICommand
    {
        public static Dictionary<string, ushort> itemNameToItemId = new Dictionary<string, ushort>();

        public string Command { get; } = "customitem";

        public string[] Aliases { get; } = new string[] { "custitem", "citem", "cust" };

        public string Description { get; } = "Gives custom item, not providing a player will give it to the command executor. Usage: \"cust <Custom Item Name> [Player Name/Player ID]\". Can use @ALL, @HUMAN, @SCP, @MTF, @CI in replacement for player";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.GivingItems))
            {
                response = "You Do Not Have Permission To Do That! Required Perm: Giving Items";

                return false;
            }

            if (arguments.Array.Length < 2)
            {
                response = "Must provide a custom item name! Usage: \"cust <Custom Item Name> [Player]\"";
                return false;
            }

            if (arguments.Array.Length < 3)
            {
                if (itemNameToItemId.ContainsKey(arguments.Array[1].ToUpper()))
                {
                    Player _p = Player.Get(sender);

                    AddCustomItem(_p, arguments.Array[1].ToUpper());

                    response = "Giving item " + arguments.Array[1].ToUpper() + " to " + _p.Nickname + " (" + _p.PlayerId + ")";

                    return true;
                }
                else
                {
                    response = "No custom item has been found with name " + arguments.Array[1].ToUpper();

                    return false;
                }
            }

            if (arguments.Array[2].ToCharArray()[0] == '@')
            {
                switch (arguments.Array[2].ToUpper())
                {
                    case "@ALL":
                        foreach (Player _p in Player.GetPlayers())
                        {
                            if (_p.IsAlive)
                                AddCustomItem(_p, arguments.Array[1].ToUpper());
                        }
                        response = "Giving " + arguments.Array[1].ToUpper() + " to all Alive Players. ";
                        return false;
                    case "@HUMAN":
                        foreach (Player _p in Player.GetPlayers())
                        {
                            if (_p.IsAlive && !_p.IsSCP)
                                AddCustomItem(_p, arguments.Array[1].ToUpper());
                        }
                        response = "Giving " + arguments.Array[1].ToUpper() + " to all Humans. ";
                        return false;
                    case "@SCP":
                        foreach (Player _p in Player.GetPlayers())
                        {
                            if (_p.IsAlive && _p.Role.GetFaction() == Faction.SCP)
                                AddCustomItem(_p, arguments.Array[1].ToUpper());
                        }
                        response = "Giving " + arguments.Array[1].ToUpper() + " to all SCPs. ";
                        return false;
                    case "@MTF":
                        foreach (Player _p in Player.GetPlayers())
                        {
                            if (_p.IsAlive && _p.Role.GetFaction() == Faction.FoundationStaff)
                                AddCustomItem(_p, arguments.Array[1].ToUpper());
                        }
                        response = "Giving " + arguments.Array[1].ToUpper() + " to all Foundation Forces. ";
                        return false;
                    case "@CI":
                        foreach (Player _p in Player.GetPlayers())
                        {
                            if (_p.IsAlive && _p.Role.GetFaction() == Faction.FoundationEnemy)
                                AddCustomItem(_p, arguments.Array[1].ToUpper());
                        }
                        response = "Giving " + arguments.Array[1].ToUpper() + " to all Chaos Forces. ";
                        return false;
                }
            }

            if (Player.TryGetByName(arguments.Array[2], out Player p))
            {
                if (itemNameToItemId.ContainsKey(arguments.Array[1].ToUpper()))
                {
                    AddCustomItem(p, arguments.Array[1].ToUpper());

                    response = "Giving item " + arguments.Array[1].ToUpper() + " to " + p.Nickname + " (" + p.PlayerId + ")";

                    return true;
                }
                else
                {
                    response = "No custom item has been found with name " + arguments.Array[1].ToUpper();

                    return false;
                }
            }
            else if (int.TryParse(arguments.Array[2], out int i) && Player.TryGet(i, out Player _p))
            {
                if (itemNameToItemId.ContainsKey(arguments.Array[1].ToUpper()))
                {
                    AddCustomItem(_p, arguments.Array[1].ToUpper());

                    response = "Giving item " + arguments.Array[1].ToUpper() + " to " + _p.Nickname + " (" + _p.PlayerId + ")";

                    return true;
                }
                else
                {
                    response = "No custom item has been found with name " + arguments.Array[1].ToUpper();

                    return false;
                }
            }

            response = "No player found with name/id: " + arguments.Array[2];

            return false;
        }

        public static void AddCustomItem(Player p, string itemName)
        {
            if (string.IsNullOrEmpty(itemName))
                return;

            if (itemName.ToCharArray()[0] == '_' && int.TryParse(itemName.Remove(0, 1), out int i))
            {
                p.AddItem((ItemType)i);

                return;
            }

            ItemBase b = p.AddItem((ItemType)itemNameToItemId[itemName]);

            Plugin.customItems.Add(b.ItemSerial, itemName);

            if (b.Category == ItemCategory.Firearm)
            {
                Firearm f = (Firearm)b;
                f.Status = new FirearmStatus(f.AmmoManagerModule.MaxAmmo, FirearmStatusFlags.MagazineInserted, 0);
            }
        }

        public static string RandomCustomItemWithType(ushort id)
        {
            if (!itemNameToItemId.ContainsValue(id))
                return "";

            List<string> temp = new List<string>();

            foreach (string key in itemNameToItemId.Keys)
            {
                if (itemNameToItemId[key] == id && key.ToCharArray()[0] != '_')
                    temp.Add(key);
            }

            if (temp.Count > 0)
                return temp[Random.Range(0, temp.Count)];
            else
                return "";
        }

        public static string RandomCustomItemWithType(ItemType id)
        {
            if (!itemNameToItemId.ContainsValue((ushort)id))
                return "";

            List<string> temp = new List<string>();

            foreach (string key in itemNameToItemId.Keys)
            {
                if (itemNameToItemId[key] == (ushort)id && key.ToCharArray()[0] != '_')
                    temp.Add(key);
            }

            if (temp.Count > 0)
                return temp[Random.Range(0, temp.Count)];
            else
                return "";
        }
    }
}
