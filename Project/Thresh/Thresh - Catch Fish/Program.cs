using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ThreshCatchFish
{
    internal class Program
    {
        public static Menu Menu;
        public static Spell Q, E;
        public static Orbwalking.Orbwalker Orbwalker;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
                {
            if (!ObjectManager.Player.IsChampion("Thresh"))
            {
                return;
            }
            Q = new Spell(SpellSlot.Q, 1100);
            Q.SetSkillshot(0.500f, 70, 1900f, true, SkillshotType.SkillshotLine);
            E = new Spell(SpellSlot.E, 400);

            Menu = new Menu("CatchFishThresh", "CatchFishThresh", true);
            var orbwalker = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(orbwalker);
            Menu.AddItem(new MenuItem("CastQ", "Cast Q").SetValue(true));
            Menu.AddItem(new MenuItem("FarmQ", "LaneClear Q").SetValue(true));
            Menu.AddItem(new MenuItem("FarmE", "LaneClear E").SetValue(true));
            Menu.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                case Orbwalking.OrbwalkingMode.Mixed:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                {
                    Farm();
                }
                    break;
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (target.IsValidTarget() && Menu.Item("CastQ").IsActive() && Q.IsReady())
            {
                Q.Cast(target).IsCasted();
            }
        }

        private static void Farm()
        {
            var minions = MinionManager.GetMinions(Q.Range);
            var eMinions = minions.Where(m => m.IsValidTarget(E.Range)).ToList();
            var killableMinion = minions.FirstOrDefault(m => Q.IsKillable(m));

            if (killableMinion != null && Menu.Item("FarmQ").IsActive() && Q.IsReady())
            {
                Q.Cast(killableMinion);
            }

            if (Menu.Item("FarmE").IsActive() && E.IsReady() && eMinions.Count > 2)
            {
                E.Cast(eMinions.FirstOrDefault());
            }
        }
    }
}
