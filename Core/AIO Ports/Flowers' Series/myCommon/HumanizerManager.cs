using EloBuddy; 
using LeagueSharp.Common; 
namespace ADCCOMMON
{
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;

    public static class HumanizerManager
    {
        private static int randomTime, allTime;
        private static bool Enabled;
        private static Random random;
        private static Menu humanizerMenu;

        public static void AddToMenu(Menu mainMenu)
        {
            humanizerMenu = mainMenu;

            humanizerMenu.AddItem(new MenuItem("EnableHumanizer", "Enabled", true).SetValue(false));
            humanizerMenu.AddItem(
                new MenuItem("AttackSpeed", "When Player AttackSpeed >= x(x/100)", true).SetValue(
                    new Slider(180, 150, 250)));
            humanizerMenu.AddItem(
                new MenuItem("MinRandomTime", "Min Random Move Time (x*100)", true).SetValue(
                    new Slider(1, 1, 10)));
            humanizerMenu.AddItem(
                new MenuItem("MaxRandomTime", "Max Random Move Time (x*100)", true).SetValue(
                    new Slider(2, 1, 10)));

            allTime = Utils.TickCount;

            random = new Random();

            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (!humanizerMenu.Item("EnableHumanizer", true).GetValue<bool>())
            {
                Orbwalking.Move = true;
                return;
            }

            if (ObjectManager.Player.ChampionName == "Jhin")
            {
                Orbwalking.Move = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name != "JhinRShot";
                return;
            }

            if (ObjectManager.Player.ChampionName == "Draven" || ObjectManager.Player.ChampionName == "Kalista")
            {
                Orbwalking.Move = true;
                return;
            }

            var RealAttackSpeed = 1 / ObjectManager.Player.AttackDelay;
            var LimitSpeed = (float)humanizerMenu.Item("AttackSpeed", true).GetValue<Slider>().Value / 100;
            var minRandomTime = humanizerMenu.Item("MinRandomTime", true).GetValue<Slider>().Value;
            var maxRandomTime = humanizerMenu.Item("MaxRandomTime", true).GetValue<Slider>().Value;

            Enabled = RealAttackSpeed >= LimitSpeed;
            randomTime = random.Next(minRandomTime, maxRandomTime) * 100;

            if (Move() && Enabled)
            {
                LeagueSharp.Common.Utility.DelayAction.Add(1000, () => allTime = Utils.TickCount + randomTime);
            }

            Orbwalking.Move = Move();
        }

        private static bool Move()
        {
            if (!Enabled)
            {
                return true;
            }

            if (Orbwalking.isCombo)
            {
                if (ObjectManager.Player.CountEnemiesInRange(Orbwalking.GetRealAutoAttackRange(ObjectManager.Player)) == 0)
                {
                    return true;
                }

                return Utils.TickCount - allTime > 0;
            }

            if (Orbwalking.isLaneClear)
            {
                var haveMinions =
                    MinionManager.GetMinions(ObjectManager.Player.Position,
                        Orbwalking.GetRealAutoAttackRange(ObjectManager.Player)).Any();
                var haveMobs =
                    MinionManager.GetMinions(ObjectManager.Player.Position,
                        Orbwalking.GetRealAutoAttackRange(ObjectManager.Player), MinionTypes.All,
                        MinionTeam.Neutral, MinionOrderTypes.MaxHealth).Any();
                var haveTurret =
                    ObjectManager.Get<Obj_AI_Turret>()
                        .Any(
                            x =>
                                !x.IsDead && x.IsEnemy &&
                                x.Distance(ObjectManager.Player) <=
                                Orbwalking.GetRealAutoAttackRange(ObjectManager.Player));
                var haveLamp =
                    ObjectManager.Get<Obj_LampBulb>()
                        .Any(
                            x =>
                                !x.IsDead && x.IsEnemy &&
                                x.Position.Distance(ObjectManager.Player.Position) <=
                                Orbwalking.GetRealAutoAttackRange(ObjectManager.Player));

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
