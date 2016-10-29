
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace NabbActivator
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    /// <summary>
    ///     The activator class.
    /// </summary>
    internal partial class Activator
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Loads the smite.
        /// </summary>
        public static void SmiteInit()
        {
            Drawing.OnDraw += delegate
                {
                    /// <summary>
                    ///     The Smite Logics.
                    /// </summary>
                    if (Vars.Smite != null && Vars.Smite.IsReady() && Vars.Smite.Slot != SpellSlot.Unknown)
                    {
                        if (!Vars.Menu["keys"]["smite"].GetValue<MenuKeyBind>().Active)
                        {
                            return;
                        }

                        /// <summary>
                        ///     The Jungle Smite Logic.
                        /// </summary>
                        foreach (var minion in
                            Targets.JungleMinions.Where(m => m.IsValidTarget(Vars.Smite.Range)))
                        {
                            var buff =
                                GameObjects.Player.Buffs.FirstOrDefault(
                                    b => b.Name.ToLower().Contains("smitedamagetracker"));
                            if (buff != null && minion.Health > GameObjects.Player.GetBuffCount(buff.Name))
                            {
                                return;
                            }

                            if (Vars.Menu["smite"]["misc"]["limit"].GetValue<MenuBool>().Value)
                            {
                                if (!minion.CharData.BaseSkinName.Equals("SRU_Baron")
                                    && !minion.CharData.BaseSkinName.Equals("SRU_RiftHerald")
                                    && !minion.CharData.BaseSkinName.Contains("SRU_Dragon"))
                                {
                                    return;
                                }
                            }

                            if (Vars.Menu["smite"]["misc"]["stacks"].GetValue<MenuBool>().Value)
                            {
                                if (GameObjects.Player.Spellbook.GetSpell(Vars.Smite.Slot).Ammo == 1)
                                {
                                    if (!minion.CharData.BaseSkinName.Equals("SRU_Baron")
                                        && !minion.CharData.BaseSkinName.Equals("SRU_RiftHerald")
                                        && !minion.CharData.BaseSkinName.Contains("SRU_Dragon"))
                                    {
                                        return;
                                    }
                                }
                            }

                            Vars.Smite.CastOnUnit(minion);
                        }
                    }
                };
        }

        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Spells(EventArgs args)
        {
            if (!Vars.Menu["spells"].GetValue<MenuBool>().Value)
            {
                return;
            }

            /// <summary>
            ///     The Remove Scurvy Logic.
            /// </summary>
            if (GameObjects.Player.ChampionName.Equals("Gangplank"))
            {
                if (Vars.W != null && Vars.W.IsReady() && Bools.ShouldCleanse(GameObjects.Player))
                {
                    DelayAction.Add(
                        Vars.Menu["cleansers"].GetValue<MenuSliderButton>().SValue,
                        () => { Vars.W.Cast(); });
                }
            }

            /// <summary>
            ///     The Cleanse Logic.
            /// </summary>
            if (SpellSlots.Cleanse.IsReady())
            {
                if (Bools.ShouldCleanse(GameObjects.Player))
                {
                    DelayAction.Add(
                        Vars.Menu["cleansers"].GetValue<MenuSliderButton>().SValue,
                        () => { GameObjects.Player.Spellbook.CastSpell(SpellSlots.Cleanse); });
                }
            }

            /// <summary>
            ///     The Clarity Logic.
            /// </summary>
            if (SpellSlots.Clarity.IsReady())
            {
                if (GameObjects.AllyHeroes.Count(a => a.ManaPercent <= 60) >= 3)
                {
                    GameObjects.Player.Spellbook.CastSpell(SpellSlots.Clarity);
                }
            }

            /// <summary>
            ///     The Ignite Logic.
            /// </summary>
            if (SpellSlots.Ignite.IsReady())
            {
                foreach (var target in
                    GameObjects.EnemyHeroes.Where(t => t.IsValidTarget(600f + GameObjects.Player.BoundingRadius)))
                {
                    if (Vars.GetIgniteDamage > target.Health
                        || Health.GetPrediction(target, (int)(1000 + Game.Ping / 2f)) <= 0)
                    {
                        GameObjects.Player.Spellbook.CastSpell(SpellSlots.Ignite, target);
                    }
                }
            }

            /// <summary>
            ///     The Barrier Logic.
            /// </summary>
            if (SpellSlots.Barrier.IsReady())
            {
                if (GameObjects.Player.CountEnemyHeroesInRange(700f) > 0
                    && Health.GetPrediction(GameObjects.Player, (int)(1000 + Game.Ping / 2f))
                    <= GameObjects.Player.MaxHealth / 6)
                {
                    GameObjects.Player.Spellbook.CastSpell(SpellSlots.Barrier);
                    return;
                }
            }

            /// <summary>
            ///     The Heal Logic.
            /// </summary>
            if (SpellSlots.Heal.IsReady())
            {
                if (GameObjects.Player.CountEnemyHeroesInRange(850f + GameObjects.Player.BoundingRadius) > 0
                    && Health.GetPrediction(GameObjects.Player, (int)(1000 + Game.Ping / 2f))
                    <= GameObjects.Player.MaxHealth / 6)
                {
                    GameObjects.Player.Spellbook.CastSpell(SpellSlots.Heal);
                }
                else
                {
                    foreach (var ally in
                        GameObjects.AllyHeroes.Where(
                            a =>
                            a.IsValidTarget(850f + GameObjects.Player.BoundingRadius, false)
                            && a.CountEnemyHeroesInRange(850f) > 0
                            && Health.GetPrediction(a, (int)(1000 + Game.Ping / 2f)) <= a.MaxHealth / 6))
                    {
                        GameObjects.Player.Spellbook.CastSpell(SpellSlots.Heal, ally);
                    }
                }
            }

            /// <summary>
            ///     The Smite Logics.
            /// </summary>
            if (Vars.Smite.IsReady() && Vars.Smite.Slot != SpellSlot.Unknown)
            {
                if (!Vars.Menu["keys"]["smite"].GetValue<MenuKeyBind>().Active)
                {
                    return;
                }

                /// <summary>
                ///     The Combo Smite Logic.
                /// </summary>
                if (Vars.Menu["smite"]["misc"]["combo"].GetValue<MenuBool>().Value)
                {
                    if (Variables.Orbwalker.GetTarget() is AIHeroClient)
                    {
                        Vars.Smite.CastOnUnit(Variables.Orbwalker.GetTarget() as AIHeroClient);
                    }
                }

                /// <summary>
                ///     The Killsteal Smite Logic.
                /// </summary>
                if (Vars.Menu["smite"]["misc"]["killsteal"].GetValue<MenuBool>().Value)
                {
                    if (GameObjects.Player.HasBuff("smitedamagetrackerstalker")
                        || GameObjects.Player.HasBuff("smitedamagetrackerskirmisher"))
                    {
                        if (Vars.Menu["smite"]["misc"]["stacks"].GetValue<MenuBool>().Value)
                        {
                            if (GameObjects.Player.Spellbook.GetSpell(Vars.Smite.Slot).Ammo == 1)
                            {
                                return;
                            }
                        }

                        foreach (var target in GameObjects.EnemyHeroes.Where(t => t.IsValidTarget(Vars.Smite.Range)))
                        {
                            if (Vars.GetChallengingSmiteDamage > target.Health
                                && GameObjects.Player.HasBuff("smitedamagetrackerstalker"))
                            {
                                Vars.Smite.CastOnUnit(target);
                            }
                            else if (Vars.GetChallengingSmiteDamage > target.Health
                                     && GameObjects.Player.HasBuff("smitedamagetrackerskirmisher"))
                            {
                                Vars.Smite.CastOnUnit(target);
                            }
                        }
                    }
                }
            }

            if (!Targets.Target.IsValidTarget())
            {
                return;
            }

            /// <summary>
            ///     The Exhaust Logic.
            /// </summary>
            if (SpellSlots.Exhaust.IsReady())
            {
                foreach (var ally in
                    GameObjects.AllyHeroes.Where(
                        a =>
                        a.CountEnemyHeroesInRange(650f) >= 1
                        && a.Distance(GameObjects.EnemyHeroes.OrderBy(o => o.Distance(a)).FirstOrDefault()) < 700f
                        && GameObjects.EnemyHeroes.OrderBy(o => o.Distance(a))
                               .FirstOrDefault()
                               .IsValidTarget(650f + GameObjects.Player.BoundingRadius)
                        && Health.GetPrediction(a, (int)(1000 + Game.Ping / 2f)) <= a.MaxHealth / 6))
                {
                    GameObjects.Player.Spellbook.CastSpell(
                        SpellSlots.Exhaust,
                        GameObjects.EnemyHeroes.OrderBy(o => o.Distance(ally)).FirstOrDefault());
                }
            }
        }

        #endregion
    }
}