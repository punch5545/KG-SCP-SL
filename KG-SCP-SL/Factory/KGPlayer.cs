using PluginAPI.Core.Attributes;
using PluginAPI.Core.Interfaces;
using PluginAPI.Core;
using PluginAPI.Enums;
using PluginAPI.Events;
using RemoteAdmin;
using CommandSystem;

namespace KG_SCP_SL.Factory
{
    public class KGPlayer : Player
    {
        public KGPlayer(IGameComponent component) : base(component)
        {
            EventManager.RegisterEvents(Main.Singleton, this);
        }

        public string TestParam { get; set; }

        public string Test => "TestValue";

        [PluginEvent(ServerEventType.RemoteAdminCommand)]
        public void OnRaCommand(ICommandSender plr, string cmd, string[] args)
        {
            if (!(plr is PlayerCommandSender pcs) || pcs.ReferenceHub != ReferenceHub)
                return;

            // Log.Info($" [&4KGPlayer&r] Player {pcs.Nickname} executed command {cmd}");
        }

        public override void OnDestroy()
        {
            EventManager.UnregisterEvents(Main.Singleton, this);
        }
    }
}
