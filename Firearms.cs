using CustomPlayerEffects;
using InventorySystem.Items.Firearms;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwiftKraft
{
    public class Firearms
    {
        private bool delay;

        [PluginEvent(ServerEventType.PlayerChangeItem)]
        public void OnPlayerChangeItem(Player player, ushort prevItemSerial, ushort currItemSerial)
        {
            if (player.CurrentItem == null || !Plugin.customItems.ContainsKey(currItemSerial) || player.CurrentItem.Category != ItemCategory.Firearm)
                return;

            Firearm firearm = (Firearm)player.CurrentItem;

            uint attachments = 0;
            string gunName = "";

            switch (Plugin.customItems[currItemSerial])
            {
                case "M4A1_S":
                    gunName = "M4A1-S";
                    attachments = 10523393;
                    if (firearm.Status.Ammo > 30)
                        firearm.Status = new FirearmStatus(30, FirearmStatusFlags.MagazineInserted, attachments);
                    break;
                case "M4A4":
                    gunName = "M4A4";
                    attachments = 10621473;
                    break;
                case "AN94":
                    gunName = "AN94";
                    attachments = 10752769;
                    if (firearm.Status.Ammo > 30)
                        firearm.Status = new FirearmStatus(30, FirearmStatusFlags.MagazineInserted, attachments);
                    break;
                case "SG553":
                    gunName = "SG553";
                    attachments = 10621700;
                    if (firearm.Status.Ammo > 30)
                        firearm.Status = new FirearmStatus(30, FirearmStatusFlags.MagazineInserted, attachments);
                    break;
                case "MP5SD":
                    gunName = "MP5SD";
                    attachments = 35089;
                    if (firearm.Status.Ammo > 30)
                        firearm.Status = new FirearmStatus(30, FirearmStatusFlags.MagazineInserted, attachments);
                    break;
                case "P90":
                    gunName = "P90";
                    attachments = 24849;
                    break;
                case "MAC_10":
                    gunName = "MAC-10";
                    attachments = 5393;
                    break;
                case "USP":
                    gunName = "USP-S";
                    attachments = 337;
                    break;
                case "P2000":
                    gunName = "P2000";
                    attachments = 2121;
                    break;
                case "GLOCK_17":
                    gunName = "Glock-17";
                    attachments = 51;
                    break;
                case "CZ75":
                    gunName = "CZ75-Auto";
                    attachments = 5385;
                    if (firearm.Status.Ammo > 12)
                        firearm.Status = new FirearmStatus(12, FirearmStatusFlags.MagazineInserted, attachments);
                    break;
                case "DEAGLE":
                    gunName = "Deagle";
                    attachments = 594;
                    if (firearm.Status.Ammo > 4)
                        firearm.Status = new FirearmStatus(4, FirearmStatusFlags.MagazineInserted, attachments);
                    break;
                case "M249":
                    gunName = "M249";
                    attachments = 580;
                    break;
                case "NOVA":
                    gunName = "Nova";
                    attachments = 1065;
                    break;
                case "AWP":
                    gunName = "AWP";
                    attachments = 136228;
                    if (firearm.Status.Ammo > 1)
                        firearm.Status = new FirearmStatus(1, FirearmStatusFlags.MagazineInserted, attachments);
                    break;
                case "SSG08":
                    gunName = "SSG08";
                    attachments = 10757256;
                    if (firearm.Status.Ammo > 1)
                        firearm.Status = new FirearmStatus(1, FirearmStatusFlags.MagazineInserted, attachments);
                    break;
                case "XR87":
                    gunName = "XR-87 Anti-SCP LMG";
                    attachments = 165122;
                    break;
                case "ECHO_S":
                    gunName = "ECHO-S";
                    attachments = 136258;
                    if (firearm.Status.Ammo > 20)
                        firearm.Status = new FirearmStatus(20, FirearmStatusFlags.MagazineInserted, attachments);
                    break;
                case "_FUNNY_GUN":
                    gunName = "Funny Gun";
                    attachments = 0;
                    break;
            }

            if (!string.IsNullOrEmpty(gunName))
                player.SendBroadcast("Equipped " + gunName + "!", 3, Broadcast.BroadcastFlags.Normal, true);

            firearm.Status = new FirearmStatus((byte)Mathf.Min(firearm.Status.Ammo, firearm.AmmoManagerModule.MaxAmmo), FirearmStatusFlags.MagazineInserted, attachments);
        }

        [PluginEvent(ServerEventType.PlayerReloadWeapon)]
        public void OnPlayerReloadWeapon(Player player, Firearm firearm)
        {
            if (!Plugin.customItems.ContainsKey(player.CurrentItem.ItemSerial) || delay)
                return;

            switch (Plugin.customItems[player.CurrentItem.ItemSerial])
            {
                case "AWP":
                    firearm.OnStatusChanged += (a, b) =>
                    {
                        if (b.Ammo > 1)
                        {
                            player.AddAmmo(ItemType.Ammo762x39, (ushort)(firearm.Status.Ammo - 1));
                            firearm.Status = new FirearmStatus(1, FirearmStatusFlags.Chambered, 136228);
                        }
                    };
                    break;
                case "SSG08":
                    firearm.OnStatusChanged += (a, b) =>
                    {
                        if (b.Ammo > 1)
                        {
                            player.AddAmmo(ItemType.Ammo556x45, (ushort)(firearm.Status.Ammo - 1));
                            firearm.Status = new FirearmStatus(1, FirearmStatusFlags.Chambered, 10757256);
                        }
                    };
                    break;
                case "CZ75":
                    firearm.OnStatusChanged += (a, b) =>
                    {
                        if (b.Ammo > 12)
                        {
                            player.AddAmmo(ItemType.Ammo9x19, (ushort)(firearm.Status.Ammo - 12));
                            firearm.Status = new FirearmStatus(12, firearm.Status.Flags, 5385);
                        }
                    };
                    break;
                case "ECHO_S":
                    firearm.OnStatusChanged += (a, b) =>
                    {
                        if (b.Ammo > 20)
                        {
                            player.AddAmmo(ItemType.Ammo762x39, (ushort)(firearm.Status.Ammo - 20));
                            firearm.Status = new FirearmStatus(20, firearm.Status.Flags, 136258);
                        }
                    };
                    break;
            }
        }

        [PluginEvent(ServerEventType.PlayerDamage)]
        public void OnPlayerDamage(Player victim, Player attacker, DamageHandlerBase damage)
        {
            if (attacker == null)
                return;

            if (damage is FirearmDamageHandler standard)
            {
                // Log.Info("Dealt Damage: " + standard.DealtHealthDamage + " " + standard.Damage);

                victim.EffectsManager.EnableEffect<Sinkhole>(0.1f, true);

                if (!Plugin.customItems.ContainsKey(attacker.CurrentItem.ItemSerial) || delay)
                    return;

                delay = true;

                float multiplier = 0f;
                float hitmarker = 1f;

                switch (standard.Hitbox)
                {
                    case HitboxType.Headshot:
                        hitmarker = 2.5f;
                        switch (Plugin.customItems[attacker.CurrentItem.ItemSerial])
                        {
                            case "M4A1_S":
                                multiplier = 2f;
                                break;
                            case "M4A4":
                                multiplier = 2.1f;
                                break;
                            case "AN94":
                                multiplier = 4f;
                                break;
                            case "SG553":
                                multiplier = 3f;
                                break;
                            case "SSG08":
                                multiplier = 5f;
                                break;
                            case "MP5SD":
                                multiplier = 0.65f;
                                break;
                            case "P90":
                                multiplier = 1.4f;
                                break;
                            case "MAC_10":
                                multiplier = 0.5f;
                                break;
                            case "USP":
                                multiplier = 3.5f;
                                break;
                            case "P2000":
                                multiplier = 3.5f;
                                break;
                            case "GLOCK_17":
                                multiplier = 0.7f;
                                break;
                            case "CZ75":
                                multiplier = 2.5f;
                                break;
                            case "DEAGLE":
                                multiplier = 0.5f;
                                break;
                            case "AWP":
                                multiplier = 7f;
                                break;
                            case "ECHO_S":
                                multiplier = 4.5f;
                                break;
                            case "_FUNNY_GUN":
                                multiplier = 500f;
                                break;
                        }
                        break;
                    case HitboxType.Body:
                        hitmarker = 1f;
                        switch (Plugin.customItems[attacker.CurrentItem.ItemSerial])
                        {
                            case "M4A1_S":
                                multiplier = 0.05f;
                                break;
                            case "M4A4":
                                multiplier = 0.2f;
                                break;
                            case "AN94":
                                multiplier = 0.3f;
                                break;
                            case "SG553":
                                multiplier = 0.2f;
                                break;
                            case "SSG08":
                                multiplier = 2.175f;
                                break;
                            case "MP5SD":
                                multiplier = 0.2f;
                                break;
                            case "P90":
                                multiplier = 0.1f;
                                break;
                            case "USP":
                                multiplier = 1f;
                                break;
                            case "P2000":
                                multiplier = 1f;
                                break;
                            case "GLOCK_17":
                                multiplier = 0.3f;
                                break;
                            case "CZ75":
                                multiplier = 0.45f;
                                break;
                            case "DEAGLE":
                                multiplier = -0.5f;
                                break;
                            case "AWP":
                                multiplier = 4f;
                                break;
                            case "M249":
                                multiplier = -0.2f;
                                break;
                            case "ECHO_S":
                                multiplier = 0.5f;
                                break;
                            case "XR87":
                                multiplier = -0.2f;
                                break;
                            case "_FUNNY_GUN":
                                multiplier = 500f;
                                break;
                        }
                        break;
                    case HitboxType.Limb:
                        hitmarker = 0.7f;
                        switch (Plugin.customItems[attacker.CurrentItem.ItemSerial])
                        {
                            case "M4A1_S":
                                multiplier = 0.05f;
                                break;
                            case "M4A4":
                                multiplier = 0.05f;
                                break;
                            case "AN94":
                                multiplier = 0.2f;
                                break;
                            case "SG553":
                                multiplier = 0.2f;
                                break;
                            case "SSG08":
                                multiplier = 1.085f;
                                break;
                            case "MP5SD":
                                multiplier = 0.1f;
                                break;
                            case "USP":
                                multiplier = 0.25f;
                                break;
                            case "P2000":
                                multiplier = 0.25f;
                                break;
                            case "GLOCK_17":
                                multiplier = 0.2f;
                                break;
                            case "DEAGLE":
                                multiplier = -0.5f;
                                break;
                            case "AWP":
                                multiplier = 2.5f;
                                break;
                            case "M249":
                                multiplier = -0.2f;
                                break;
                            case "ECHO_S":
                                multiplier = 0.1f;
                                break;
                            case "XR87":
                                multiplier = -0.2f;
                                break;
                            case "_FUNNY_GUN":
                                multiplier = 500f;
                                break;
                        }
                        break;
                }

                if (victim.IsSCP)
                {
                    if (Buying.IsOn)
                    {
                        if (Buying.playerEco.ContainsKey(attacker.PlayerId))
                            Buying.playerEco[attacker.PlayerId] += 10;
                        else
                            Buying.playerEco.Add(attacker.PlayerId, 10);
                    }

                    switch (Plugin.customItems[attacker.CurrentItem.ItemSerial])
                    {
                        case "AWP":
                            multiplier = 7f;
                            break;
                        case "XR87":
                            multiplier = 2.2f;
                            break;
                        case "P90":
                            multiplier = 0.3f;
                            break;
                        case "ECHO_S":
                            multiplier = 0f;
                            break;
                        case "DEAGLE":
                            multiplier = 1f;
                            break;
                    }
                }

                if (multiplier >= 0f)
                    victim.Damage(new FirearmDamageHandler((Firearm)attacker.CurrentItem, standard.Damage * multiplier, false));
                else
                    victim.Heal(standard.Damage * Mathf.Abs(multiplier));

                attacker.ReferenceHub.StartCoroutine(HitMarker(attacker, hitmarker));

                delay = false;
            }
        }

        public IEnumerator HitMarker(Player attacker, float hitmarker)
        {
            yield return new WaitForSeconds(0.1f);

            attacker.ReceiveHitMarker(hitmarker);
        }

        [PluginEvent(ServerEventType.PlayerAimWeapon)]
        public void OnPlayerAimWeapon(Player player, Firearm firearm, bool aiming)
        {
            // Log.Info(firearm.Status.Attachments.ToString());

            if (!Plugin.customItems.ContainsKey(firearm.ItemSerial))
                return;

            uint attachments = 0;

            switch (Plugin.customItems[player.CurrentItem.ItemSerial])
            {
                case "M4A1_S":
                    attachments = 10523393;
                    break;
                case "M4A4":
                    attachments = 10621473;
                    break;
                case "AN94":
                    attachments = 10752769;
                    break;
                case "SG553":
                    attachments = 10621700;
                    break;
                case "SSG08":
                    attachments = 10757256;
                    break;
                case "MP5SD":
                    attachments = 35089;
                    break;
                case "P90":
                    attachments = 24849;
                    break;
                case "MAC_10":
                    attachments = 5393;
                    break;
                case "USP":
                    attachments = 337;
                    break;
                case "P2000":
                    attachments = 2121;
                    break;
                case "GLOCK_17":
                    attachments = 51;
                    break;
                case "CZ75":
                    attachments = 5385;
                    break;
                case "DEAGLE":
                    attachments = 594;
                    break;
                case "M249":
                    attachments = 580;
                    break;
                case "NOVA":
                    attachments = 1065;
                    break;
                case "AWP":
                    attachments = 136228;
                    break;
                case "XR87":
                    attachments = 165122;
                    break;
                case "ECHO_S":
                    attachments = 136258;
                    break;
                case "_FUNNY_GUN":
                    attachments = 0;
                    break;
            }

            firearm.Status = new FirearmStatus(firearm.Status.Ammo, firearm.Status.Flags, attachments);
        }

        [PluginEvent(ServerEventType.PlayerShotWeapon)]
        public void OnShotWeapon(Player player, Firearm firearm)
        {
            if (!Plugin.customItems.ContainsKey(firearm.ItemSerial))
                return;

            uint attachments = 0;

            switch (Plugin.customItems[player.CurrentItem.ItemSerial])
            {
                case "M4A1_S":
                    attachments = 10523393;
                    break;
                case "M4A4":
                    attachments = 10621473;
                    break;
                case "AN94":
                    attachments = 10752769;
                    break;
                case "SG553":
                    attachments = 10621700;
                    break;
                case "SSG08":
                    attachments = 10757256;
                    break;
                case "MP5SD":
                    attachments = 35089;
                    break;
                case "P90":
                    attachments = 24849;
                    break;
                case "MAC_10":
                    attachments = 5393;
                    break;
                case "USP":
                    attachments = 337;
                    break;
                case "P2000":
                    attachments = 2121;
                    break;
                case "GLOCK_17":
                    attachments = 51;
                    break;
                case "CZ75":
                    attachments = 5385;
                    break;
                case "DEAGLE":
                    attachments = 594;
                    break;
                case "M249":
                    attachments = 580;
                    break;
                case "NOVA":
                    attachments = 1065;
                    break;
                case "AWP":
                    attachments = 136228;
                    break;
                case "XR87":
                    attachments = 165122;
                    break;
                case "ECHO_S":
                    attachments = 136258;
                    break;
                case "_FUNNY_GUN":
                    attachments = 0;
                    break;
            }

            firearm.Status = new FirearmStatus(firearm.Status.Ammo, firearm.Status.Flags, attachments);
        }
    }
}
