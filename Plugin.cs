using CommandSystem;
using CustomPlayerEffects;
using Footprinting;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Pickups;
using MapGeneration.Distributors;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Core.Items;
using PluginAPI.Enums;
using PluginAPI.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SwiftKraft
{
    public class Plugin
    {
        public static readonly Dictionary<ushort, string> customItems = new Dictionary<ushort, string>();
        public static readonly Dictionary<int, int> kills = new Dictionary<int, int>();
        public static int killTarget;

        public List<Scp079Generator> generators = new List<Scp079Generator>();

        [PluginEntryPoint("SwiftKraft", "v1.5", "Powerful Guns", "SwiftKraft")]
        public void Init()
        {
            CustomItemConversion.IsOn = true;

            EventManager.RegisterEvents<Plugin>(this);
            EventManager.RegisterEvents<Firearms>(this);
            EventManager.RegisterEvents<CustomLoadout>(this);
            EventManager.RegisterEvents<CustomItemConversion>(this);
            EventManager.RegisterEvents<SCPBuffs>(this);

            #region Register Custom Items

            RegisterItem("M4A1_S", ItemType.GunE11SR, 2900);
            RegisterItem("M4A4", ItemType.GunE11SR, 2900);
            RegisterItem("AN94", ItemType.GunE11SR, 2900);
            RegisterItem("SG553", ItemType.GunE11SR, 3100);
            RegisterItem("SSG08", ItemType.GunE11SR, 1150);
            RegisterItem("USP", ItemType.GunCOM18, 500);
            RegisterItem("P2000", ItemType.GunCOM18, 550);
            RegisterItem("MP5SD", ItemType.GunCrossvec, 1600);
            RegisterItem("P90", ItemType.GunCrossvec, 1750);
            RegisterItem("MAC_10", ItemType.GunFSP9, 950);
            RegisterItem("CZ75", ItemType.GunFSP9, 450);
            RegisterItem("GLOCK_17", ItemType.GunCOM15, 300);
            RegisterItem("DEAGLE", ItemType.GunRevolver, 700);
            RegisterItem("AWP", ItemType.GunAK, 4700);
            RegisterItem("XR87", ItemType.GunAK, 4200);
            RegisterItem("ECHO_S", ItemType.GunAK, 5200);
            RegisterItem("M249", ItemType.GunLogicer, 3250);
            RegisterItem("NOVA", ItemType.GunShotgun, 1150);
            RegisterBuy("_26", 300);
            RegisterBuy("_25", 500);
            RegisterBuy("_14", 300);
            RegisterBuy("_33", 600);
            RegisterBuy("_34", 150);
            RegisterBuy("_18", 1000);
            RegisterBuy("_46", 800);
            RegisterBuy("_32", 1400);
            RegisterBuy("_31", 2000);
            RegisterBuy("_36", 400);
            RegisterBuy("_37", 1000);
            RegisterBuy("_38", 1200);
            CustomItem.itemNameToItemId.Add("_FUNNY_GUN", 20);
            CustomItem.itemNameToItemId.Add("_GUARD_SPAWN", 4);
            CustomItem.itemNameToItemId.Add("_GOLDEN_GUN", 39);
            CustomItem.itemNameToItemId.Add("_PROTO_MEDKIT", 14);
            CustomItem.itemNameToItemId.Add("_PROTO_GRENADE", 25);
            CustomItem.itemNameToItemId.Add("_CLUSTER_GRENADE", 25);

            #endregion
        }

        public void RegisterItem(string item, ItemType itemTypeId, int cost)
        {
            CustomItem.itemNameToItemId.Add(item.ToUpper(), (ushort)itemTypeId);
            RegisterBuy(item.ToUpper(), cost);
        }

        public void RegisterBuy(string buy, int cost)
        {
            Buying.itemCosts.Add(buy.ToUpper(), cost);

            if (buy.ToCharArray()[0] == '_')
            {
                if (!uint.TryParse(buy.Remove(0, 1), out uint i))
                    return;

                ItemType item = (ItemType)i;

                Buying.translatedItemBuys.Add(buy.ToUpper(), item.ToString().ToUpper());
                Buying.itemBuyTranslation.Add(item.ToString().ToUpper(), buy.ToUpper());
            }
            else
            {
                Buying.translatedItemBuys.Add(buy.ToUpper(), buy.ToUpper());
                Buying.itemBuyTranslation.Add(buy.ToUpper(), buy.ToUpper());
            }
        }

        [PluginEvent(ServerEventType.PlayerJoined)]
        public void OnPlayerJoin(Player player)
        {
            player.SendBroadcast("Welcome To The Server, " + player.Nickname + "! ", 5);
        }

        [PluginEvent(ServerEventType.PlayerDeath)]
        public void OnPlayerDeath(Player victim, Player attacker, DamageHandlerBase damageHandlerBase)
        {
            if (killTarget != 0)
            {
                if (victim.PlayerId == killTarget)
                {
                    if (attacker != null)
                    {
                        Log.Info("Kill Target: " + victim.Nickname + " Has Been Killed By " + attacker.Nickname + "!");
                        foreach (Player p in Player.GetPlayers())
                            p.SendBroadcast("Kill Target: " + victim.Nickname + " Has Been Killed By " + attacker.Nickname + "!", 3, Broadcast.BroadcastFlags.Normal, false);
                    }
                    else
                    {
                        Log.Info("Kill Target: " + victim.Nickname + " Has Been Killed! ");
                        foreach (Player p in Player.GetPlayers())
                            p.SendBroadcast("Kill Target: " + victim.Nickname + " Has Been Killed! ", 3, Broadcast.BroadcastFlags.Normal, false);
                    }
                }
            }

            if (attacker == null)
                return;

            if (kills.ContainsKey(attacker.PlayerId))
                kills[attacker.PlayerId]++;
            else
                kills.Add(attacker.PlayerId, 1);

            string msg = "";

            switch (kills[attacker.PlayerId])
            {
                case 2:
                    msg = "DOUBLE KILL! ";
                    break;
                case 3:
                    msg = "TRIPLE KILL! ";
                    break;
                case 4:
                    msg = "QUADRAKILL! ";
                    break;
                case 5:
                    msg = "PENTAKILL!! ";
                    break;
                case 6:
                    msg = "ULTRAKILL!!! ";
                    break;
                case 7:
                    msg = "KILLING SPREE!!! ";
                    break;
                case 8:
                    msg = "UNSTOPPABLE!!! ";
                    break;
                default:
                    if (kills[attacker.PlayerId] > 8)
                        msg = "UNSTOPPABLE!!! x" + (kills[attacker.PlayerId] - 7);
                    break;
            }

            attacker.SendBroadcast("You Killed " + victim.Nickname + "! " + msg, 2, Broadcast.BroadcastFlags.Normal, true);

            attacker.ReferenceHub.StartCoroutine(KillCounter(attacker.PlayerId));

            if (Buying.IsOn)
            {
                if (victim.PlayerId != attacker.PlayerId)
                    GiveEconomy.AddEconomy(attacker, 400, "Terminated Enemy", 3);

                if (kills[attacker.PlayerId] > 1)
                {
                    int killstreakBonus = 100 * kills[attacker.PlayerId];

                    GiveEconomy.AddEconomy(attacker, killstreakBonus, "Killstreak Bonus", 3);
                }
            }
        }

        public IEnumerator KillCounter(int atk)
        {
            int c = kills[atk];

            yield return new WaitForSeconds(7f);

            if (c >= kills[atk])
                kills[atk] = 0;
        }

        [PluginEvent(ServerEventType.PlayerChangeItem)]
        public void OnPlayerChangeItem(Player player, ushort prevItemSerial, ushort currItemSerial)
        {
            if (player.CurrentItem == null || !customItems.ContainsKey(currItemSerial))
                return;

            string message = "";

            switch (customItems[player.CurrentItem.ItemSerial])
            {
                case "_GUARD_SPAWN":
                    message = "Equipped Bodyguard Summon! (Drop to Use)";
                    break;
                case "_PROTO_MEDKIT":
                    message = "Equipped Prototype Medkit! ";
                    break;
                case "_PROTO_GRENADE":
                    message = "Equipped Prototype Grenade! ";
                    break;
                case "_CLUSTER_GRENADE":
                    message = "Equipped Cluster Grenade! ";
                    break;
            }

            if (!string.IsNullOrEmpty(message))
                player.SendBroadcast(message, 3, Broadcast.BroadcastFlags.Normal, true);
        }

        [PluginEvent(ServerEventType.PlayerDropItem)]
        public void OnPlayerDropItem(Player player, ItemBase item)
        {
            if (!customItems.ContainsKey(item.ItemSerial))
                return;

            if (item.ItemTypeId == ItemType.KeycardGuard)
            {
                List<Player> players = new List<Player>();

                foreach (Player p in Player.GetPlayers())
                {
                    if (!p.IsAlive)
                        players.Add(p);
                }

                if (players.Count < 1)
                    return;

                Player target = players[Random.Range(0, players.Count)];

                switch (player.Role.GetFaction())
                {
                    case Faction.FoundationStaff:
                        target.SetRole(RoleTypeId.NtfSergeant);
                        break;
                    case Faction.FoundationEnemy:
                        target.SetRole(RoleTypeId.ChaosRifleman);
                        break;
                    case Faction.Unclassified:
                        target.SetRole(RoleTypeId.FacilityGuard);
                        break;
                    case Faction.SCP:
                        target.SetRole(RoleTypeId.Scp0492);
                        break;
                }

                target.Position = player.Position;
                target.SendBroadcast("You have been summoned by " + player.Nickname + ", protect them! ", 3, Broadcast.BroadcastFlags.Normal, true);

                customItems.Remove(item.ItemSerial);
            }
        }

        [PluginEvent(ServerEventType.PlayerUsedItem)]
        public void OnPlayerUsedItem(Player player, ItemBase item)
        {
            if (!customItems.ContainsKey(item.ItemSerial))
                return;

            if (customItems[item.ItemSerial] == "_PROTO_MEDKIT")
            {
                player.SendBroadcast("Used Prototype Medkit! Reserve Painkillers Added! ", 3, Broadcast.BroadcastFlags.Normal, true);

                if (!player.IsInventoryFull)
                    player.AddItem(ItemType.Painkillers);
                else
                {
                    ItemPickup i = ItemPickup.Create(ItemType.Painkillers, player.Position, Quaternion.identity);
                    i.Spawn();
                }

                player.EffectsManager.EnableEffect<Invigorated>(3f, true);
            }

            customItems.Remove(item.ItemSerial);
        }

        [PluginEvent(ServerEventType.CassieAnnouncesScpTermination)]
        public void OnSCPAnnounced(Player scp, DamageHandlerBase damage, string announcement)
        {
            if (scp.Role != RoleTypeId.Scp049)
                return;

            int count = 0;

            foreach (Player p in Player.GetPlayers())
            {
                if (p.Role == RoleTypeId.Scp0492)
                    count++;
            }

            Server.Instance.ReferenceHub.StartCoroutine(Announce(count));
        }

        [PluginEvent(ServerEventType.GrenadeExploded)]
        public void OnGrenadeExploded(Footprint owner, Vector3 position, ItemPickupBase item)
        {
            if (!customItems.ContainsKey(item.NetworkInfo.Serial))
                return;

            switch (customItems[item.NetworkInfo.Serial])
            {
                case "_PROTO_GRENADE":
                    ItemPickup i = ItemPickup.Create(ItemType.GrenadeFlash, item.NetworkInfo.RelativePosition.Position, item.NetworkInfo.RelativeRotation.Value);
                    i.Spawn();
                    i.Rigidbody.AddForceAtPosition(new Vector3(20f, 1f, 20f), item.NetworkInfo.RelativePosition.Position, ForceMode.Impulse);
                    ItemPickup i2 = ItemPickup.Create(ItemType.GrenadeFlash, item.NetworkInfo.RelativePosition.Position, item.NetworkInfo.RelativeRotation.Value);
                    i2.Spawn();
                    i2.Rigidbody.AddForceAtPosition(new Vector3(-20f, 1f, 20f), item.NetworkInfo.RelativePosition.Position, ForceMode.Impulse);
                    ItemPickup i3 = ItemPickup.Create(ItemType.GrenadeFlash, item.NetworkInfo.RelativePosition.Position, item.NetworkInfo.RelativeRotation.Value);
                    i3.Spawn();
                    i3.Rigidbody.AddForceAtPosition(new Vector3(20f, 1f, -20f), item.NetworkInfo.RelativePosition.Position, ForceMode.Impulse);
                    ItemPickup i4 = ItemPickup.Create(ItemType.GrenadeFlash, item.NetworkInfo.RelativePosition.Position, item.NetworkInfo.RelativeRotation.Value);
                    i4.Spawn();
                    i4.Rigidbody.AddForceAtPosition(new Vector3(-20f, 1f, -20f), item.NetworkInfo.RelativePosition.Position, ForceMode.Impulse);
                    break;
                case "_CLUSTER_GRENADE":
                    ItemPickup ii = ItemPickup.Create(ItemType.GrenadeHE, item.NetworkInfo.RelativePosition.Position, item.NetworkInfo.RelativeRotation.Value);
                    ii.Spawn();
                    ii.Rigidbody.AddForceAtPosition(new Vector3(40f, 5f, 40f), item.NetworkInfo.RelativePosition.Position, ForceMode.Impulse);
                    ItemPickup ii2 = ItemPickup.Create(ItemType.GrenadeHE, item.NetworkInfo.RelativePosition.Position, item.NetworkInfo.RelativeRotation.Value);
                    ii2.Spawn();
                    ii2.Rigidbody.AddForceAtPosition(new Vector3(-40f, 5f, 40f), item.NetworkInfo.RelativePosition.Position, ForceMode.Impulse);
                    ItemPickup ii3 = ItemPickup.Create(ItemType.GrenadeHE, item.NetworkInfo.RelativePosition.Position, item.NetworkInfo.RelativeRotation.Value);
                    ii3.Spawn();
                    ii3.Rigidbody.AddForceAtPosition(new Vector3(40f, 5f, -40f), item.NetworkInfo.RelativePosition.Position, ForceMode.Impulse);
                    ItemPickup ii4 = ItemPickup.Create(ItemType.GrenadeHE, item.NetworkInfo.RelativePosition.Position, item.NetworkInfo.RelativeRotation.Value);
                    ii4.Spawn();
                    ii4.Rigidbody.AddForceAtPosition(new Vector3(-40f, 5f, -40f), item.NetworkInfo.RelativePosition.Position, ForceMode.Impulse);
                    break;
            }
        }

        private IEnumerator Announce(int count)
        {
            yield return new WaitForSeconds(2f);

            Cassie.Message(count.ToString() + " SCP 0 4 9 2 detected", isSubtitles: true);
        }

        [PluginEvent(ServerEventType.RoundRestart)]
        public void OnRoundRestart()
        {
            Log.Info("Round Restarting! Clearing Custom Items and Economy! ");

            customItems.Clear();
            kills.Clear();
            generators.Clear();
            Buying.playerEco.Clear();
        }

        [PluginEvent(ServerEventType.LczDecontaminationStart)]
        public void OnLczDecontaminationStarts()
        {
            if (Buying.IsOn)
            {
                Log.Info("Started LCZ Decontamination. Giving Economy. ");

                foreach (Player p in Player.GetPlayers())
                {
                    GiveEconomy.AddEconomy(p, 3000, "LCZ Decontamination Started", 4);
                }
            }
        }

        [PluginEvent(ServerEventType.PlayerActivateGenerator)]
        public void OnPlayerActivateGenerator(Player plr, Scp079Generator gen)
        {
            if (Buying.IsOn && !plr.IsSCP)
            {
                if (!generators.Contains(gen))
                {
                    generators.Add(gen);

                    GiveEconomy.AddEconomy(plr, 1000, "Activated Generator", 3);

                    foreach (Player p in Player.GetPlayers())
                    {
                        if (!p.IsAlive || p.Role.GetFaction() != plr.Role.GetFaction())
                            continue;

                        GiveEconomy.AddEconomy(p, 2000, "Team Activated Generator", 2);
                    }
                }
            }
        }

        [PluginEvent(ServerEventType.PlayerUnlockGenerator)]
        public void OnUnlockGenerator(Player plr, Scp079Generator generator)
        {
            if (Buying.IsOn)
            {
                GiveEconomy.AddEconomy(plr, 400, "Unlocked Generator", 3);
            }
        }

        [PluginEvent(ServerEventType.PlayerEscape)]
        public void OnPlayerEscaped(Player plr, RoleTypeId role)
        {
            if (Buying.IsOn)
            {
                if (role.GetFaction() == Faction.FoundationStaff)
                    GiveEconomy.AddEconomy(plr, 3500, "Escaped Facility", 4);
                else
                    GiveEconomy.AddEconomy(plr, 4000, "Escaped Facility", 4);
            }
        }
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class About : ICommand
    {
        public string Command { get; } = "swiftkraft";

        public string[] Aliases { get; } = new string[] { "skabout", "swftkft" };

        public string Description { get; } = "Tells you what this plugin is all about :) . Usage: \"swiftkraft\"";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response =

@"
===== SwiftKraft v1.5 =====

Plugin Made By SwiftKraft! 

Adds custom items and weapons to the game! 

===========================

RA Commands: 

- swiftkraft - Shows this message <3. Aliases: skabout, swftkft.
- listcustomitem - Lists all registered custom items. Aliases: custlist, clist.
- customitem <Custom Item Name> [Player Name/Player ID] - Gives custom item to you or a player. Aliases: custitem, citem, cust. Targeters: @ALL, @HUMAN, @SCP, @MTF, @CI
- killtarget <Player Name/Player ID> - Sets kill target, killer of kill target will be broadcasted when kill target dies. Aliases: ktarget, target, kt.
- conversion <1/0> - Turns on or off for conversion of custom items (spawn loadouts and pickups). Aliases: conv, cnvs, cv.
- attachments - Displays the attachment combination serial number for your current weapon, mainly for ease of adding new weapons. Aliases: att, atch.
- allowbuy <1/0> - Turns on and off economy and purchasing of weapons through the console. Aliases: buy, allowb.
- cleareconomy - Clears everyone's money. Aliases: cleareco, cleco.
- giveeconomy <Integer> [Player Name/Player ID] - Gives you or a player money. Aliases: giveeco, geco.
- seteconomy <Integer> [Player Name/Player ID] - Sets you or a player's money. Aliases: seteco, seco.

Client Commands:

- .purchase [Item Name] - Shows a list of purchasable items or attempts to purchase an item if provided. Aliases: .pur.
- .economy - Shows the amount of money you have. Aliases: .eco.

===========================
";

            return true;
        }
    }
}
