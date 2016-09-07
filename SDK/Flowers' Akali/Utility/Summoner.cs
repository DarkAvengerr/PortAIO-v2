using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Flowers_Akali.Utility
{
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using System;

    internal class Summoner
    {
        private static AIHeroClient Me => Program.Me;
        private static Menu Menu => Tools.Menu;

        private static SpellSlot Ignite = SpellSlot.Unknown;
        private static SpellSlot Exhaust = SpellSlot.Unknown;
        private static SpellSlot Heal = SpellSlot.Unknown;

        private static float IgniteRange = 600f;

        internal static void Inject()
        {
            Ignite = Me.GetSpellSlot("SummonerDot");
            //Exhaust = Me.GetSpellSlot("SummonerExhaust");
            //Heal = Me.GetSpellSlot("SummonerHeal");


            var SummonerMenu = Menu.Add(new Menu("Summoner", "Summoner"));
            {
                if (Ignite != SpellSlot.Unknown)
                {
                    var IgniteMenu = SummonerMenu.Add(new Menu("Ignite", "Ignite"));
                    {
                        IgniteMenu.Add(new MenuBool("Combo", "Use In Combo", true));
                        IgniteMenu.Add(new MenuBool("KillSteal", "Use In KillSteal", true));
                    }
                }
            }

            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Ignite != SpellSlot.Unknown && Ignite.IsReady())
            {
                IgniteLogic();
            }
        }

        private static void IgniteLogic()
        {
            var target = Common.Manager.GetTarget(IgniteRange, DamageType.True);

            if(Common.Manager.CheckTarget(target))
            {
                if (Common.Manager.InCombo && Menu["Summoner"]["Ignite"]["Combo"] && target.IsValidTarget(IgniteRange) && target.HealthPercent < 20)
                {
                    Me.Spellbook.CastSpell(Ignite, target);
                    return;
                }

                if (GetIgniteDamage(target) > target.Health && target.IsValidTarget(IgniteRange))
                {
                    Me.Spellbook.CastSpell(Ignite, target);
                    return;
                }
            }
        }

        private static double GetIgniteDamage(AIHeroClient target)
        {
            return 50 + 20 * Me.Level - (target.HPRegenRate / 5 * 3);
        }
    }
}