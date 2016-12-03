using System;
using System.Linq;
using Activator.Base;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Activator.Summoners
{
    internal class exhuast : CoreSum
    {
        internal override string Name => "summonerexhaust";
        internal override string DisplayName => "Exhaust";
        internal override string[] ExtraNames => new[] { "" };
        internal override float Range => 650f;
        internal override int Duration => 100;
        internal override int Priority => 5;

        public override void AttachMenu(Menu menu)
        {
            Activator.UseEnemyMenu = true;
            menu.AddItem(new MenuItem("a" + Name + "pct", "Exhaust on ally HP (%)")).SetValue(new Slider(35));
            menu.AddItem(new MenuItem("e" + Name + "pct", "Exhaust on enemy HP (%)")).SetValue(new Slider(45));
            menu.AddItem(new MenuItem("use" + Name + "ulti", "Use on Dangerous (Utimates Only)")).SetValue(true).SetTooltip("Or Spells with \"Force Exhaust\"");
            menu.AddItem(new MenuItem("f" + Name + "pct", "-> Only if HP (%) <= ")).SetValue(new Slider(90));
            menu.AddItem(new MenuItem("mode" + Name, "Mode: ")).SetValue(new StringList(new[] { "Always", "Combo" }));
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            var hid =
                Activator.Heroes.Where(x => x.Player.IsValidTarget(Range + 250))
                    .OrderByDescending(x => x.Player.TotalAttackDamage)
                    .FirstOrDefault();

            foreach (var hero in Activator.Allies())
            {
                var attacker = hero.Attacker as AIHeroClient;
                if (attacker == null || hid == null)
                    continue;

                if (hero.Player.Distance(Player.ServerPosition) > 1000)
                    continue;

                if (attacker.Distance(hero.Player) > 1000 ||
                    attacker.Distance(Player) > Range)
                    continue;

                if (hero.HitTypes.Contains(HitType.ForceExhaust))
                {
                    UseSpellOn(attacker);
                    break;
                }

                if (!Parent.Item(Parent.Name + "useon" + attacker.NetworkId).GetValue<bool>())
                    continue;

                if (Helpers.GetRole(attacker) == PrimaryRole.Support)
                    continue;

                if (Menu.Item("use" + Name + "ulti").GetValue<bool>())
                {
                    if (hero.HitTypes.Contains(HitType.Ultimate))
                    {
                        if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                            Menu.Item("f" + Name + "pct").GetValue<Slider>().Value)
                        {
                            UseSpellOn(attacker, Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
                        }
                    }
                }

                if (hero.Player.Health / hero.Player.MaxHealth * 100 <= Menu.Item("a" + Name + "pct").GetValue<Slider>().Value)
                {
                    if (hero.IncomeDamage > 0 || !hero.Player.IsFacing(attacker))
                    {
                        if (attacker.NetworkId == hid.Player.NetworkId)
                        {
                            UseSpellOn(attacker, Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
                        }
                    }
                }

                if (attacker.Health / attacker.MaxHealth * 100 <= Menu.Item("e" + Name + "pct").GetValue<Slider>().Value)
                {
                    if (hero.IncomeDamage > 0 || !attacker.IsFacing(hero.Player))
                    {
                        UseSpellOn(attacker, Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
                    }
                }
            }
        }
    }
}
