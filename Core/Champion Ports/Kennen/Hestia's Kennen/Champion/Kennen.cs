using System;
using System.Linq;
using Kennen.Core;
using Kennen.Modes;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Kennen.Champion
{
    internal class Kennen
    {
        public Kennen()
        {
            Kennen_OnLoad();
        }

        public static void Kennen_OnLoad()
        {
            if (ObjectManager.Player.ChampionName != "Kennen")
                return;

            Spells.Initialize();
            Configs.Initialize();

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawings.Drawing_OnDraw;
            //LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = ComboDamage;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
                return;

            switch (Configs.orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo.ExecuteCombo();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    LastHit.ExecuteLastHit();
                    Harass.ExecuteHarass();
                    break;

                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit.ExecuteLastHit();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear.ExecuteLaneClear();
                    JungleClear.ExecuteJungleClear();
                    break;
                case Orbwalking.OrbwalkingMode.CustomMode:
                    Flee.ExecuteFlee();
                    break;
            }

            KillSteal();
            AutoQ();
            CastMultiR();
        }

        private static void CastMultiR()
        {
            var castRmulti = Configs.config.Item("useRmulti").GetValue<bool>() && Spells.R.IsReady();
            var multiRtargets = Configs.config.Item("multiRtargets").GetValue<Slider>().Value;

            if (castRmulti && ObjectManager.Player.CountEnemiesInRange(Spells.R.Range + 50) > multiRtargets)
            {
                Spells.R.Cast();
            }
        }

        private static void KillSteal()
        {
            if (!Configs.config.Item("killsteal").GetValue<bool>())
                return;

            if (Configs.config.Item("useQks").GetValue<bool>() && Spells.Q.IsReady())
            {
                foreach (
                    var target in
                        HeroManager.Enemies.Where(
                            enemy =>
                                enemy.IsValidTarget(Spells.Q.Range) && !enemy.HasBuffOfType(BuffType.Invulnerability))
                            .Where(target => target.Health < ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q)))
                {
                    Spells.Q.CastSpell(target, "predMode", "hitchanceQ");
                }
            }

            if (Configs.config.Item("useWks").GetValue<bool>() && Spells.W.IsReady())
            {
                foreach (
                    var target in
                        HeroManager.Enemies.Where(
                            enemy =>
                                enemy.IsValidTarget(Spells.W.Range) && !enemy.HasBuffOfType(BuffType.Invulnerability))
                            .Where(target => target.Health < ObjectManager.Player.GetSpellDamage(target, SpellSlot.W)))
                {
                    if (HasMark(target))
                    {
                        Spells.W.Cast();
                    }
                }
            }

            if (Configs.config.Item("useIks").GetValue<bool>() && Spells.Ignite.Slot.IsReady() && Spells.Ignite != null &&
                Spells.Ignite.Slot != SpellSlot.Unknown)
            {
                foreach (
                    var target in
                        HeroManager.Enemies.Where(
                            enemy =>
                                enemy.IsValidTarget(Spells.Ignite.SData.CastRange) &&
                                !enemy.HasBuffOfType(BuffType.Invulnerability))
                            .Where(
                                target =>
                                    target.Health <
                                    ObjectManager.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite)))
                {
                    ObjectManager.Player.Spellbook.CastSpell(Spells.Ignite.Slot, target);
                }
            }
        }

        private static void AutoQ()
        {
            var autoQ = Configs.config.Item("autoQ").GetValue<KeyBind>().Active && Spells.Q.IsReady();

            var target = TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Magical);

            if (autoQ && target.IsValidTarget(Spells.Q.Range))
            {
                Spells.Q.CastSpell(target, "predMode", "hitchanceQ");
            }
        }

        public static bool HasMark(Obj_AI_Base target)
        {
            return target.HasBuff("KennenMarkOfStorm");
        }

        public static bool CanStun(Obj_AI_Base target)
        {
            return target.GetBuffCount("KennenMarkOfStorm") == 2;
        }

        public static bool IsRushing()
        {
            return ObjectManager.Player.HasBuff("KennenLightningRush");
        }

        public static float ComboDamage(Obj_AI_Base enemy)
        {
            double damage = 0;
            if (Spells.Q.IsReady())
            {
                damage += Spells.Q.GetDamage(enemy);
            }
            if (Spells.W.IsReady())
            {
                damage += Spells.W.GetDamage(enemy);
            }
            if (Spells.R.IsReady())
            {
                damage += Spells.R.GetDamage(enemy);
            }
            var igniteDamage = ObjectManager.Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
            if (ObjectManager.Player.Spellbook.CanUseSpell(ObjectManager.Player.GetSpellSlot("summonerdot")) == SpellState.Ready)
            {
                damage += igniteDamage;
            }

            return (float) damage;
        }
    }
}
