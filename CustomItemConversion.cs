using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Pickups;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using Scp914;
using UnityEngine;

namespace SwiftKraft
{
    public class CustomItemConversion
    {
        public static bool IsOn;

        [PluginEvent(ServerEventType.PlayerChangeItem)]
        public void OnPlayerChangedItem(Player player, ushort prevItemSerial, ushort currItemSerial)
        {
            if (!IsOn || player.CurrentItem == null || Plugin.customItems.ContainsKey(currItemSerial))
                return;

            string temp = CustomItem.RandomCustomItemWithType(player.CurrentItem.ItemTypeId);

            if (!string.IsNullOrEmpty(temp))
                Plugin.customItems.Add(player.CurrentItem.ItemSerial, temp);
        }

        [PluginEvent(ServerEventType.PlayerAimWeapon)]
        public void OnPlayerAimWeapon(Player player, Firearm firearm, bool aiming)
        {
            if (!IsOn || player.CurrentItem == null || Plugin.customItems.ContainsKey(firearm.ItemSerial))
                return;

            string temp = CustomItem.RandomCustomItemWithType(player.CurrentItem.ItemTypeId);

            if (!string.IsNullOrEmpty(temp))
                Plugin.customItems.Add(player.CurrentItem.ItemSerial, temp);
        }

        [PluginEvent(ServerEventType.PlayerSearchedPickup)]
        public void OnPlayerSearchedPickup(Player player, ItemPickupBase itemPickupBase)
        {
            if (!IsOn || Plugin.customItems.ContainsKey(itemPickupBase.Info.Serial))
                return;

            string temp = CustomItem.RandomCustomItemWithType(itemPickupBase.Info.ItemId);

            if (!string.IsNullOrEmpty(temp))
                Plugin.customItems.Add(itemPickupBase.Info.Serial, temp);
        }

        [PluginEvent(ServerEventType.Scp914UpgradePickup)]
        public void OnUpgradePickup(ItemPickupBase itemPickupBase, Vector3 position, Scp914KnobSetting scp914Knob)
        {
            if (!Plugin.customItems.ContainsKey(itemPickupBase.Info.Serial))
                return;

            Plugin.customItems.Remove(itemPickupBase.Info.Serial);

            if (!IsOn)
            {
                string temp = CustomItem.RandomCustomItemWithType(itemPickupBase.Info.ItemId);

                if (!string.IsNullOrEmpty(temp))
                    Plugin.customItems.Add(itemPickupBase.Info.Serial, temp);
            }
        }

        [PluginEvent(ServerEventType.Scp914UpgradeInventory)]
        public void OnUpgradePickup(Player player, ItemBase itemBase, Scp914KnobSetting scp914Knob)
        {
            if (!Plugin.customItems.ContainsKey(itemBase.ItemSerial))
                return;

            Plugin.customItems.Remove(itemBase.ItemSerial);

            if (!IsOn)
            {
                string temp = CustomItem.RandomCustomItemWithType(itemBase.ItemTypeId);

                if (!string.IsNullOrEmpty(temp))
                    Plugin.customItems.Add(itemBase.ItemSerial, temp);
            }
        }
    }
}
