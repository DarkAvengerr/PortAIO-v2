using EloBuddy; 
using LeagueSharp.Common; 
namespace ElLeeSin.Utilities
{
    using System;
    using System.Linq;

    using ElLeeSin.Components;
    using ElLeeSin.Components.SpellManagers;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using static LeeSin;

    using ItemData = LeagueSharp.Common.Data.ItemData;

    internal static class Misc
    {
        #region Properties

        /// <summary>
        ///     Gets Ravenous Hydra
        /// </summary>
        /// <value>
        ///     Ravenous Hydra
        /// </value>
        internal static Items.Item Hydra => ItemData.Ravenous_Hydra_Melee_Only.GetItem();

        /// <summary>
        ///     Gets the E Instance name
        /// </summary>
        /// <value>
        ///     E instance name
        /// </value>
        internal static bool IsEOne
            => spells[Spells.E].Instance.Name.Equals("BlindMonkEOne", StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        ///     Gets the Q Instance name
        /// </summary>
        /// <value>
        ///     Q instance name
        /// </value>
        internal static bool IsQOne
            => spells[Spells.Q].Instance.Name.Equals("BlindMonkQOne", StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        ///     Gets the W Instance name
        /// </summary>
        /// <value>
        ///     W instance name
        /// </value>
        internal static bool IsWOne
            => spells[Spells.W].Instance.Name.Equals("BlindMonkWOne", StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        ///     Gets Tiamat Item
        /// </summary>
        /// <value>
        ///     Tiamat Item
        /// </value>
        internal static Items.Item Tiamat => ItemData.Tiamat_Melee_Only.GetItem();

        /// <summary>
        ///     Gets Titanic Hydra
        /// </summary>
        /// <value>
        ///     Titanic Hydra
        /// </value>
        internal static Items.Item Titanic => ItemData.Titanic_Hydra_Melee_Only.GetItem();

        /// <summary>
        ///     The ward flash range.
        /// </summary>
        internal static float WardFlashRange => InsecManager.WardRange + spells[Spells.R].Range - 100;

        /// <summary>
        ///     Gets the W Instance name
        /// </summary>
        /// QState
        /// <value>
        ///     W instance name
        /// </value>
        internal static Wardmanager.WCastStage WStage
        {
            get
            {
                if (!spells[Spells.W].IsReady())
                {
                    return Wardmanager.WCastStage.Cooldown;
                }

                return ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W)
                           .Name.Equals("blindmonkwtwo", StringComparison.InvariantCultureIgnoreCase)
                           ? Wardmanager.WCastStage.Second
                           : Wardmanager.WCastStage.First;
            }
        }

        /// <summary>
        ///     Gets Youmuus Ghostblade
        /// </summary>
        /// <value>
        ///     Youmuus Ghostblade
        /// </value>
        internal static Items.Item Youmuu => ItemData.Youmuus_Ghostblade.GetItem();

        #endregion

        #region Methods

        /// <summary>
        ///     Compares gameobjects.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="@object"></param>
        /// <returns></returns>
        internal static bool Compare(this GameObject gameObject, GameObject @object)
        {
            return (gameObject != null) && gameObject.IsValid && (@object != null) && @object.IsValid
                   && (gameObject.NetworkId == @object.NetworkId);
        }

        /// <summary>
        ///     Orbwalker.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="target"></param>
        public static void Orbwalk(Vector3 pos, AIHeroClient target = null)
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, pos);
        }

        /// <summary>
        ///     Gets the Q2 damage
        /// </summary>
        /// <param name="target"></param>
        /// <param name="subHP"></param>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static float Q2Damage(Obj_AI_Base target, float subHP = 0, bool monster = false)
        {
            var dmg = new[] { 50, 80, 110, 140, 170 }[spells[Spells.Q].Level - 1]
                      + 0.9 * ObjectManager.Player.FlatPhysicalDamageMod
                      + 0.08 * (target.MaxHealth - (target.Health - subHP));

            return
                (float)
                (ObjectManager.Player.CalcDamage(
                     target,
                     Damage.DamageType.Physical,
                     target is Obj_AI_Minion ? Math.Min(dmg, 400) : dmg) + subHP);
        }

        /// <summary>
        ///     Gets the distance to player.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        internal static float DistanceToPlayer(this Obj_AI_Base source)
        {
            return ObjectManager.Player.Distance(source);
        }

        /// <summary>
        ///     Gets the menu item.
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public static bool GetMenuItem(string paramName)
        {
            return MyMenu.Menu.Item(paramName).IsActive();
        }

        /// <summary>
        ///     Returns if target has any buffs.
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        internal static bool HasAnyBuffs(this Obj_AI_Base unit, string s)
        {
            return
                unit.Buffs.Any(
                    a => a.Name.ToLower().Contains(s.ToLower()) || a.DisplayName.ToLower().Contains(s.ToLower()));
        }

        /// <summary>
        ///     Checks if a target has the BlindMonkTempest buff.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        internal static bool HasBlindMonkTempest(Obj_AI_Base target) => target.HasBuff("BlindMonkTempest");

        /// <summary>
        ///     Returns if target has Q buff.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        internal static bool HasQBuff(this Obj_AI_Base unit)
        {
            return unit.HasAnyBuffs("BlindMonkQOne") || unit.HasAnyBuffs("blindmonkqonechaos")
                   || unit.HasAnyBuffs("BlindMonkSonicWave");
        }

        /// <summary>
        ///     Returns the Q buff
        /// </summary>
        /// <returns></returns>
        internal static Obj_AI_Base ReturnQBuff()
        {
            try
            {
                return
                    ObjectManager.Get<Obj_AI_Base>()
                        .Where(a => a.IsValidTarget(1300))
                        .FirstOrDefault(unit => unit.HasQBuff());
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
            return null;
        }

        #endregion
    }
}