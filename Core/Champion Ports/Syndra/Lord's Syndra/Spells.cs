using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace LordsSyndra
{
    public static class Spells
    {
        public static List<Spell> SpellList = new List<Spell>();
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Spell QE;
        public static Spell QE2;
        public static int QEComboT;
        public static int WEComboT;
        public static int QWLastcast = 0;
        public static SpellSlot IgniteSlot;
        public static SpellSlot FlashSlot;
        public static int FlashLastCast;


        public static void UseQSpell(Obj_AI_Base target)
        {
            if (!Spells.Q.IsReady()) return;
            var pos = Q.GetPrediction(target, true);
            if (pos.Hitchance >= HitChance.VeryHigh && target.IsValidTarget(Spells.Q.Range))
                Spells.Q.Cast(pos.CastPosition);
        }

        public static void UseWSpell(Obj_AI_Base qeTarget, Obj_AI_Base wTarget)
        {
            //Use W1
            if (qeTarget != null && Spells.W.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1)
            {
                var gObjectPos = Utils.GetGrabableObjectPos(false);

                if (gObjectPos.To2D().IsValid() && Environment.TickCount - Spells.Q.LastCastAttemptT > Game.Ping + 150 &&
                    Environment.TickCount - Spells.E.LastCastAttemptT > 750 + Game.Ping &&
                    Environment.TickCount - Spells.W.LastCastAttemptT > 750 + Game.Ping)
                {
                    var grabsomething = false;
                    if (wTarget != null)
                    {
                        var pos2 = Spells.W.GetPrediction(wTarget, true);
                        if (pos2.Hitchance >= HitChance.High) grabsomething = true;
                    }
                    if (grabsomething || qeTarget.IsStunned)
                        Spells.W.Cast(gObjectPos);
                }
            }


            //            Chat.Print("wObject: " + OrbManager.WObject(false) + " Target " + wTarget.BaseSkinName + " toggle " + Player.Spellbook.GetSpell(SpellSlot.W).ToggleState + " isready: " + W.IsReady());
            //Use W2
            //            if (wTarget == null || Player.Spellbook.GetSpell(SpellSlot.W).ToggleState != 2 || !W.IsReady())
            //               return;


            if (wTarget != null && Spells.W.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 2 &&
                wTarget.IsValidTarget(Spells.W.Range) && Environment.TickCount - Spells.E.LastCastAttemptT > 300)
            {

                //                W.UpdateSourcePosition(OrbManager.WObject(false).ServerPosition);
                var pos = Spells.W.GetPrediction(wTarget, true);
                //                Chat.Print("casting w " + Player.Spellbook.GetSpell(SpellSlot.W).ToggleState + " pred"+pos.Hitchance);
                if (pos.Hitchance >= HitChance.High)
                    Spells.W.Cast(pos.CastPosition);
            }
        }

        public static void UseESpell(Obj_AI_Base target)
        {
            if (target == null)
                return;

            foreach (
                var orb in
                    OrbManager.GetOrbs(true)
                        .Where(orb => orb.To2D().IsValid() && ObjectManager.Player.Distance(orb, true) < Math.Pow(Spells.E.Range, 2)))
            {
                var sp = orb.To2D() + Vector2.Normalize(ObjectManager.Player.ServerPosition.To2D() - orb.To2D()) * 100f;
                var ep = orb.To2D() + Vector2.Normalize(orb.To2D() - ObjectManager.Player.ServerPosition.To2D()) * 592;

                Spells.QE.Delay = Spells.E.Delay + ObjectManager.Player.Distance(orb) / Spells.E.Speed;
                Spells.QE.UpdateSourcePosition(orb);

                var pPo = Spells.QE.GetPrediction(target).UnitPosition.To2D();

                if (pPo.Distance(sp, ep, true, true) <= Math.Pow(Spells.QE.Width + target.BoundingRadius, 2) &&
                    ObjectManager.Player.Distance(sp) <=
                    Spells.E.Range)
                    Spells.E.Cast(orb);
            }
        }

        public static void UseQeSpell(Obj_AI_Base target)
        {
            if (!Spells.Q.IsReady() || !Spells.E.IsReady() || target == null) return;
            var sPos = Prediction.GetPrediction(target, Spells.Q.Delay + Spells.E.Delay).UnitPosition;
            if (ObjectManager.Player.Distance(sPos, true) > Math.Pow(Spells.E.Range, 2))
            {
                var orb = ObjectManager.Player.ServerPosition + Vector3.Normalize
                    (sPos - ObjectManager.Player.ServerPosition) * Spells.E.Range;
                Spells.QE.Delay = Spells.Q.Delay + Spells.E.Delay + ObjectManager.Player.Distance(orb) / Spells.E.Speed;
                var pos = Spells.QE.GetPrediction(target);
                if (pos.Hitchance >= HitChance.Medium)
                {
                    UseQe2Spell(target, orb);
                }
            }
            else
            {
                Spells.Q.Width = 65f;
                var pos = Spells.Q.GetPrediction(target, true);
                Spells.Q.Width = 110f;
                if (pos.Hitchance >= HitChance.Medium)
                    UseQe2Spell(target, pos.UnitPosition);
            }
        }

        public static void UseQe2Spell(Obj_AI_Base target, Vector3 pos)
        {
            if (target == null || !(ObjectManager.Player.Distance(pos, true) <= Math.Pow(Spells.E.Range, 2)))
                return;
            var sp = pos + Vector3.Normalize(ObjectManager.Player.ServerPosition - pos) * 100f;
            var ep = pos + Vector3.Normalize(pos - ObjectManager.Player.ServerPosition) * 592;
            Spells.QE.Delay = Spells.Q.Delay + Spells.E.Delay + ObjectManager.Player.ServerPosition.Distance(pos) / Spells.E.Speed;
            Spells.QE.UpdateSourcePosition(pos);
            var pPo = Spells.QE.GetPrediction(target).UnitPosition.To2D().ProjectOn(sp.To2D(), ep.To2D());
            if (!(pPo.SegmentPoint.Distance(target, true) <= Math.Pow(Spells.QE.Width + target.BoundingRadius, 2)))
                return;
            var delay = 120 - (int)(ObjectManager.Player.Distance(pos) / 2.5) +
                Program.Menu.Item("QEDelay").GetValue<Slider>().Value;
            LeagueSharp.Common.Utility.DelayAction.Add(Math.Max(0, delay), () => Spells.E.Cast(pos));

            Spells.QE.LastCastAttemptT = Environment.TickCount;
            Spells.Q.Cast(pos);
            UseESpell(target);
        }

        public static void UpdateSpellRange()
        {
            //Update R Range
            Spells.R.Range = Spells.R.Level == 3 ? 750f : 675f;

            //Update E Width
            Spells.E.Width = Spells.E.Level == 5 ? 45f : (float)(45 * 0.5);

            //Update QE Range
            var qeRnew = Program.Menu.Item("QEMR").GetValue<Slider>().Value * .01 * 1292;
            Spells.QE.Range = (float)qeRnew;
        }

        public static void UseSpells(bool useQBool, bool useWBool, bool useEBool, bool useRBool, bool useQEBool)
        {
            //Set Target
            var qTarget = TargetSelector.GetTarget(Spells.Q.Range + 25f, TargetSelector.DamageType.Magical);
            var wTarget = TargetSelector.GetTarget(Spells.W.Range + Spells.W.Width, TargetSelector.DamageType.Magical);
            var rTarget = TargetSelector.GetTarget(Spells.R.Range, TargetSelector.DamageType.Magical);
            var qeTarget = TargetSelector.GetTarget(Spells.QE.Range + 150, TargetSelector.DamageType.Magical);

            //Harass Combo Key Override
            if (rTarget != null &&
                (Program.harassKey.GetValue<KeyBind>().Active || Program.laneclearKey.GetValue<KeyBind>().Active) &&
                Program.comboKey.GetValue<KeyBind>().Active &&
                ObjectManager.Player.Distance(rTarget, true) <= Math.Pow(Spells.R.Range, 2) &&
                Utils.BuffCheck(rTarget) && Utils.DetectCollision(rTarget))
            {
                if (Program.Menu.Item("DontR" + rTarget.CharData.BaseSkinName) != null &&
                    Program.Menu.Item("DontR" + rTarget.CharData.BaseSkinName).GetValue<bool>() == false &&
                    useRBool && GetDamage.overkillcheckv2(rTarget) <= rTarget.Health && ObjectManager.Player.HealthPercent >= 35)
                {
                    Spells.R.CastOnUnit(rTarget);
                    Spells.R.LastCastAttemptT = Environment.TickCount;
                }
            }
            if (Spells.IgniteSlot.IsReady() && qTarget.Health < GetDamage.GetIgniteDamage(qTarget))
                ObjectManager.Player.Spellbook.CastSpell(Spells.IgniteSlot, qTarget);

            if (Spells.R.IsReady())
            {
                //R, Ignite 
                foreach (var enemy in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(
                            enemy =>
                                enemy.Team != ObjectManager.Player.Team && enemy.IsValidTarget(Spells.R.Range) && !enemy.IsDead &&
                                Utils.BuffCheck(enemy)))
                {
                    //R
                    var useR = Program.Menu.Item("DontR" + enemy.CharData.BaseSkinName).GetValue<bool>() == false && useRBool;
                    var okR = Program.Menu.Item("okR" + enemy.CharData.BaseSkinName).GetValue<Slider>().Value * .01 + 1;
                    if (Utils.DetectCollision(enemy) && useR && ObjectManager.Player.Distance(enemy, true) <= Math.Pow(Spells.R.Range, 2) &&
                        (GetDamage.GetRDamage(enemy)) > enemy.Health * okR &&
                        RCheck(enemy))
                    {
                        if (
                            !(ObjectManager.Player.GetSpellDamage(enemy, SpellSlot.Q) > enemy.Health &&
                              ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).CooldownExpires - Game.Time < 2 &&
                              ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).CooldownExpires - Game.Time >= 0 && enemy.IsStunned) &&
                            Environment.TickCount - Spells.Q.LastCastAttemptT > 500 + Game.Ping &&
                            GetDamage.overkillcheckv2(rTarget) <= rTarget.Health && ObjectManager.Player.HealthPercent >= 35)
                        {
                            Spells.R.CastOnUnit(enemy);
                            Spells.R.LastCastAttemptT = Environment.TickCount;
                        }

                    }
                    //Ignite
                    if (!(ObjectManager.Player.Distance(enemy, true) <= 600 * 600) || !(GetDamage.GetIgniteDamage(enemy) > enemy.Health))
                        continue;
                    if (Program.Menu.Item("IgniteALLCD").GetValue<bool>())
                    {
                        if (!Spells.Q.IsReady() && !Spells.W.IsReady() && !Spells.E.IsReady() && !Spells.R.IsReady() &&
                            Environment.TickCount - Spells.R.LastCastAttemptT > Game.Ping + 750 &&
                            Environment.TickCount - Spells.QE.LastCastAttemptT > Game.Ping + 750 &&
                            Environment.TickCount - Spells.W.LastCastAttemptT > Game.Ping + 750)
                            ObjectManager.Player.Spellbook.CastSpell(Spells.IgniteSlot, enemy);
                    }
                    else
                        ObjectManager.Player.Spellbook.CastSpell(Spells.IgniteSlot, enemy);

                }
            }

            //Use QE
            if (useQEBool && Utils.DetectCollision(qeTarget) && qeTarget != null && Spells.Q.IsReady() &&
                (Spells.E.IsReady() && Spells.E.Level > 0) &&
                ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana + ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).SData.Mana <=
                ObjectManager.Player.Mana)
            {
                Spells.UseQeSpell(qeTarget);
            }

            //Use Q
            else if (useQBool && qTarget != null)
            {
                Spells.UseQSpell(qTarget);
            }

            //Use E
            if (useEBool && Spells.E.IsReady() && Environment.TickCount - Spells.W.LastCastAttemptT > Game.Ping + 150 &&
                Environment.TickCount - QWLastcast > Game.Ping)
                foreach (
                    var enemy in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(enemy => enemy.Team != ObjectManager.Player.Team)
                            .Where(enemy => enemy.IsValidTarget(Spells.E.Range)))
                {
                    if (GetDamage.GetComboDamage(enemy, useQBool, useWBool, useEBool, useRBool) > enemy.Health &&
                        ObjectManager.Player.Distance(enemy, true) <= Math.Pow(Spells.E.Range, 2))
                        Spells.E.Cast(enemy);
                    else if (ObjectManager.Player.Distance(enemy, true) <= Math.Pow(Spells.QE.Range, 2))
                        Spells.UseESpell(enemy);
                }
            //Use W
            if (useWBool)
            {
                Spells.UseWSpell(qeTarget, wTarget);
            }
        }
        public static bool RCheck(AIHeroClient enemy)
        {
            double aa = 0;
            if (Program.Menu.Item("DontRwA").GetValue<bool>()) aa = ObjectManager.Player.GetAutoAttackDamage(enemy);
            //Menu check
            if (Program.Menu.Item("DontRwParam").GetValue<StringList>().SelectedIndex == 2) return true;

            //If can be killed by all the skills that are checked
            if (Program.Menu.Item("DontRwParam").GetValue<StringList>().SelectedIndex == 0 &&
                GetDamage.GetComboDamage(enemy, Program.Menu.Item("DontRwQ").GetValue<bool>(), 
                Program.Menu.Item("DontRwW").GetValue<bool>(),
                Program.Menu.Item("DontRwE").GetValue<bool>(), false) + aa >= enemy.Health) return false;
            //If can be killed by either any of the skills
            if (Program.Menu.Item("DontRwParam").GetValue<StringList>().SelectedIndex == 1 &&
                (GetDamage.GetComboDamage(enemy, Program.Menu.Item("DontRwQ").GetValue<bool>(), false, false, false) >=
                 enemy.Health ||
                 GetDamage.GetComboDamage(enemy, Program.Menu.Item("DontRwW").GetValue<bool>(), false, false, false) >=
                 enemy.Health ||
                 GetDamage.GetComboDamage(enemy, Program.Menu.Item("DontRwE").GetValue<bool>(), false, false, false) >=
                 enemy.Health || aa >= enemy.Health)) return false;

            //Check last cast times
            return Environment.TickCount - Spells.Q.LastCastAttemptT > 600 + Game.Ping &&
                   Environment.TickCount - Spells.E.LastCastAttemptT > 600 + Game.Ping &&
                   Environment.TickCount - Spells.W.LastCastAttemptT > 600 + Game.Ping;
        }
        public static void Spellsdata()
        {
            Spells.Q = new Spell(SpellSlot.Q, 800);
            Spells.Q.SetSkillshot(0.85f, 125f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Spells.W = new Spell(SpellSlot.W, 925);
            Spells.W.SetSkillshot(0.25f, 140f, 1600f, false, SkillshotType.SkillshotCircle);

            Spells.E = new Spell(SpellSlot.E, 700);
            Spells.E.SetSkillshot(0.30f, (float)(45 * 0.5), 2500, false, SkillshotType.SkillshotCone);

            Spells.R = new Spell(SpellSlot.R, 675);
            Spells.R.SetTargetted(0.5f, 1100f);

            //QE = new Spell(SpellSlot.E, 1100);
            //QE.SetSkillshot(float.MaxValue, 55f, 2000f, false, SkillshotType.SkillshotCircle);
            Spells.QE = new Spell(SpellSlot.E, 1300);
            Spells.QE.SetSkillshot(float.MaxValue, 65f, 8000f, false, SkillshotType.SkillshotLine);


            Spells.IgniteSlot = ObjectManager.Player.GetSpellSlot("SummonerDot");
            Spells.FlashSlot = ObjectManager.Player.GetSpellSlot("summonerflash");

            Spells.SpellList.Add(Spells.Q);
            Spells.SpellList.Add(Spells.W);
            Spells.SpellList.Add(Spells.E);
            Spells.SpellList.Add(Spells.R);
        }

    }
}
