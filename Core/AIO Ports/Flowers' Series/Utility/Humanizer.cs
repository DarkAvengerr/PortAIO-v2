namespace Flowers_ADC_Series.Utility
{
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;

    internal class Humanizer : Program
    {
        private static int randomTime, allTime;
        private static bool Enabled;
        private static Random random;
        private new static readonly Menu Menu = Utilitymenu;

        internal static void Init()
        {
            var HumanizerMenu = Menu.AddSubMenu(new Menu("Humanizer", "Humanizer"));
            {
                HumanizerMenu.AddItem(new MenuItem("EnableHumanizer", "Enabled", true).SetValue(false));
                HumanizerMenu.AddItem(
                    new MenuItem("AttackSpeed", "When Player AttackSpeed >= x(x/100)", true).SetValue(
                        new Slider(180, 150, 250)));
                HumanizerMenu.AddItem(
                    new MenuItem("MinRandomTime", "Min Random Move Time (x*100)", true).SetValue(
                        new Slider(1, 1, 10)));
                HumanizerMenu.AddItem(
                    new MenuItem("MaxRandomTime", "Max Random Move Time (x*100)", true).SetValue(
                        new Slider(2, 1, 10)));
            }
            allTime = Utils.TickCount;

            random = new Random();

            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (!Menu.Item("EnableHumanizer", true).GetValue<bool>())
            {
                return;
            }

            if (Me.ChampionName == "Jhin")
            {
                Orbwalker.SetMovement(Me.Spellbook.GetSpell(SpellSlot.R).Name != "JhinRShot");
                return;
            }

            if (Me.ChampionName == "Draven" || Me.ChampionName == "Kalista")
            {
                Orbwalker.SetMovement(true);
                return;
            }

            var RealAttackSpeed = 1/Me.AttackDelay;
            var LimitSpeed = (float)Menu.Item("AttackSpeed", true).GetValue<Slider>().Value/100;
            var minRandomTime = Menu.Item("MinRandomTime", true).GetValue<Slider>().Value;
            var maxRandomTime = Menu.Item("MaxRandomTime", true).GetValue<Slider>().Value;

            Enabled = RealAttackSpeed >= LimitSpeed;
            randomTime = random.Next(minRandomTime, maxRandomTime)*100;

            if (Move() && Enabled)
            {
                LeagueSharp.Common.Utility.DelayAction.Add(1000,() => allTime = Utils.TickCount + randomTime);
            }

            Orbwalker.SetMovement(Move());
        }

        private static bool Move()
        {
            if (!Enabled)
            {
                return true;
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    if (Me.CountEnemiesInRange(Orbwalking.GetRealAutoAttackRange(Me)) == 0)
                    {
                        return true;
                    }

                    return Utils.TickCount - allTime > 0;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    var haveMinions = MinionManager.GetMinions(Me.Position, Orbwalking.GetRealAutoAttackRange(Me)).Any();
                    var haveMobs =
                        MinionManager.GetMinions(Me.Position, Orbwalking.GetRealAutoAttackRange(Me), MinionTypes.All,
                            MinionTeam.Neutral, MinionOrderTypes.MaxHealth).Any();
                    var haveTurret =
                        ObjectManager.Get<Obj_AI_Turret>()
                            .Any(x => !x.IsDead && x.IsEnemy && x.Distance(Me) <= Orbwalking.GetRealAutoAttackRange(Me));

                    var haveLamp =
                        ObjectManager.Get<Obj_LampBulb>()
                            .Any(
                                x =>
                                    !x.IsDead && x.IsEnemy &&
                                    x.Position.Distance(Me.Position) <= Orbwalking.GetRealAutoAttackRange(Me));

                    if (!haveMinions && !haveMobs && !haveTurret && !haveLamp)
                    {
                        return true;
                    }

                    return Utils.TickCount - allTime > 0;
            }

            return true;
        }
    }
}