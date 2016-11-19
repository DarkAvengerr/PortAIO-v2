using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace ElRengarDecentralized.Utils
{
    using ElRengarDecentralized.Enumerations;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class BuffManager
    {
        /// <summary>
        ///     Gets or sets a value indicating whether the player has the passive buff.
        /// </summary>
        public static bool HasPassive => ObjectManager.Player.Buffs.Any(x => x.Name.ToLower().Contains("rengarpassivebuff"));

        /// <summary>
        ///     Gets or sets a value indicating whether player has the ultimate buff.
        /// </summary>
        public static bool HasUltimate => ObjectManager.Player.Buffs.Any(x => x.Name.ToLower().Contains("rengarr"));

        /// <summary>
        ///     Gets or sets what the spell works on.
        /// </summary>
        /// <value>
        ///     The buff types the spell works on.
        /// </value>
        public static List<BuffType> Buffs = new List<BuffType>()
        {
            BuffType.Charm, BuffType.Flee, BuffType.Slow, BuffType.Polymorph,
            BuffType.Silence, BuffType.Snare, BuffType.Stun, BuffType.Taunt, BuffType.Suppression
        };

        /// <summary>
        ///     Called when the game processes spell casts.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
        internal static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (args.SData.Name.Equals("rengarr", StringComparison.InvariantCultureIgnoreCase))
            {
                if (ItemManager.Youmuus.IsOwned() && ItemManager.Youmuus.IsValidAndReady())
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(3500, () => ItemManager.Youmuus.Cast());
                }
            }
        }
    }
}
