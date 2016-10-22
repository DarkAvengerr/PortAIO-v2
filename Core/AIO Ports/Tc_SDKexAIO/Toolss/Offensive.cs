using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Tc_SDKexAIO.Toolss
{

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    using System;
    using System.Linq;

    using Common;
    using Config;

    internal class Offensive
    {
        private static AIHeroClient Player => PlaySharp.Player;

        private static Menu Menu => Tools.Menu;

        private static Items.Item BotRuinedKing, Tiamat, Hydra, Titanic;

        internal static void Init()
        {
            var OffMenu = Menu.Add(new Menu("Offensive", "Offensive"));
            {
                OffMenu.GetSeparator("Youmuus Mode");
                OffMenu.Add(new MenuBool("Youmuus", "Use Youmuu", true));
                OffMenu.GetSlider("Youmuus.s", "Youmuus enemy  HP Min >=", 70, 0, 100);
                OffMenu.GetSeparator("Cutlass Mode");
                OffMenu.Add(new MenuBool("Cutlass", "Use Cutlass", true));
                OffMenu.GetSlider("Cutlass.s", "Cutlass enemy HP Min >=", 70, 0, 100);
                OffMenu.GetSeparator("Botrk Mode");
                OffMenu.Add(new MenuBool("Botrk", "Use Botrk", true));
                OffMenu.GetSlider("Botrk.s", "Botrk enemy HP Min >=", 70, 0, 100);
                OffMenu.GetSeparator("BotRuinedKing Mode");
                OffMenu.Add(new MenuBool("BotRuined", "Use BotRuinedKing", true));
                OffMenu.GetSlider("BotRuined.s", "BotRuinedKing enemy HP Min >=", 70, 0, 100);
                OffMenu.GetSeparator("Combo Mode");
                OffMenu.Add(new MenuBool("Combo", "Combo Use", true));
                OffMenu.Add(new MenuBool("Hydra", "Hydra Use", true));
                OffMenu.Add(new MenuBool("Tiamat", "Tiamat Use", true));
                OffMenu.Add(new MenuBool("Titanic", "Titanic Use", true));

            }
            Game.OnUpdate += OnUpdate;

            BotRuinedKing = new Items.Item(ItemId.Blade_of_the_Ruined_King, 550);
            Tiamat = new Items.Item(ItemId.Tiamat_Melee_Only, 400);
            Hydra = new Items.Item(ItemId.Ravenous_Hydra_Melee_Only, 400);
            Titanic = new Items.Item(3748, 0);
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            if (Manager.Combo && Menu["Offensive"]["Combo"])
            {
                var t = Variables.TargetSelector.GetTarget(600, DamageType.Physical);

                if (t != null && t.IsHPBarRendered)
                {
                    if (Menu["Offensive"]["Youmuus"] && Items.HasItem(3142)
                        && t.HealthPercent >= Menu["Offensive"]["Youmuus.s"].GetValue<MenuSlider>().Value
                        && t.IsValidTarget(Player.GetRealAutoAttackRange() + 150))
                    {
                        Items.UseItem(3142, t);
                    }

                    if (Menu["Offensive"]["Cutlass"] && Items.HasItem(3144)
                        && t.HealthPercent >= Menu["Offensive"]["Cutlass.s"].GetValue<MenuSlider>().Value
                        && t.IsValidTarget(Player.GetRealAutoAttackRange()))
                    {
                        Items.UseItem(3144, t);
                    }

                    if (Menu["Offensive"]["Botrk"] && Items.HasItem(3153)
                        && (t.HealthPercent >= Menu["Offensive"]["Botrk.s"].GetValue<MenuSlider>().Value
                        && Player.HealthPercent < 70) && t.IsValidTarget(Player.GetRealAutoAttackRange()))
                    {
                        Items.UseItem(3153, t);
                    }

                    if (Menu["Offensive"]["Hydra"])
                    {
                        UseItem(t);
                    }

                    if (Menu["Offensive"]["Tiamat"])
                    {
                        UseItem(t);
                    }
                    if (Menu["Offensive"]["Titanic"])
                    {
                        UseItem(t);
                    }
                    if (Menu["Offensive"]["BotRuined"] && (t.HealthPercent >= Menu["Offensive"]["BotRuined.s"].GetValue<MenuSlider>().Value
                        && Player.HealthPercent < 60) && t.IsValidTarget(Player.GetRealAutoAttackRange()))
                    {
                        UseItem(t);
                    }
                }
            }
        }

        private static void UseItem(AIHeroClient target)
        {
            if (target != null)
            {
                if (BotRuinedKing.IsReady)
                {
                    BotRuinedKing.Cast(target);
                }
            }

            if (Tiamat.IsReady && Player.CountEnemyHeroesInRange(Tiamat.Range) > 0)
            {
                Tiamat.Cast();
            }

            if (Hydra.IsReady && Player.CountEnemyHeroesInRange(Hydra.Range) > 0)
            {
                Hydra.Cast();
            }

            if (Titanic.IsReady && !Player.Spellbook.IsAutoAttacking && Variables.Orbwalker.GetTarget() != null)
            {
                Titanic.Cast();
            }
        }
    }
}