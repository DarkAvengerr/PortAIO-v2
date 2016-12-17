using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using SharpDX;
using EloBuddy; 
using LeagueSharp.Common; 
namespace ElLeeSin.Components.Spells
{
    using ElLeeSin.Components.SpellManagers;
    using ElLeeSin.Utilities;

    internal class SpellE
    {
        public static void CastE()
        {
            if (ObjectManager.Player.IsDashing())
            {
                return;
            }

            if (Misc.IsEOne)
            {
                var enemiesCount =
                    HeroManager.Enemies.Where(
                        h =>
                            h.IsValid && !h.IsDead && h.IsVisible
                            && h.Distance(ObjectManager.Player) < LeeSin.spells[LeeSin.Spells.E].Range - 50).ToList();

                if (enemiesCount.Count == 0)
                {
                    return;
                }

                if (LeeSin.PassiveStacks == 0 && ObjectManager.Player.Mana >= 75 || enemiesCount.Count >= 2
                    || enemiesCount.Any(t => t.Distance(ObjectManager.Player) > Orbwalking.GetRealAutoAttackRange(ObjectManager.Player)))
                {
                    LeeSin.spells[LeeSin.Spells.E].Cast();
                    LeeSin.lastSpellCastTime = Environment.TickCount;
                }
            }
            else
            {
                var enemiesCount =
                    HeroManager.Enemies.Where(
                        h =>
                            h.IsValid && !h.IsDead && h.IsVisible && Misc.HasBlindMonkTempest(h)
                            && h.Distance(ObjectManager.Player) < 500f).ToList();

                if (Environment.TickCount - LeeSin.lastSpellCastTime <= 500)
                {
                    return;
                }

                if (enemiesCount.Count == 0)
                {
                    return;
                }

                if ((LeeSin.PassiveStacks == 0 || enemiesCount.Count >= 2)
                    || enemiesCount.Any(t => t.Distance(ObjectManager.Player) > Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 60))
                {
                    LeeSin.spells[LeeSin.Spells.E].Cast();
                }
            }
        }
    }
}
