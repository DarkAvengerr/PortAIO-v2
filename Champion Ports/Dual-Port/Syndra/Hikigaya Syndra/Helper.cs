using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy;
using LeagueSharp.Common;
namespace Hikigaya_Syndra
{
    class Helper : Program
    {
        public static Vector3 GetGrabableObjectPos(bool onlyOrbs)
        {
            if (onlyOrbs)
                return OrbManager.GetOrbToGrab((int)W.Range);
            foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValidTarget(W.Range)))
                return minion.ServerPosition;
            return OrbManager.GetOrbToGrab((int)W.Range);
        }

        public static void UseW(Obj_AI_Base grabObject, Obj_AI_Base enemy)
        {
            if (grabObject != null && W.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "SyndraW")
            {
                var gObjectPos = GetGrabableObjectPos(false);

                if (gObjectPos.To2D().IsValid() && Environment.TickCount - Q.LastCastAttemptT > Game.Ping + 150
                    && Environment.TickCount - E.LastCastAttemptT > 750 + Game.Ping && Environment.TickCount - W.LastCastAttemptT > 750 + Game.Ping)
                {
                    var grabsomething = false;
                    if (enemy != null)
                    {
                        var pos2 = W.GetPrediction(enemy, true);
                        if (pos2.Hitchance >= HitChance.High) grabsomething = true;
                    }
                    if (grabsomething || grabObject.IsStunned)
                    {
                        W.Cast(gObjectPos);
                    }

                }
            }
            if (enemy != null && W.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "SyndraWCast")
            {
                var pos = W.GetPrediction(enemy, true);
                if (pos.Hitchance >= HitChance.High)
                {
                    W.Cast(pos.CastPosition);
                }
            }
        }
        public static void UseQe(Obj_AI_Base target)
        {
            if (!Q.IsReady() || !E.IsReady() || target == null) return;
            var sPos = Prediction.GetPrediction(target, Q.Delay + E.Delay).UnitPosition;
            if (ObjectManager.Player.Distance(sPos, true) > Math.Pow(E.Range, 2))
            {
                var orb = ObjectManager.Player.ServerPosition + Vector3.Normalize(sPos - ObjectManager.Player.ServerPosition) * E.Range;
                Qe.Delay = Q.Delay + E.Delay + ObjectManager.Player.Distance(orb) / E.Speed;
                var pos = Qe.GetPrediction(target);
                if (pos.Hitchance >= HitChance.High)
                {
                    UseQe2(target, orb);
                }
            }
            else
            {
                Q.Width = 40f;
                var pos = Q.GetPrediction(target, true);
                Q.Width = 125f;
                if (pos.Hitchance >= HitChance.VeryHigh)
                    UseQe2(target, pos.UnitPosition);
            }
        }

        public static void UseE(Obj_AI_Base target)
        {
            if (target == null)
            {
                return;
            }
            foreach (var orb in OrbManager.GetOrbs(true).Where(orb => orb.To2D().IsValid() && ObjectManager.Player.Distance(orb, true) < Math.Pow(E.Range, 2)))
            {
                var sp = orb.To2D() + Vector2.Normalize(ObjectManager.Player.ServerPosition.To2D() - orb.To2D()) * 100f;
                var ep = orb.To2D() + Vector2.Normalize(orb.To2D() - ObjectManager.Player.ServerPosition.To2D()) * 592;
                Qe.Delay = E.Delay + ObjectManager.Player.Distance(orb) / E.Speed;
                Qe.UpdateSourcePosition(orb);
                var pPo = Qe.GetPrediction(target).UnitPosition.To2D();
                if (pPo.Distance(sp, ep, true, true) <= Math.Pow(Qe.Width + target.BoundingRadius, 2))
                {
                    E.Cast(orb);
                }
            }
        }

        public static void UseQe2(Obj_AI_Base target, Vector3 pos)
        {
            if (target == null || !(ObjectManager.Player.Distance(pos, true) <= Math.Pow(E.Range, 2)))
            {
                return;
            }

            var sp = pos + Vector3.Normalize(ObjectManager.Player.ServerPosition - pos) * 100f;
            var ep = pos + Vector3.Normalize(pos - ObjectManager.Player.ServerPosition) * 592;
            Qe.Delay = Q.Delay + E.Delay + ObjectManager.Player.ServerPosition.Distance(pos) / E.Speed;
            Qe.UpdateSourcePosition(pos);
            var pPo = Qe.GetPrediction(target).UnitPosition.To2D().ProjectOn(sp.To2D(), ep.To2D());

            if (!pPo.IsOnSegment ||
                !(pPo.SegmentPoint.Distance(target, true) <= Math.Pow(Qe.Width + target.BoundingRadius, 2)))
            {
                return;
            }

            var delay = 280 - (int)(ObjectManager.Player.Distance(pos) / 2.5) + Config.Item("q.e.delay").GetValue<Slider>().Value;
            LeagueSharp.Common.Utility.DelayAction.Add(Math.Max(0, delay), () => E.Cast(pos));
            Qe.LastCastAttemptT = Environment.TickCount;
            Q.Cast(pos);
            UseE(target);
        }

        public static float TotalDamage(Obj_AI_Base enemy)
        {
            var damage = 0f;
            if (Q.IsReady() && Config.Item("q.combo").GetValue<bool>())
            {
                damage += (float)ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Magical,
                    Q.GetDamage(enemy));
            }
            if (W.IsReady() && Config.Item("w.combo").GetValue<bool>())
            {
                damage += (float)ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Magical,
                    W.GetDamage(enemy));
            }
            if (E.IsReady() && Config.Item("e.combo").GetValue<bool>())
            {
                damage += (float)ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Magical,
                    E.GetDamage(enemy));
            }
            if (R.IsReady() && Config.Item("r.combo").GetValue<bool>())
            {
                damage += (float)ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Magical,
                    R.GetDamage(enemy)) * ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo;
            }
            if (IgniteSlot != SpellSlot.Unknown && ObjectManager.Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready &&
                Config.Item("r.combo").GetValue<bool>())
            {
                damage += (float)ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Magical,
                    ObjectManager.Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite));
            }
            return damage;
        }
        public static bool BuffCheck(Obj_AI_Base enemy)
        {
            var targetBuffs = new HashSet<string>(enemy.Buffs.Select(buff => buff.Name), StringComparer.OrdinalIgnoreCase);
            /*
             rUndyMenu.AddItem(new MenuItem("kindred.r", "Kindred's Lamb's Respite(R)").SetValue(true));
                            rUndyMenu.AddItem(new MenuItem("vlad.w", "Vladimir (W)").SetValue(true));
                            rUndyMenu.AddItem(new MenuItem("try.r", "Tryndamere's Undying Rage (R)").SetValue(true));
                            rUndyMenu.AddItem(new MenuItem("kayle.r", "Kayle's Intervention (R)").SetValue(true));
                            rUndyMenu.AddItem(new MenuItem("morgana.e", "Morgana's Black Shield (E)").SetValue(true));
                            rUndyMenu.AddItem(new MenuItem("sivir.e", "Sivir's Spell Shield (E)").SetValue(true));
                            rUndyMenu.AddItem(new MenuItem("banshee.passive", "Banshee's Veil (PASSIVE)").SetValue(true));
                            rUndyMenu.AddItem(new MenuItem("nocturne.w", "Nocturne's Shroud of Darkness (W)").SetValue(true));

                            rUndyMenu.AddItem(new MenuItem("aatrox.r", "Aatrox's (R)").SetValue(true));
                            rUndyMenu.AddItem(new MenuItem("zac.passive", "Zac's (PASSIVE)").SetValue(true));
                            rUndyMenu.AddItem(new MenuItem("alistar.r", "Alistar's (R)").SetValue(true));
             */
            // Kindred's Lamb's Respite(R)
            if (targetBuffs.Contains("KindredRNoDeathBuff") && enemy.HealthPercent <= 10 && Config.Item("kindred.r").GetValue<bool>())
            {
                return true;
            }

            // Vladimir W
            if (targetBuffs.Contains("VladimirSanguinePool") && Config.Item("vlad.w").GetValue<bool>())
            {
                return true;
            }

            // Tryndamere's Undying Rage (R)
            if (targetBuffs.Contains("UndyingRage") && enemy.Health <= enemy.MaxHealth * 0.10f && Config.Item("try.r").GetValue<bool>())
            {
                return true;
            }

            // Kayle's Intervention (R)
            if (targetBuffs.Contains("JudicatorIntervention") && Config.Item("kayle.r").GetValue<bool>())
            {
                return true;
            }

            // Morgana's Black Shield (E)
            if (targetBuffs.Contains("BlackShield") && Config.Item("morgana.e").GetValue<bool>()
                 && enemy.Health + enemy.MagicShield > TotalDamage(enemy))
            {
                return true;
            }

            // Banshee's Veil (PASSIVE)
            if (targetBuffs.Contains("bansheesveil") && Config.Item("banshee.passive").GetValue<bool>())
            {
                return true;
            }

            // Sivir's Spell Shield (E)
            if (targetBuffs.Contains("SivirE") && Config.Item("sivir.e").GetValue<bool>())
            {
                return true;
            }

            // Nocturne's Shroud of Darkness (W)
            if (targetBuffs.Contains("NocturneShroudofDarkness") && Config.Item("nocturne.w").GetValue<bool>())
            {
                return true;
            }

            // Aatrox (PASSIVE)
            if (targetBuffs.Contains("aatroxpassivedeath") && Config.Item("aatrox.passive").GetValue<bool>())
            {
                return true;
            }

            // Zac (PASSIVE)
            if (targetBuffs.Contains("ZacRebirthReady") && Config.Item("zac.passive").GetValue<bool>())
            {
                return true;
            }

            // Sion (PASSIVE)
            if (targetBuffs.Contains("sionpassivezombie") && Config.Item("sion.passive").GetValue<bool>())
            {
                return true;
            }

            // Zilean's Chrono's Shift (R)
            if (targetBuffs.Contains("chronoshift") && Config.Item("zilean.r").GetValue<bool>())
            {
                return true;
            }

            // Yorick's Zombies
            if (targetBuffs.Contains("yorickrazombie") && Config.Item("yorick.zombie").GetValue<bool>())
            {
                return true;
            }

            return false;
        }
    }
}