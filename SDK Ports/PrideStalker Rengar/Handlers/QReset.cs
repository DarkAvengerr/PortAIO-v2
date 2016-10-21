using System.Linq;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.Utils;
using PrideStalker_Rengar.Main;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace PrideStalker_Rengar.Handlers
{
    internal class QReset : Core
    {
        public static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || !Spells.Q.IsReady()) return;

            if (Orbwalker.ActiveMode == OrbwalkingMode.Combo)
            {
                var a = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Player.GetRealAutoAttackRange(Player) + 150));

                var targets = a as AIHeroClient[] ?? a.ToArray();

                foreach (var target in targets)
                {
                    if (MenuConfig.ComboMode.SelectedValue != "Ap Combo")
                    {
                        Spells.Q.Cast(target);
                    }
                    else
                    {
                        if (Player.Mana >= 5) return;

                        Spells.Q.Cast(target);
                    }
                }
              
            }

            if (Orbwalker.ActiveMode != OrbwalkingMode.LaneClear) return;

            var turret = args.Target as Obj_AI_Turret;

            if (turret != null)
            {
                Spells.Q.Cast(turret);
            }

            if (Player.Mana == 5 && MenuConfig.Passive.Active)
            {
                return;
            }

            var mobs = GameObjects.Jungle.Where(x => x.IsValidTarget(Player.GetRealAutoAttackRange(Player)));

            foreach (var m in mobs)
            {
                if (m.Health < Player.GetAutoAttackDamage(m)) return;

                Spells.Q.Cast(m);
            }
        }
    }
}
