using CommandSystem;
using PlayerRoles;
using PluginAPI.Core;
using System;
using System.Collections.Generic;

namespace SwiftKraft
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Buying : ICommand
    {
        public static Dictionary<string, int> itemCosts = new Dictionary<string, int>(); // CustomItemID | Cost
        public static Dictionary<int, int> playerEco = new Dictionary<int, int>(); // PlayerID | Money
        public static Dictionary<string, string> translatedItemBuys = new Dictionary<string, string>(); // CustomItemID | Alias
        public static Dictionary<string, string> itemBuyTranslation = new Dictionary<string, string>(); // Alias | CustomItemID

        public static bool IsOn;

        public string Command { get; } = "allowbuy";

        public string[] Aliases { get; } = new string[] { "buy", "allowb" };

        public string Description { get; } = "Toggles buy phase, 1 is on, 0 is off. Usage: \"buy <1/0>\"";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.RoundEvents))
            {
                response = "You Do Not Have Permission To Do That! Required Perm: Round Events";

                return false;
            }

            if (arguments.Array.Length < 2)
            {
                response = "Buy Phase: " + IsOn;
                return true;
            }

            if (int.TryParse(arguments.Array[1], out int i))
            {
                if (i != 1 && i != 0)
                {
                    response = "Must provide 1 (True) or 0 (False)! Usage: \"buy <1/0>\"";
                    return false;
                }

                IsOn = Convert.ToBoolean(i);

                response = "Buy Phase: " + IsOn;

                return true;
            }

            response = "Invalid input! Buy Phase: " + IsOn;

            return false;
        }
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Eco : ICommand
    {
        public string Command { get; } = "economy";

        public string[] Aliases { get; } = new string[] { "eco" };

        public string Description { get; } = "Checks economy for a player. Usage: \"eco <Player Name/Player ID>\"";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if(arguments.Array.Length < 2)
            {
                response = "Need to provide player name or player id! Usage: \"eco <Player Name/Player ID>\"";
                return false;
            }

            if (Player.TryGetByName(arguments.Array[1], out Player p))
            {
                if (Buying.playerEco.ContainsKey(p.PlayerId))
                    response = p.Nickname + ": $" + Buying.playerEco[p.PlayerId];
                else
                    response = p.Nickname + ": $0";

                return true;
            }
            else if (int.TryParse(arguments.Array[1], out int i) && Player.TryGet(i, out Player _p))
            {
                if (Buying.playerEco.ContainsKey(_p.PlayerId))
                    response = _p.Nickname + ": $" + Buying.playerEco[_p.PlayerId];
                else
                    response = _p.Nickname + ": $0";

                return true;
            }

            response = "No player was found with name/id: " + arguments.Array[1];

            return false;
        }
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ClearEconomy : ICommand
    {
        public string Command { get; } = "cleareconomy";

        public string[] Aliases { get; } = new string[] { "cleareco", "cleco" };

        public string Description { get; } = "Clears everyone's economy. Usage: \"cleco\"";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.PlayersManagement))
            {
                response = "You Do Not Have Permission To Do That! Required Perm: Players Management";

                return false;
            }

            Buying.playerEco.Clear();

            response = "Cleared economy! ";

            return true;
        }
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class GiveEconomy : ICommand
    {
        public string Command { get; } = "giveeconomy";

        public string[] Aliases { get; } = new string[] { "giveeco", "geco" };

        public string Description { get; } = "Gives you or a player money. Usage: \"geco <Integer> [Player Name/Player ID]\"";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.PlayersManagement))
            {
                response = "You Do Not Have Permission To Do That! Required Perm: Players Management";

                return false;
            }

            if (arguments.Array.Length < 2)
            {
                response = "Please provide the amount to give! ";

                return false;
            }

            if (arguments.Array.Length < 3)
            {
                if (!int.TryParse(arguments.Array[1], out int i))
                {
                    response = "Please provide an Integer! ";

                    return false;
                }

                if (Player.TryGet(sender, out Player p))
                {
                    AddEconomy(p, i, "Admin Allowance", 3);

                    response = "Given $" + i.ToString() + " to " + p.Nickname + "! ";

                    return true;
                }
                else
                {
                    response = "An unknown error occurred! ";

                    return false;
                }
            }

            if (!int.TryParse(arguments.Array[1], out int _i))
            {
                response = "Please provide an Integer! ";

                return false;
            }

            if (!int.TryParse(arguments.Array[2], out int id) && !Player.TryGetByName(arguments.Array[2], out Player _p))
            {
                response = "No player found with name: " + arguments.Array[2] + "! ";

                return false;
            }

            if (arguments.Array[2].ToCharArray()[0] == '@')
            {
                switch (arguments.Array[2].ToUpper())
                {
                    case "@ALL":
                        foreach (Player __p in Player.GetPlayers())
                        {
                            if (__p.IsAlive)
                                AddEconomy(__p, _i, "Admin Allowance", 3);
                        }
                        response = "Giving " + arguments.Array[1].ToUpper() + " to all Alive Players. ";
                        return false;
                    case "@HUMAN":
                        foreach (Player __p in Player.GetPlayers())
                        {
                            if (__p.IsAlive && !__p.IsSCP)
                                AddEconomy(__p, _i, "Admin Allowance", 3);
                        }
                        response = "Giving " + arguments.Array[1].ToUpper() + " to all Humans. ";
                        return false;
                    case "@SCP":
                        foreach (Player __p in Player.GetPlayers())
                        {
                            if (__p.IsAlive && __p.Role.GetFaction() == Faction.SCP)
                                AddEconomy(__p, _i, "Admin Allowance", 3);
                        }
                        response = "Giving " + arguments.Array[1].ToUpper() + " to all SCPs. ";
                        return false;
                    case "@MTF":
                        foreach (Player __p in Player.GetPlayers())
                        {
                            if (__p.IsAlive && __p.Role.GetFaction() == Faction.FoundationStaff)
                                AddEconomy(__p, _i, "Admin Allowance", 3);
                        }
                        response = "Giving " + arguments.Array[1].ToUpper() + " to all Foundation Forces. ";
                        return false;
                    case "@CI":
                        foreach (Player __p in Player.GetPlayers())
                        {
                            if (__p.IsAlive && __p.Role.GetFaction() == Faction.FoundationEnemy)
                                AddEconomy(__p, _i, "Admin Allowance", 3);
                        }
                        response = "Giving " + arguments.Array[1].ToUpper() + " to all Chaos Forces. ";
                        return false;
                }
            }

            if (!Player.TryGet(id, out _p))
            {
                response = "No player found with id: " + arguments.Array[2] + "! ";

                return false;
            }

            AddEconomy(_p, _i, "Admin Allowance", 3);

            response = "Given $" + _i.ToString() + " to " + _p.Nickname + "! ";

            return true;
        }

        public static void AddEconomy(int id, int amount, string reason, ushort time)
        {
            if (Player.TryGet(id, out Player p))
                p.SendBroadcast(reason + ": +$" + amount, time);

            if (Buying.playerEco.ContainsKey(id))
                Buying.playerEco[id] += amount;
            else
                Buying.playerEco.Add(id, amount);
        }

        public static void AddEconomy(Player p, int amount, string reason, ushort time)
        {
            AddEconomy(p.PlayerId, amount, reason, time);
        }
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SetEconomy : ICommand
    {
        public string Command { get; } = "seteconomy";

        public string[] Aliases { get; } = new string[] { "seteco", "seco" };

        public string Description { get; } = "Sets you or a player's money. Usage: \"seco <Integer> [Player ID/Player Name]\"";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.PlayersManagement))
            {
                response = "You Do Not Have Permission To Do That! Required Perm: Players Management";

                return false;
            }

            if (arguments.Array.Length < 2)
            {
                response = "Please provide the amount to set! ";

                return false;
            }

            if (arguments.Array.Length < 3)
            {
                if (!int.TryParse(arguments.Array[1], out int i))
                {
                    response = "Please provide an Integer! ";

                    return false;
                }

                if (Player.TryGet(sender, out Player p))
                {
                    OverrideEconomy(p, i);

                    response = "Set money for " + p.Nickname + " to $" + i.ToString() + "! ";

                    return true;
                }
                else
                {
                    response = "An unknown error occurred! ";

                    return false;
                }
            }

            if (!int.TryParse(arguments.Array[1], out int _i))
            {
                response = "Please provide an Integer! ";

                return false;
            }

            if (!int.TryParse(arguments.Array[2], out int id) && !Player.TryGetByName(arguments.Array[2], out Player _p))
            {
                response = "No player found with name: " + arguments.Array[2] + "! ";

                return false;
            }

            if (arguments.Array[2].ToCharArray()[0] == '@')
            {
                switch (arguments.Array[2].ToUpper())
                {
                    case "@ALL":
                        foreach (Player __p in Player.GetPlayers())
                        {
                            if (__p.IsAlive)
                                OverrideEconomy(__p, _i);
                        }
                        response = "Giving " + arguments.Array[1].ToUpper() + " to all Alive Players. ";
                        return false;
                    case "@HUMAN":
                        foreach (Player __p in Player.GetPlayers())
                        {
                            if (__p.IsAlive && !__p.IsSCP)
                                OverrideEconomy(__p, _i);
                        }
                        response = "Giving " + arguments.Array[1].ToUpper() + " to all Humans. ";
                        return false;
                    case "@SCP":
                        foreach (Player __p in Player.GetPlayers())
                        {
                            if (__p.IsAlive && __p.Role.GetFaction() == Faction.SCP)
                                OverrideEconomy(__p, _i);
                        }
                        response = "Giving " + arguments.Array[1].ToUpper() + " to all SCPs. ";
                        return false;
                    case "@MTF":
                        foreach (Player __p in Player.GetPlayers())
                        {
                            if (__p.IsAlive && __p.Role.GetFaction() == Faction.FoundationStaff)
                                OverrideEconomy(__p, _i);
                        }
                        response = "Giving " + arguments.Array[1].ToUpper() + " to all Foundation Forces. ";
                        return false;
                    case "@CI":
                        foreach (Player __p in Player.GetPlayers())
                        {
                            if (__p.IsAlive && __p.Role.GetFaction() == Faction.FoundationEnemy)
                                OverrideEconomy(__p, _i);
                        }
                        response = "Giving " + arguments.Array[1].ToUpper() + " to all Chaos Forces. ";
                        return false;
                }
            }

            if (!Player.TryGet(id, out _p))
            {
                response = "No player found with id: " + arguments.Array[2] + "! ";

                return false;
            }

            OverrideEconomy(_p, _i);

            response = "Set money for " + _p.Nickname + " to $" + _i.ToString() + "! ";

            return true;
        }

        public void OverrideEconomy(Player p, int amount)
        {
            if (Buying.playerEco.ContainsKey(p.PlayerId))
                Buying.playerEco[p.PlayerId] = amount;
            else
                Buying.playerEco.Add(p.PlayerId, amount);
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class Buy : ICommand
    {
        public string Command { get; } = "purchase";

        public string[] Aliases { get; } = new string[] { "pur" };

        public string Description { get; } = "Buys equipment when buying is on, execute without argument to list purchasable items. Usage: \".pur [Item Name]\"";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Buying.IsOn)
            {
                response = "Buying is disabled! ";

                return false;
            }

            if (arguments.Array.Length < 2)
            {
                response = "\nList of items: \n\n";

                foreach (string n in Buying.translatedItemBuys.Values)
                {
                    response += "- " + n + "  - $" + Buying.itemCosts[Buying.itemBuyTranslation[n]].ToString() + "\n";
                }

                if (Player.TryGet(sender, out Player _p) && Buying.playerEco.ContainsKey(_p.PlayerId))
                    response += "\nCurrent money: $" + Buying.playerEco[_p.PlayerId];
                else
                {
                    Buying.playerEco.Add(_p.PlayerId, 0);
                    response += "\nCurrent money: $" + Buying.playerEco[_p.PlayerId];
                }

                return true;
            }

            if (!Buying.itemBuyTranslation.ContainsKey(arguments.Array[1].ToUpper()))
            {
                response = "Please provide a valid item! ";

                return false;
            }

            if (Player.TryGet(sender, out Player p) && Buying.playerEco.ContainsKey(p.PlayerId))
            {
                if (p.IsSCP)
                {
                    response = "SCPs Cannot Buy! ";

                    return false;
                }

                if (Buying.playerEco[p.PlayerId] >= Buying.itemCosts[Buying.itemBuyTranslation[arguments.Array[1].ToUpper()]])
                {
                    if (p.IsInventoryFull)
                    {
                        response = "Inventory Full! ";

                        return false;
                    }
                    else
                    {
                        Buying.playerEco[p.PlayerId] -= Buying.itemCosts[Buying.itemBuyTranslation[arguments.Array[1].ToUpper()]];

                        CustomItem.AddCustomItem(p, Buying.itemBuyTranslation[arguments.Array[1].ToUpper()]);

                        p.SetAmmo(ItemType.Ammo9x19, (ushort)p.GetAmmoLimit(ItemType.Ammo9x19));
                        p.SetAmmo(ItemType.Ammo762x39, (ushort)p.GetAmmoLimit(ItemType.Ammo762x39));
                        p.SetAmmo(ItemType.Ammo556x45, (ushort)p.GetAmmoLimit(ItemType.Ammo556x45));
                        p.SetAmmo(ItemType.Ammo44cal, (ushort)p.GetAmmoLimit(ItemType.Ammo44cal));
                        p.SetAmmo(ItemType.Ammo12gauge, (ushort)p.GetAmmoLimit(ItemType.Ammo12gauge));

                        response = "Purchased " + Buying.translatedItemBuys[Buying.itemBuyTranslation[arguments.Array[1].ToUpper()]] + "! \nCurrent Money: $" + Buying.playerEco[p.PlayerId];

                        return true;
                    }
                }
                else
                {
                    response = "Not enough money! Current money: $" + Buying.playerEco[p.PlayerId];

                    return false;
                }
            }

            if (!Buying.playerEco.ContainsKey(p.PlayerId))
                Buying.playerEco.Add(p.PlayerId, 0);

            response = "You do not have an economy bound to you! Bounded new economy to you, please try again. ";

            return false;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class Economy : ICommand
    {
        public string Command { get; } = "economy";

        public string[] Aliases { get; } = new string[] { "eco" };

        public string Description { get; } = "Shows your economy when buying is on. Usage: \".eco\"";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (Player.TryGet(sender, out Player _p) && Buying.playerEco.ContainsKey(_p.PlayerId))
                response = "\nCurrent money: $" + Buying.playerEco[_p.PlayerId];
            else
            {
                Buying.playerEco.Add(_p.PlayerId, 0);
                response = "\nCurrent money: $" + Buying.playerEco[_p.PlayerId];
            }

            return true;
        }
    }
}
