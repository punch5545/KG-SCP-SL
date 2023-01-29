using PluginAPI.Core.Factories;
using PluginAPI.Core.Interfaces;
using System;

namespace KG_SCP_SL.Factory
{
    /// <summary>
    /// A factory for <see cref="KGPlayer"/>s.
    /// </summary>
    public class KGPlayerFactory : PlayerFactory
    {
        public override Type BaseType { get; } = typeof(KGPlayer);
        public override IPlayer Create(IGameComponent component) => new KGPlayer(component);
    }
}
