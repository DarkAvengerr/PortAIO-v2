using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hiki.Kled.Extensions;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Utilities = Hiki.Kled.Extensions.Utilities;
using EloBuddy;

namespace Hiki.Kled.Champions
{
    internal class Kled
    {
        public Kled()
        {
            KledOnLoad();
        }

        private static void KledOnLoad()
        {

            Spells.Initialize();
            Menus.Initialize();

            Game.OnUpdate += OnUpdate;

        }

        private static void OnUpdate(EventArgs args)
        {
            switch (Menus.Orbwalker.ActiveMode)
            {
                  case Orbwalking.OrbwalkingMode.Combo:
                    OnCombo();
                    break;
                    case Orbwalking.OrbwalkingMode.Mixed:
                    OnMixed();
                    break;
                    case Orbwalking.OrbwalkingMode.LaneClear:
                    OnJungle();
                    OnClear();
                    break;
            }

            if (Menus.Config.Item("manual.r").GetValue<KeyBind>().Active)
            {
                Orbwalking.MoveTo(Game.CursorPos);

                if (Spells.R.IsReady() && Utilities.Enabled("r.combo"))
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.R.Range)))
                    {
                        if (Utilities.Enabled("r." + enemy.ChampionName) &&
                            ObjectManager.Player.CountEnemiesInRange(Utilities.Slider("max.r.distance")) <= Utilities.Slider("min.enemy"))
                        {
                            Spells.R.Cast(enemy.Position);
                        }
                    }
                }
            }
        }

        public static void OnCombo()
        {
            if (Spells.SkaarlQ.IsReady() && Utilities.Enabled("skaarl.q.combo") && Utilities.IsSkaarl())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.SkaarlQ.Range)))
                {
                    Spells.Q.HikiCast(enemy, Utilities.HikiChance("hitchance"), "prediction", Menus.Config);
                }
            }

            if (Spells.Q.IsReady() && Utilities.Enabled("q.combo") && Utilities.IsKled())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.Q.Range)))
                {
                    Spells.Q.HikiCast(enemy, Utilities.HikiChance("hitchance"), "prediction", Menus.Config);
                }
            }

            if (Spells.E.IsReady() && Utilities.Enabled("e.combo") && Utilities.IsSkaarl())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.SkaarlQ.Range)))
                {
                    if (Menus.Config.Item("e.mode").GetValue<StringList>().SelectedIndex == 0)
                    {
                        Utilities.ECast(enemy);
                    }
                    else
                    {
                        Spells.E.Cast(Game.CursorPos);
                    }
                }
            }
        }

        private static void OnMixed()
        {
            if (Spells.SkaarlQ.IsReady() && Utilities.Enabled("skaarl.q.harass") && Utilities.IsSkaarl())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.SkaarlQ.Range)))
                {
                    Spells.Q.HikiCast(enemy, Utilities.HikiChance("hitchance"), "prediction", Menus.Config);
                }
            }

            if (Spells.Q.IsReady() && Utilities.Enabled("q.harass") && Utilities.IsKled())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.Q.Range)))
                {
                    Spells.Q.HikiCast(enemy, Utilities.HikiChance("hitchance"), "prediction", Menus.Config);
                }
            }
        }

        private static void OnJungle()
        {
            var mob = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (mob == null || mob.Count == 0)
            {
                return;
            }

            if (Spells.Q.IsReady() && Utilities.Enabled("q.jungle") && Utilities.IsKled())
            {
                Spells.Q.Cast(mob[0].Position);
            }

            if (Spells.SkaarlQ.IsReady() && Utilities.Enabled("q.skaarl.jungle") && Utilities.IsSkaarl())
            {
                Spells.SkaarlQ.Cast(mob[0].Position);
            }

            if (Spells.E.IsReady() && Utilities.Enabled("e.jungle") && Utilities.IsSkaarl())
            {
                Spells.E.Cast(mob[0].Position);
            }
        }

        private static void OnClear()
        {
            if (Spells.Q.IsReady() && Utilities.Enabled("q.clear") && Utilities.IsKled())
            {
                var minions = MinionManager.GetMinions(ObjectManager.Player.Position, Spells.Q.Range, MinionTypes.All,
                MinionTeam.NotAlly);

                var minioncount = Spells.Q.GetLineFarmLocation(minions);
                if (minions == null || minions.Count == 0)
                {
                    return;
                }

                if (minioncount.MinionsHit >= Utilities.Slider("min.count"))
                {
                    Spells.Q.Cast(minioncount.Position);
                }
            }
        }

    }
}
