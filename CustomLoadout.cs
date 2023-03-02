using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using System.Collections;
using UnityEngine;

namespace SwiftKraft
{
    public class CustomLoadout
    {
        [PluginEvent(ServerEventType.PlayerSpawn)]
        public void OnPlayerSpawn(Player player, RoleTypeId roleTypeId)
        {
            if (!CustomItemConversion.IsOn)
                return;

            player.ReferenceHub.StartCoroutine(GrantLoadout(player, roleTypeId));
        }

        public IEnumerator GrantLoadout(Player player, RoleTypeId roleTypeId)
        {
            yield return new WaitForSeconds(0.2f);

            player.ClearInventory(true);

            switch (roleTypeId)
            {
                case RoleTypeId.NtfPrivate:
                    player.AddAmmo(ItemType.Ammo9x19, 170);
                    CustomItem.AddCustomItem(player, CustomItem.RandomCustomItemWithType(ItemType.GunCrossvec));
                    CustomItem.AddCustomItem(player, CustomItem.RandomCustomItemWithType(ItemType.GunCOM18));
                    player.AddItem(ItemType.KeycardNTFOfficer);
                    player.AddItem(ItemType.Radio);
                    player.AddItem(ItemType.ArmorCombat);
                    player.AddItem(ItemType.Medkit);
                    player.AddItem(ItemType.GrenadeFlash);
                    break;
                case RoleTypeId.NtfSergeant:
                    player.AddAmmo(ItemType.Ammo9x19, 60);
                    player.AddAmmo(ItemType.Ammo556x45, 120);
                    CustomItem.AddCustomItem(player, CustomItem.RandomCustomItemWithType(ItemType.GunE11SR));
                    CustomItem.AddCustomItem(player, CustomItem.RandomCustomItemWithType(ItemType.GunCOM18));
                    player.AddItem(ItemType.KeycardNTFLieutenant);
                    player.AddItem(ItemType.Radio);
                    player.AddItem(ItemType.ArmorCombat);
                    CustomItem.AddCustomItem(player, "_PROTO_MEDKIT");
                    player.AddItem(ItemType.GrenadeHE);
                    break;
                case RoleTypeId.NtfSpecialist:
                    player.AddAmmo(ItemType.Ammo9x19, 60);
                    player.AddAmmo(ItemType.Ammo556x45, 120);
                    CustomItem.AddCustomItem(player, CustomItem.RandomCustomItemWithType(ItemType.GunE11SR));
                    CustomItem.AddCustomItem(player, CustomItem.RandomCustomItemWithType(ItemType.GunCOM18));
                    player.AddItem(ItemType.KeycardNTFLieutenant);
                    player.AddItem(ItemType.Radio);
                    player.AddItem(ItemType.ArmorCombat);
                    CustomItem.AddCustomItem(player, "_PROTO_MEDKIT");
                    player.AddItem(ItemType.GrenadeHE);
                    break;
                case RoleTypeId.NtfCaptain:
                    player.AddAmmo(ItemType.Ammo12gauge, 36);
                    player.AddAmmo(ItemType.Ammo762x39, 60);
                    CustomItem.AddCustomItem(player, CustomItem.RandomCustomItemWithType(ItemType.GunAK));
                    CustomItem.AddCustomItem(player, CustomItem.RandomCustomItemWithType(ItemType.GunShotgun));
                    player.AddItem(ItemType.KeycardNTFCommander);
                    player.AddItem(ItemType.Radio);
                    player.AddItem(ItemType.ArmorHeavy);
                    CustomItem.AddCustomItem(player, "_PROTO_MEDKIT");
                    player.AddItem(ItemType.Adrenaline);
                    player.AddItem(ItemType.GrenadeHE);
                    break;
                case RoleTypeId.FacilityGuard:
                    player.AddAmmo(ItemType.Ammo9x19, 100);
                    CustomItem.AddCustomItem(player, CustomItem.RandomCustomItemWithType(ItemType.GunFSP9));
                    CustomItem.AddCustomItem(player, CustomItem.RandomCustomItemWithType(ItemType.GunCOM18));
                    player.AddItem(ItemType.KeycardGuard);
                    player.AddItem(ItemType.Radio);
                    player.AddItem(ItemType.ArmorLight);
                    player.AddItem(ItemType.Medkit);
                    player.AddItem(ItemType.GrenadeFlash);
                    break;
                case RoleTypeId.ChaosConscript:
                    player.AddAmmo(ItemType.Ammo556x45, 120);
                    player.AddAmmo(ItemType.Ammo9x19, 60);
                    CustomItem.AddCustomItem(player, CustomItem.RandomCustomItemWithType(ItemType.GunE11SR));
                    CustomItem.AddCustomItem(player, CustomItem.RandomCustomItemWithType(ItemType.GunCOM15));
                    player.AddItem(ItemType.KeycardChaosInsurgency);
                    player.AddItem(ItemType.ArmorCombat);
                    CustomItem.AddCustomItem(player, "_PROTO_MEDKIT");
                    player.AddItem(ItemType.GrenadeFlash);
                    break;
                case RoleTypeId.ChaosRifleman:
                    player.AddAmmo(ItemType.Ammo556x45, 120);
                    player.AddAmmo(ItemType.Ammo9x19, 50);
                    CustomItem.AddCustomItem(player, CustomItem.RandomCustomItemWithType(ItemType.GunE11SR));
                    CustomItem.AddCustomItem(player, CustomItem.RandomCustomItemWithType(ItemType.GunCOM15));
                    player.AddItem(ItemType.KeycardChaosInsurgency);
                    player.AddItem(ItemType.ArmorCombat);
                    player.AddItem(ItemType.Medkit);
                    player.AddItem(ItemType.GrenadeFlash);
                    break;
                case RoleTypeId.ChaosRepressor:
                    player.AddAmmo(ItemType.Ammo762x39, 300);
                    player.AddAmmo(ItemType.Ammo44cal, 36);
                    CustomItem.AddCustomItem(player, CustomItem.RandomCustomItemWithType(ItemType.GunLogicer));
                    CustomItem.AddCustomItem(player, CustomItem.RandomCustomItemWithType(ItemType.GunRevolver));
                    player.AddItem(ItemType.KeycardChaosInsurgency);
                    player.AddItem(ItemType.ArmorCombat);
                    CustomItem.AddCustomItem(player, "_PROTO_MEDKIT");
                    player.AddItem(ItemType.GrenadeHE);
                    break;
                case RoleTypeId.ChaosMarauder:
                    player.AddAmmo(ItemType.Ammo762x39, 30);
                    player.AddAmmo(ItemType.Ammo12gauge, 26);
                    CustomItem.AddCustomItem(player, CustomItem.RandomCustomItemWithType(ItemType.GunAK));
                    CustomItem.AddCustomItem(player, CustomItem.RandomCustomItemWithType(ItemType.GunShotgun));
                    player.AddItem(ItemType.KeycardChaosInsurgency);
                    player.AddItem(ItemType.ArmorHeavy);
                    CustomItem.AddCustomItem(player, "_PROTO_MEDKIT");
                    player.AddItem(ItemType.Adrenaline);
                    player.AddItem(ItemType.GrenadeHE);
                    break;
            }
        }
    }
}
