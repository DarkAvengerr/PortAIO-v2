using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;

using EloBuddy;
using LeagueSharp.Common;
namespace KicKassadin
{

    internal enum Spells
    {
        Q, W, E, R,
    }

    class KicKassadin
    {
        public const string Champion = "Kassadin";
        public static Orbwalking.Orbwalker Orbwalker;
        private static SpellSlot _ignite;

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        #region HitChance

        private static HitChance CustomHitChance
        {
            get { return GetHitchance(); }
        }

        private static HitChance GetHitchance()
        {
            switch (KassMenu.Config.Item("hitChance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.Medium;
            }
        }

        #endregion

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>() {
            {Spells.Q, new Spell(SpellSlot.Q, 650)},
            // Reduce the W range because it's the melee ability and you want to get a hit off
            {Spells.W, new Spell(SpellSlot.W, 150)},
            {Spells.E, new Spell(SpellSlot.E, 700)},
            {Spells.R, new Spell(SpellSlot.R, 500)},
        };

        #region OnLoad
        private static void Game_OnGameLoad()
        {

            KassMenu.Init();
            if (ObjectManager.Player.CharData.BaseSkinName != Champion)
                return;

            Notifications.AddNotification("KicKassadin by TheOBJop");

            // Set skillshots
            spells[Spells.Q].SetTargetted(0.5f, 1400);
            spells[Spells.E].SetSkillshot(0.5f, 10f, float.MaxValue, false, SkillshotType.SkillshotCone);
            spells[Spells.R].SetSkillshot(0.5f, 150f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            _ignite = SpellSlot.Summoner2;

            // Interruption
            Interrupter2.OnInterruptableTarget += (source, eventArgs) =>
            {
                if (source.IsEnemy && Vector3.Distance(source.Position, Player.Position) < spells[Spells.Q].Range)
                {
                    spells[Spells.Q].CastOnUnit(source);
                }
            };

            // Teleport away from the gap closer
            AntiGapcloser.OnEnemyGapcloser += (activeGapcloser) =>
            {
                if (KassMenu.Config.Item("Flee").GetValue<KeyBind>().Active && KassMenu.Config.Item("Antigap").GetValue<bool>() && spells[Spells.R].IsReady())
                    spells[Spells.R].Cast(Player.Position.Shorten(activeGapcloser.Sender.Position, spells[Spells.R].Range));
                else if (KassMenu.Config.Item("Antigap").GetValue<bool>() && spells[Spells.R].IsReady())
                    spells[Spells.R].Cast(Player.Position.Shorten(activeGapcloser.Sender.Position, Player.Position.Distance(activeGapcloser.End)));
            };

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawings.Drawing_OnDraw;
        }
        #endregion

        #region OnUpdate
        private static void Game_OnUpdate(EventArgs e)
        {
            if (Player.IsDead)
                return;

            // Flee takes priority, if you accidentally hold down both, you flee.
            if (KassMenu.Config.Item("Flee").GetValue<KeyBind>().Active)
            {
                Flee();
            }
            else
            {
                switch (Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Combo:
                        Combo();
                        break;
                    case Orbwalking.OrbwalkingMode.LaneClear:
                        LaneClear();
                        break;
                    case Orbwalking.OrbwalkingMode.Mixed:
                        Harass();
                        break;
                }
            }
        }
        #endregion

        #region Combo
        private static void Combo()
        {
            var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Magical);
            if (target == null)
                return;

            var useQ = KassMenu.Config.Item("Combo.Q").GetValue<bool>();
            var useW = KassMenu.Config.Item("Combo.W").GetValue<bool>();
            var useE = KassMenu.Config.Item("Combo.E").GetValue<bool>();
            var useR = KassMenu.Config.Item("Combo.R").GetValue<bool>();
            var useIgnite = KassMenu.Config.Item("Combo.Ignite").GetValue<bool>();
            var dontRCount = KassMenu.Config.Item("Combo.DontR").GetValue<Slider>().Value;
            var gapcloseR = KassMenu.Config.Item("Combo.GapcloseR").GetValue<bool>();
            var gapcloseLimit = KassMenu.Config.Item("Combo.MaxStacksForR").GetValue<Slider>().Value;
            var gapcloseHealth = KassMenu.Config.Item("Combo.GapCloseHealth").GetValue<Slider>().Value;

            var forceDive = KassMenu.Config.Item("Combo.Dive").GetValue<KeyBind>().Active;

            if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].CastOnUnit(target);
            }

            if (useW && spells[Spells.W].IsReady() && Player.Position.CountEnemiesInRange(spells[Spells.W].Range) >= 1)
            {
                spells[Spells.W].Cast();
            }

            if (useE && spells[Spells.E].IsReady() && target.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].CastIfHitchanceEquals(target, CustomHitChance);
            }

            if (useR && spells[Spells.R].IsReady() && (!target.UnderTurret() || forceDive))
            {
                var extraEnemies = Player.Position.GetEnemiesInRange(1000).Count;

                if (extraEnemies <= dontRCount)
                {
                    spells[Spells.R].CastIfHitchanceEquals(target, CustomHitChance);
                }
            }

            if (Player.Distance(target.Position) > spells[Spells.E].Range && gapcloseR && spells[Spells.R].IsReady() && Player.GetBuff("KassadinR").Count < gapcloseLimit && target.HealthPercent <= gapcloseHealth)
            {
                spells[Spells.R].Cast(Player.Position.Extend(target.Position, 500));
            }

            if (Player.Distance(target.Position) <= 600 && IgniteDamage(target) >= target.Health && useIgnite)
            {
                Player.Spellbook.CastSpell(_ignite, target);
            }
        }
        #endregion

        #region LaneClear
        private static void LaneClear()
        {
            var minMana = KassMenu.Config.Item("Laneclear.Mana").GetValue<Slider>().Value;

            var useQ = KassMenu.Config.Item("Laneclear.Q").GetValue<bool>();
            var useW = KassMenu.Config.Item("Laneclear.W").GetValue<bool>();
            var useE = KassMenu.Config.Item("Laneclear.E").GetValue<bool>();
            var useR = KassMenu.Config.Item("Laneclear.R").GetValue<bool>();

            var MinionsQ = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.Q].Range);
            var MinionsW = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.W].Range);
            var MinionsE = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.E].Range);
            var MinionsR = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.R].Range);

            Obj_AI_Base lowestHp = null;
            // Get minion with lowest hp for Q
            if (MinionsQ != null)
            {
                lowestHp = MinionsQ[0];
                for (int i = 0; i < MinionsQ.Count; i++)
                {
                    if (lowestHp.HealthPercent > MinionsQ[i].HealthPercent)
                    {
                        lowestHp = MinionsQ[i];
                    }
                }
            }

            var Wfarmpos = spells[Spells.W].GetLineFarmLocation(MinionsW, spells[Spells.W].Width);
            var Efarmpos = spells[Spells.E].GetCircularFarmLocation(MinionsE, spells[Spells.E].Width);
            var Rfarmpos = spells[Spells.R].GetCircularFarmLocation(MinionsR, spells[Spells.R].Width);

            if (useQ && spells[Spells.Q].IsReady() && lowestHp != null && Player.ManaPercent >= minMana)
                spells[Spells.Q].CastOnUnit(lowestHp);
            if (useW && Player.ManaPercent >= minMana)
                spells[Spells.W].Cast(Wfarmpos.Position);
            if (Efarmpos.MinionsHit >= 3 && MinionsE.Count >= 3 && useE && Player.ManaPercent >= minMana)
                spells[Spells.E].Cast(Efarmpos.Position);
            if (Rfarmpos.MinionsHit >= 3 && MinionsR.Count >= 3 && useR && Player.ManaPercent >= minMana)
                spells[Spells.R].Cast(Rfarmpos.Position);

        }
        #endregion

        #region Harass
        private static void Harass()
        {
            var targetQ = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            var targetE = TargetSelector.GetTarget(spells[Spells.E].Range, TargetSelector.DamageType.Magical);
            if (targetQ == null && targetE == null)
                return;

            var useQ = KassMenu.Config.Item("Harass.Q").GetValue<bool>();
            var useE = KassMenu.Config.Item("Harass.E").GetValue<bool>();
            var minMana = KassMenu.Config.Item("Harass.MinMana").GetValue<Slider>().Value;

            if (useQ && spells[Spells.Q].IsReady() && targetQ.IsValidTarget(spells[Spells.Q].Range) && Player.ManaPercent > minMana)
            {
                spells[Spells.Q].CastOnUnit(targetQ);
            }

            if (useE && spells[Spells.E].IsReady() && targetE.IsValidTarget(spells[Spells.E].Range) && Player.ManaPercent > minMana)
            {
                spells[Spells.E].Cast(targetE.Position);
            }
        }
        #endregion

        #region Flee
        private static void Flee()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (spells[Spells.R].IsReady())
            {
                spells[Spells.R].Cast(Player.Position.Extend(Game.CursorPos, spells[Spells.R].Range));
            }
        }
        #endregion

        #region IgniteDamage

        private static float IgniteDamage(AIHeroClient target)
        {
            if (_ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(_ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        #endregion

        #region ComboDamage

        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            float damage = 0;

            if (spells[Spells.Q].IsReady())
            {
                damage += spells[Spells.Q].GetDamage(enemy);
            }

            if (spells[Spells.W].IsReady())
            {
                damage += spells[Spells.W].GetDamage(enemy);
            }

            if (spells[Spells.E].IsReady())
            {
                damage += spells[Spells.E].GetDamage(enemy);
            }

            if (spells[Spells.R].IsReady())
            {
                damage += spells[Spells.R].GetDamage(enemy);
            }

            if (_ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(_ignite) != SpellState.Ready)
            {
                damage += (float)Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
            }

            return damage;
        }

        #endregion

        #region Main
        public static void Main()
        {
            Game_OnGameLoad();
        }
        #endregion
    }
}
