using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Ashe.Logic
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    #endregion

    internal class ELogic
    {
        #region Fields

        public readonly Dictionary<string, Vector3> Camp = new Dictionary<string, Vector3> {
                                                                   {
                                                                       "mid_Dragon",
                                                                       new Vector3(
                                                                       9122f,
                                                                       4058f,
                                                                       53.95995f)
                                                                   },
                                                                   {
                                                                       "left_dragon",
                                                                       new Vector3(
                                                                       9088f,
                                                                       4544f,
                                                                       52.24316f)
                                                                   },
                                                                   {
                                                                       "baron",
                                                                       new Vector3(
                                                                       5774f,
                                                                       10706f,
                                                                       55.77578F)
                                                                   },
                                                                   {
                                                                       "red_wolves",
                                                                       new Vector3(
                                                                       11772f,
                                                                       8856f,
                                                                       50.30728f)
                                                                   }
                                                               };

        #endregion

        #region Public Methods and Operators

        public bool CanCastE()
        {
            var pos =
                Camp.FirstOrDefault(
                    x =>
                    x.Value.Distance(Variable.Player.Position) > 1750
                    && x.Value.Distance(Variable.Player.Position) < 6000);

            return pos.Value.IsValid();
        }

        public bool ComboE(AIHeroClient target)
        {
            return !target.IsVisible && !target.IsDead && target.Distance(Variable.Player) < 1500f;
        }

        public int GetEAmmo()
        {
            return ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Ammo;
        }

        #endregion
    }
}