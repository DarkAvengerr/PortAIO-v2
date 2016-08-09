using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Vi.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Vi.Champion
{
    internal static class PlayerSpells
    {
        public static List<Spell> SpellList = new List<Spell>();

        public static Spell Q, E, R;

        public static int LastAutoAttackTick;

        public static int LastQCastTick;

        public static int LastECastTick;

        public static int LastSpellCastTick;

        private static int AutoAttackCount = 1;

        public static string FirstCastedSpell = "";

        public static string SeconCastedSpell = "";

        private static List<string> CastList = new List<string>();

        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 860f);
            Q.SetSkillshot(0.5f, 75f, float.MaxValue, false, SkillshotType.SkillshotLine);
            Q.SetCharged("ViQ", "ViQ", 50, 860, 1f);
            
            E = new Spell(SpellSlot.E, 310);
            E.SetSkillshot(0.15f, 150f, float.MaxValue, false, SkillshotType.SkillshotLine);
            
            R = new Spell(SpellSlot.R, 800f);
            R.SetTargetted(0.15f, 1500f);

            SpellList.AddRange(new[] { Q, E, R });

            Game.OnUpdate += GameOnOnUpdate;
            Drawing.OnDraw += DrawingOnOnDraw;
            Obj_AI_Base.OnProcessSpellCast += Game_OnProcessSpell;
        }

        private static void DrawingOnOnDraw(EventArgs args)
        {
            //Render.Circle.DrawCircle(ObjectManager.Player.Position, Orbwalking.GetRealAutoAttackRange(null) + 180, System.Drawing.Color.Red);
        }

        public static void Game_OnProcessSpell(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs spell)
        {
            if (!unit.IsMe)
            {
                return;
            }
            if (spell.SData.Name.Contains("summoner"))
            {
                return;
            }

            CastList.Add(spell.SData.Name.ToLower());
            if (CastList.Count >= 2)
            {

                List<string> lastTwo = CastList.Skip(CastList.Count - 2).ToList();
                CastList.Clear();
                CastList = lastTwo;

                if (spell.SData.Name.ToLower().Contains("attack") && !spell.SData.Name.ToLower().Contains("vieattack"))
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(250, () => AutoAttackCount = 1);
                }

                if (CastList.Count >= 2)
                {
                    FirstCastedSpell = CastList.Skip(CastList.Count - 2).FirstOrDefault();
                    SeconCastedSpell = CastList.Skip(CastList.Count - 1).FirstOrDefault();
                }

                if (SeconCastedSpell == "viq")
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(250, () => AutoAttackCount = 0);
                    Orbwalking.ResetAutoAttackTimer();
                }

                if (SeconCastedSpell == "vieattack")
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(250, () => AutoAttackCount = 0);
                    Orbwalking.ResetAutoAttackTimer();
                }
            }

            switch (spell.Slot)
            {
                case SpellSlot.R:
                {
                        LeagueSharp.Common.Utility.DelayAction.Add(10, () => LastSpellCastTick = Environment.TickCount);
                        LeagueSharp.Common.Utility.DelayAction.Add(10, () => AutoAttackCount = 0);
                        Orbwalking.ResetAutoAttackTimer();
                    break;
                }
            }
        }

        private static void GameOnOnUpdate(EventArgs args)
        {

        }

        public static void CastQObjects(Obj_AI_Base t)
        {
            if (!Q.CanCast(t))
            {
                return;
            }

            if (Q.IsCharging)
            {
                var qPrediction = Q.GetPrediction(t);
                var hithere = qPrediction.CastPosition.Extend(ObjectManager.Player.Position, -90);
                if (qPrediction.Hitchance >= HitChance.High)
                {
                    Q.Cast(hithere);
                }
            }
            else
            {
                Q.StartCharging();
            }
        }

        public static void CastSpellSlot(this Spell spell, Obj_AI_Base t)
        {
            switch (spell.Slot)
            {
                case SpellSlot.Q:
                {
                    CastQ(t);
                    break;
                }
                case SpellSlot.E:
                {
                    CastE(t);
                    break;
                }
                case SpellSlot.R:
                {
                    CastR(t);
                    break;
                }
            }
        }

        public static void CastQ(Obj_AI_Base t)
        {
            if (!Q.CanCast(t))
            {
                return;
            }

            if (!Common.CommonHelper.ShouldCastSpell(CommonTargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(null) + 65)))
            {
                return;
            }


            if (Modes.ModeCombo.MenuLocal.Item("Combo.Mode").GetValue<StringList>().SelectedIndex == 1
                && AutoAttackCount == 0
                && t.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 120) && !Q.IsCharging)
            {
                return;
            }

            //if (Modes.ModeCombo.MenuLocal.Item("Combo.Mode").GetValue<StringList>().SelectedIndex == 1 
            //    && LastAutoAttackTick < LastSpellCastTick 
            //    && t.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65))
            //{
            //    return;
            //}

            if (Q.IsCharging)
            {
                var qPrediction = Q.GetPrediction(t);
                var hithere = qPrediction.CastPosition.Extend(ObjectManager.Player.Position, -90);
                if (qPrediction.Hitchance >= HitChance.High)
                {
                    Q.Cast(hithere);
                }
            }
            else
            {
                Q.StartCharging();
            }
        }

        public static void CastE(Obj_AI_Base t)
        {
            if (!E.IsReady() || !t.IsValidTarget(E.Range))
            {
                return;
            }

            if (!Common.CommonHelper.ShouldCastSpell(CommonTargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(null) + 65)))
            {
                return;
            }

            if (Modes.ModeCombo.MenuLocal.Item("Combo.Mode").GetValue<StringList>().SelectedIndex == 1
                //&& LastAutoAttackTick < LastSpellCastTick
                && AutoAttackCount == 0
                && t.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + (t.HasViQKnockBack() ? 170 : 95)))
            {
                return;
            }

            if (CommonBuffs.ViHasEBuff)
            {
                Modes.ModeConfig.Orbwalker.ForceTarget(t);
                Modes.ModeConfig.Orbwalker.SetAttack(true);
            }
            else
            {
                E.Cast();
            }
        }

        public static void CastR(Obj_AI_Base t)
        {
            if (!R.CanCast(t))
            {
                return;
            }

            if (!Common.CommonHelper.ShouldCastSpell(CommonTargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(null) + 65)))
            {
                return;
            }

            if (Modes.ModeCombo.MenuLocal.Item("Combo.Mode").GetValue<StringList>().SelectedIndex == 1
                && AutoAttackCount == 0
                && t.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 120))
            {
                return;
            }

            if (CommonBuffs.ViHasEBuff && t.Health <= CommonMath.GetComboDamage(t))
            {
                R.CastOnUnit(t);
            }
        }
    }
}
