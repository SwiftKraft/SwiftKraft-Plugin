using CommandSystem;
using CustomPlayerEffects;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
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
/*        [PluginConfig]
        public Config cfg;*/

        public static readonly Dictionary<ushort, string> customItems = new Dictionary<ushort, string>();
        public static int killTarget;

        [PluginEntryPoint("SwiftKraft", "v1.0", "Powerful Guns", "SwiftKraft")]
        public void Init()
        {
            EventManager.RegisterEvents<Plugin>(this);
            EventManager.RegisterEvents<Firearms>(this);
            EventManager.RegisterEvents<CustomLoadout>(this);
            EventManager.RegisterEvents<CustomItemConversion>(this);

            #region Register Custom Items

            CustomItem.itemNameToItemId.Add("M4A1_S", 20);
            CustomItem.itemNameToItemId.Add("M4A4", 20);
            CustomItem.itemNameToItemId.Add("AN94", 20);
            CustomItem.itemNameToItemId.Add("SG553", 20);
            CustomItem.itemNameToItemId.Add("SSG08", 20);
            CustomItem.itemNameToItemId.Add("USP", 30);
            CustomItem.itemNameToItemId.Add("P2000", 30);
            CustomItem.itemNameToItemId.Add("MP5SD", 21);
            CustomItem.itemNameToItemId.Add("P90", 21);
            CustomItem.itemNameToItemId.Add("MAC_10", 23);
            CustomItem.itemNameToItemId.Add("GLOCK_17", 13);
            CustomItem.itemNameToItemId.Add("CZ75", 23);
            CustomItem.itemNameToItemId.Add("DEAGLE", 39);
            CustomItem.itemNameToItemId.Add("AWP", 40);
            CustomItem.itemNameToItemId.Add("M249", 24);
            CustomItem.itemNameToItemId.Add("NOVA", 41);
            CustomItem.itemNameToItemId.Add("XR87", 40);
            CustomItem.itemNameToItemId.Add("ECHO_S", 40);
            CustomItem.itemNameToItemId.Add("_FUNNY_GUN", 20);
            CustomItem.itemNameToItemId.Add("_GUARD_SPAWN", 4);
            CustomItem.itemNameToItemId.Add("_PROTO_MEDKIT", 14);

            #endregion
        }

        [PluginEvent(ServerEventType.PlayerJoined)]
        public void OnPlayerJoin(Player player)
        {
            player.SendBroadcast("Welcome To The Server, " + player.Nickname + "! ", 5);
        }

        [PluginEvent(ServerEventType.PlayerDeath)]
        public void OnPlayerDeath(Player victim, Player attacker, DamageHandlerBase damageHandlerBase)
        {
            if (attacker == null)
                return;

            Log.Info(attacker.Nickname + " (" + attacker.Role.ToString() + ")" + " Killed " + victim.Nickname + " (" + victim.Role.ToString() + ")");
            attacker.SendBroadcast("You Killed " + victim.Nickname, 2, Broadcast.BroadcastFlags.Normal, true);

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
                    message = "Equipped Guard Summon! (Drop to Use)";
                    break;
                case "_PROTO_MEDKIT":
                    message = "Equipped Prototype Medkit! ";
                    break;
            }

            if (!string.IsNullOrEmpty(message))
                player.SendBroadcast(message, 3, Broadcast.BroadcastFlags.Normal, true);
        }

/*        [PluginEvent(ServerEventType.ItemSpawned)]
        public void OnItemSpawned(ItemType item, Vector3 position)
        {
            Log.Info("Item Spawned! Item: " + item.ToString());
        }*/

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

                target.SetRole(RoleTypeId.FacilityGuard);
                target.Position = player.Position;
                target.SendBroadcast("You have been summoned by " + player.Nickname, 3, Broadcast.BroadcastFlags.Normal, true);

                item.PickupDropModel.DestroySelf();
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

        /*[PluginEvent(ServerEventType.PlayerDryfireWeapon)]
        public void OnPlayerDryfireWeapon(Player player, Firearm firearm)
        {

        }*/

        /*[PluginEvent(ServerEventType.TeamRespawn)]
        public void OnTeamRespawn(SpawnableTeamType spawnableTeamType)
        {
            
        }*/

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

            Server.Instance.ReferenceHub.StartCoroutine(Announce(scp, damage, count));
        }

        private IEnumerator Announce(Player scp, DamageHandlerBase damage, int count)
        {
            yield return new WaitForSeconds(2f);

            Cassie.Message(count.ToString() + " SCP 0 4 9 2 detected", isSubtitles: true);
        }

        [PluginEvent(ServerEventType.RoundRestart)]
        public void OnRoundRestart()
        {
            Log.Info("Round Restarting! Clearing Custom Item Entries! ");

            customItems.Clear();
        }

/*        public class Config
        {
            public float CustomItemChance = 50f;
        }*/
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
===== SwiftKraft v1.0 =====

Plugin Made By SwiftKraft! 

Adds custom items and weapons to the game! 

===========================

Commands: 

- swiftkraft - Shows this message <3. Aliases: skabout, swftkft.
- listcustomitem - Lists all registered custom items. Aliases: custlist, clist.
- customitem <Custom Item Name> [Player Name/Player ID] - Gives custom item to you or a player. Aliases: custitem, citem, cust.
- killtarget <Player Name/Player ID> - Sets kill target, killer of kill target will be broadcasted when kill target dies. Aliases: ktarget, target, kt.
- conversion <1/0> - Turns on or off for conversion of custom items (spawn loadouts and pickups). Aliases: conv, cnvs, cv.
- attachments - Displays the attachment combination number for your current weapon. Aliases: att, atch.

===========================
";

            return true;
        }
    }
}
