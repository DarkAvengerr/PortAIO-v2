using System;
using System.Linq;
using Activator.Base;
using Activator.Data;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; namespace Activator.Summoners
{
    internal class smite : CoreSum
    {
        internal override string Name => "summonersmite";
        internal override string DisplayName => "Smite";
        internal override float Range => 500f;
        internal override int Duration => 0;
        internal override string[] ExtraNames => new[]
        {
            "s5_summonersmiteplayerganker", "s5_summonersmiteduel",
            "s5_summonersmitequick", "itemsmiteaoe"
        };

        internal static int Limiter;
        internal static void L33TSmite(Obj_AI_Base unit, float smitedmg)
        {
            foreach (var hero in Smitedata.SpellList.Where(x => x.Name == Activator.Player.ChampionName))
            {
                if (Activator.Player.GetSpellDamage(unit, hero.Slot, hero.Stage) + smitedmg >= unit.Health)
                {
                    if (unit.Distance(Activator.Player.ServerPosition) <= hero.CastRange + 
                        unit.BoundingRadius + Activator.Player.BoundingRadius)
                    {
                        if (hero.HeroReqs(unit))
                        {
                            switch (hero.Type)
                            {
                                case SpellDataTargetType.Location:
                                    Activator.Player.Spellbook.CastSpell(hero.Slot, unit.ServerPosition);
                                    break;
                                case SpellDataTargetType.Unit:
                                    Activator.Player.Spellbook.CastSpell(hero.Slot, unit);
                                    break;
                                case SpellDataTargetType.Self:
                                    Activator.Player.Spellbook.CastSpell(hero.Slot);
                                    break;
                                case SpellDataTargetType.SelfAndUnit:
                                    Activator.Player.Spellbook.CastSpell(hero.Slot);

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

                if (minion.Distance(Player.ServerPosition) <= 500 + minion.BoundingRadius + Player.BoundingRadius)
                {
                    if (Helpers.IsLargeMinion(minion) && Menu.Item("smitelarge").GetValue<bool>())
                    {
                        if (Menu.Item("smiteskill").GetValue<bool>())
                            L33TSmite(minion, damage);

                        if (damage >= minion.Health && IsReady())
                        {
                            Player.Spellbook.CastSpell(Activator.Smite, minion);
                        }
                    }

                    if (Helpers.IsSmallMinion(minion) && Menu.Item("smitesmall").GetValue<bool>())
                    {
                        if (Menu.Item("smiteskill").GetValue<bool>())
                            L33TSmite(minion, damage);

                        if (damage >= minion.Health && IsReady())
                        {
                            Player.Spellbook.CastSpell(Activator.Smite, minion);
                        }
                    }

                    if (Helpers.IsEpicMinion(minion) && Menu.Item("smitesuper").GetValue<bool>())
                    {
                        if (Menu.Item("smiteskill").GetValue<bool>())
                            L33TSmite(minion, damage);

                        if (damage >= minion.Health && IsReady())
                        {
                            Player.Spellbook.CastSpell(Activator.Smite, minion);
                        }
                    }
                }
            }

            // smite hero blu/red
            if (Player.GetSpell(Activator.Smite).Name.ToLower() == "s5_summonersmiteduel" ||
                Player.GetSpell(Activator.Smite).Name.ToLower() == "s5_summonersmiteplayerganker")
            {
                if (!Menu.Item("savesmite").GetValue<bool>() ||
                     Menu.Item("savesmite").GetValue<bool>() && Player.GetSpell(Activator.Smite).Ammo > 1)
                {
                    // KS Smite
                    if (Menu.Item("smitemode").GetValue<StringList>().SelectedIndex == 0 &&
                        Player.GetSpell(Activator.Smite).Name.ToLower() == "s5_summonersmiteplayerganker")
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
                       (Player.GetSpell(Activator.Smite).Name.ToLower() == "s5_summonersmiteduel" ||
                        Player.GetSpell(Activator.Smite).Name.ToLower() == "s5_summonersmiteplayerganker"))
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
