using System;
using System.Linq;
using Activator.Base;
using Activator.Data;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Activator.Summoners
{
    internal class smite : CoreSum
    {
        internal override string Name => "summonersmite";
        internal override string DisplayName => "Smite";
        internal override float Range => 500f + 1f;
        internal override int Duration => 0;
        internal override int Priority => 7;

        internal override string[] ExtraNames => new[]
        {
            "s5_summonersmiteplayerganker", "s5_summonersmiteduel",
            "s5_summonersmitequick", "itemsmiteaoe"
        };

        public override void AttachMenu(Menu menu)
        {
            Activator.UseEnemyMenu = true;
            var smiteCombo = Smitedata.CachedSpellList.FirstOrDefault();

            menu.AddItem(new MenuItem("usesmite", "Use Smite")).SetValue(new KeyBind('M', KeyBindType.Toggle, true)).Permashow();

            if (smiteCombo != null)
                menu.AddItem(new MenuItem("smiteskill",
                    "-> Smite + " + smiteCombo.Name + " (" + smiteCombo.Slot + ")")).SetValue(true);

            menu.AddItem(new MenuItem("smitesmall", "Smite Small Camps")).SetValue(true);
            // Menu.AddItem(new MenuItem("smitekrug", "-> Krug")).SetValue(true);
            // Menu.AddItem(new MenuItem("smitewolve", "-> Wolves")).SetValue(true);
            // Menu.AddItem(new MenuItem("smiterazor", "-> Razorbeak")).SetValue(true);
            menu.AddItem(new MenuItem("smitelarge", "Smite Large Camps")).SetValue(true);
            // Menu.AddItem(new MenuItem("smiteblu", "-> Blu")).SetValue(true);
            // Menu.AddItem(new MenuItem("smitered", "-> Red")).SetValue(true);
            menu.AddItem(new MenuItem("smitesuper", "Smite Epic Camps")).SetValue(true);
            // Menu.AddItem(new MenuItem("smitebaron", "-> Baron")).SetValue(true);
            // Menu.AddItem(new MenuItem("smitedragon", "-> Dragon")).SetValue(true);
            // Menu.AddItem(new MenuItem("smiterift", "-> Rift Herald")).SetValue(true);
            menu.AddItem(new MenuItem("smitemode", "Smite Enemies: "))
                .SetValue(new StringList(new[] { "Killsteal", "Combo", "Nope" }, 1));
            menu.AddItem(new MenuItem("savesmite", "-> Save a Smite Charge")
                .SetValue(true).SetTooltip("Will only combo smite if Ammo > 1"));
            // Menu.AddItem(new MenuItem("savesmite2", "-> Dont Smite Near Camps")
            // .SetValue(true).SetTooltip("Wont smite enemies near camps"));
        }

        internal static int Limiter;
        internal static void ComboSmite(Obj_AI_Base unit, float smitedmg)
        {
            AIHeroClient player = Activator.Player;
            foreach (var spell in Smitedata.CachedSpellList)
            {
                if (player.GetSpellDamage(unit, spell.Slot, spell.Stage) + smitedmg >= unit.Health)
                {
                    if (unit.Distance(player.ServerPosition) <= spell.CastRange +
                        unit.BoundingRadius + player.BoundingRadius)
                    {
                        if (spell.HeroReqs(unit))
                        {
                            switch (spell.Type)
                            {
                                case SpellDataTargetType.Location:
                                    player.Spellbook.CastSpell(spell.Slot, unit.ServerPosition);
                                    break;
                                case SpellDataTargetType.Unit:
                                    player.Spellbook.CastSpell(spell.Slot, unit);
                                    break;
                                case SpellDataTargetType.Self:
                                    player.Spellbook.CastSpell(spell.Slot);
                                    break;
                                case SpellDataTargetType.SelfAndUnit:
                                    player.Spellbook.CastSpell(spell.Slot);

                                    if (Utils.GameTimeTickCount - Limiter >= 200 && Orbwalking.InAutoAttackRange(unit))
                                    {
                                        LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping, () =>
                                        {
                                            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, unit);
                                            Limiter = Utils.GameTimeTickCount;
                                        });
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("usesmite").GetValue<KeyBind>().Active)
                return;

            foreach (var minion in MinionManager.GetMinions(900f, MinionTypes.All, MinionTeam.Neutral))
            {
                var damage = Player.Spellbook.GetSpell(Activator.Smite).State == SpellState.Ready
                    ? (float) Player.GetSummonerSpellDamage(minion, Damage.SummonerSpell.Smite)
                    : 0;

                if (minion.Distance(Player.ServerPosition) <= 500 + minion.BoundingRadius + Player.BoundingRadius + 5)
                {
                    if (Helpers.IsLargeMinion(minion) && Menu.Item("smitelarge").GetValue<bool>())
                    {
                        var smiteCombo = Smitedata.CachedSpellList.FirstOrDefault();
                        if (smiteCombo != null)
                        {
                            if (Menu.Item("smiteskill").GetValue<bool>())
                                ComboSmite(minion, damage);
                        }

                        if (damage >= minion.Health && IsReady())
                        {
                            Player.Spellbook.CastSpell(Activator.Smite, minion);
                        }
                    }

                    if (Helpers.IsSmallMinion(minion) && Menu.Item("smitesmall").GetValue<bool>())
                    {
                        var smiteCombo = Smitedata.CachedSpellList.FirstOrDefault();
                        if (smiteCombo != null)
                        {
                            if (Menu.Item("smiteskill").GetValue<bool>())
                                ComboSmite(minion, damage);
                        }

                        if (damage >= minion.Health && IsReady())
                            Player.Spellbook.CastSpell(Activator.Smite, minion);
                    }

                    if (Helpers.IsEpicMinion(minion) && Menu.Item("smitesuper").GetValue<bool>())
                    {
                        var smiteCombo = Smitedata.CachedSpellList.FirstOrDefault();
                        if (smiteCombo != null)
                        {
                            if (Menu.Item("smiteskill").GetValue<bool>())
                                ComboSmite(minion, damage);
                        }

                        if (damage >= minion.Health && IsReady())
                            Player.Spellbook.CastSpell(Activator.Smite, minion);
                    }
                }
            }

            // smite hero blu/red
            if (Activator.SmiteName.ToLower() == "s5_summonersmiteduel" ||
                Activator.SmiteName.ToLower() == "s5_summonersmiteplayerganker")
            {
                if (!Menu.Item("savesmite").GetValue<bool>() ||
                     Menu.Item("savesmite").GetValue<bool>() && Player.GetSpell(Activator.Smite).Ammo > 1)
                {
                    // KS Smite
                    if (Menu.Item("smitemode").GetValue<StringList>().SelectedIndex == 0 ||
                        Menu.Item("smitemode").GetValue<StringList>().SelectedIndex == 1 &&
                        Activator.SmiteName.ToLower() == "s5_summonersmiteplayerganker")
                    {
                        foreach (
                            var hero in
                                Activator.Heroes.Where(
                                    h =>
                                        h.Player.IsValidTarget(500) && !h.Player.IsZombie &&
                                        h.Player.Health <= 20 + 8 * Player.Level))
                        {
                            Player.Spellbook.CastSpell(Activator.Smite, hero.Player);
                        }
                    }

                    // Combo Smite
                    if (Menu.Item("smitemode").GetValue<StringList>().SelectedIndex == 1 &&
                       (Activator.SmiteName.ToLower() == "s5_summonersmiteduel" ||
                        Activator.SmiteName.ToLower() == "s5_summonersmiteplayerganker"))
                    {
                        if (Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
                        {
                            var t = TargetSelector.GetTarget(700f, TargetSelector.DamageType.True);
                            if (t.IsValidTarget(500))
                            {
                                Player.Spellbook.CastSpell(Activator.Smite, t);
                            }
                        }
                    }
                }
            }
        }
    }
}
