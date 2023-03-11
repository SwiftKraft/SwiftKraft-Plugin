using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;

namespace SwiftKraft
{
    public class SCPBuffs
    {
        [PluginEvent(ServerEventType.PlayerDeath)]
        public void OnPlayerDeath(Player victim, Player attacker, DamageHandlerBase damageHandlerBase)
        {
            if (!CustomItemConversion.IsOn || attacker == null || !attacker.IsSCP)
                return;

            attacker.Heal(100f);
        }
    }
}
