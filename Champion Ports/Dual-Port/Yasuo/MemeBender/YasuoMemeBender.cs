using System;
using LeagueSharp;
using LeagueSharp.Common;
using YasuoTheLastMemebender.Skills;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoTheLastMemebender
{
    /*internal enum YasuoSpell
    {
        Q,
        QEmp,
        QDash,
        W,
        E,
        R
    }*/

    internal class YasuoMemeBender
    {
        private bool doingEQCombo;
        /* private static readonly Dictionary<YasuoSpell, Spell> Spells = new Dictionary<YasuoSpell, Spell>
        {
            {YasuoSpell.Q, new Spell(SpellSlot.Q, 520f)},
            {YasuoSpell.QEmp, new Spell(SpellSlot.Q, 1000f)},
            {YasuoSpell.QDash, new Spell(SpellSlot.Q, 0f)},
            {YasuoSpell.W, new Spell(SpellSlot.W, 400f)},
            {YasuoSpell.E, new Spell(SpellSlot.E, 475)},
            {YasuoSpell.R, new Spell(SpellSlot.R, 1300)}
        };*/

        public int Knockup = 0;

        public YasuoMemeBender()
        {
            if (ObjectManager.Player.ChampionName != "Yasuo")
            {
                return;
            }
            OnLoad();
        }

        public void SetUpEvents()
        {
            new Config();
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnWndProc += Game_OnWndProc; //Changing laneclear with scroll wheel
            Obj_AI_Base.OnProcessSpellCast += WindWall.Game_ProcessSpell;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            //AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            CustomizableAntiGapcloser.OnEnemyCustomGapcloser += OnEnemyCustomGapcloser;
            /*Obj_AI_Base.OnBuffGain += OnBuffAdd;
            Obj_AI_Base.OnBuffLose += OnBuffLose;*/
        }

        public void SetUpSkills()
        {
            SteelTempest.Q.SetSkillshot(0.4f, 20f, float.MaxValue, false, SkillshotType.SkillshotLine);
            SteelTempest.QEmp.SetSkillshot(0.4f, 90f, 1000f, false, SkillshotType.SkillshotLine);
            SteelTempest.QDash.SetSkillshot(0.4f, 250f, int.MaxValue, false, SkillshotType.SkillshotCircle,
                ObjectManager.Player.ServerPosition);
            SweepingBlade.E.SetTargetted(0f, 600);
            LastBreath.R.SetTargetted(0f, float.MaxValue);
        }

        public void OnLoad()
        {
            SetUpEvents();
            SetUpSkills();
            Chat.Print("Loaded Yasuo - The Last MemeBender");
        }

        public void Game_OnGameUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            if (Config.Param<bool>("ylm.spellr.auto"))
            {
                LastBreath.AutoUltimate();
            }

            switch (Config.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed: //Harass
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    break;
                case Orbwalking.OrbwalkingMode.CustomMode:
                    if (Config.Param<KeyBind>("ylm.orbwalker.escape").Active)
                    {
                        Escape();
                    }
                    break;
            }
        }

        public void Drawing_OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
            }
            /*var bestUnit = YasuoUtils.BestQDashKnockupUnit();
            if (bestUnit != null)
            {
                Drawing.DrawCircle(bestUnit.Position, bestUnit.AttackRange, Color.Red);
            }*/
            //Draw Stuff :^ )
        }

        private void OnEnemyCustomGapcloser(CActiveCGapcloser args)
        {
            if (!SteelTempest.Empowered || !SteelTempest.Q.IsReady())
                return;
            SteelTempest.CastQ(args.Sender);
        }

        private void Interrupter2_OnInterruptableTarget(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!Config.Param<bool>("ylm.anti.interrupt.useq3") ||
                !Config.Param<bool>(string.Format("ylm.anti.interrupt.{0}", unit.ChampionName)))
                return;

            if (!SteelTempest.Empowered || !SteelTempest.Q.IsReady())
                return;

            SteelTempest.CastQ(unit);
        }

        /*private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!Config.Param<bool>("ylm.anti.gapclose.useq3"))
                return;

            if (!SteelTempest.Empowered || !SteelTempest.Q.IsReady())
                return;
            SteelTempest.CastQ(gapcloser.Sender);
        }*/

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg != 0x20a || !Config.Param<bool>("ylm.laneclear.changeWithScroll"))
                return;

            Config.Menu.Item("ylm.laneclear.enabled").SetValue(!Config.Param<bool>("ylm.laneclear.enabled"));
        }

        public void Combo()
        {
            if (doingEQCombo)
            {
                if (ObjectManager.Player.IsDashing()
                    && SteelTempest.Q.IsReady() && SteelTempest.Empowered &&
                    ObjectManager.Player.CountEnemiesInRange(250) > 0)
                {
                    SteelTempest.QDash.Cast();
                    doingEQCombo = false;
                }
                if (doingEQCombo)
                {
                    return;
                }
            }
            var comboTarget = TargetSelector.GetTarget(LastBreath.R.Range, TargetSelector.DamageType.Physical);

            if (comboTarget != null && comboTarget.IsValid)
            {
                if (YasuoUtils.DecideKnockup(comboTarget))
                {
                    var bestUnit = YasuoUtils.BestQDashKnockupUnit();
                    if (bestUnit != null)
                    {
                        doingEQCombo = true;
                        SweepingBlade.E.Cast(bestUnit);
                        LeagueSharp.Common.Utility.DelayAction.Add(100, () => { doingEQCombo = false; });
                    }
                }
                else
                {
                    SweepingBlade.GapClose(comboTarget);
                }
            }
            else
            {
                SweepingBlade.GapClose();
                return;
            }

            var targetDistance = comboTarget.Distance(ObjectManager.Player, true);
            if (Config.Param<bool>("ylm.combo.useq") && SteelTempest.Q.IsReady() &&
                targetDistance <= SteelTempest.Q.RangeSqr)
            {
                SteelTempest.Q.Cast(comboTarget);
            }

            if (Config.Param<bool>("ylm.combo.useq3") && SteelTempest.Empowered && SteelTempest.Q.IsReady() &&
                targetDistance <= SteelTempest.QEmp.RangeSqr)
            {
                SteelTempest.QEmp.Cast(comboTarget);
            }

            if (Config.Param<bool>("ylm.combo.user") && LastBreath.ShouldUlt(comboTarget))
            {
                LastBreath.CastR(comboTarget);
            }
            //TODO: Items
        }

        public void Harass()
        {
            var qRange = SteelTempest.Empowered ? SteelTempest.QEmp.Range : SteelTempest.Q.Range;
            var useQ = (SteelTempest.Empowered && Config.Param<bool>("ylm.mixed.useq3"))
                       || (!SteelTempest.Empowered && Config.Param<bool>("ylm.mixed.useq"));

            var eTarget = TargetSelector.GetTarget(SweepingBlade.E.Range, TargetSelector.DamageType.Magical);
            var qTarget = TargetSelector.GetTarget(qRange, TargetSelector.DamageType.Physical);

            if (useQ && qTarget != null && SteelTempest.Q.IsReady())
            {
                if (ObjectManager.Player.IsDashing() &&
                    ObjectManager.Player.Distance(qTarget, true) < SteelTempest.QDash.WidthSqr)
                {
                    SteelTempest.QDash.Cast();
                }
                else
                {
                    if (SteelTempest.Empowered)
                    {
                        SteelTempest.QEmp.Cast(qTarget);
                    }
                    else
                    {
                        SteelTempest.Q.Cast(qTarget);
                    }
                }
            }

            if (eTarget != null && Config.Param<bool>("ylm.mixed.usee") && SweepingBlade.CanCastE(eTarget))
            {
                if (Config.Param<Slider>("ylm.mixed.mode").Value == 0)
                {
                    SweepingBlade.E.Cast(eTarget);
                }
                else
                {
                    if (!SteelTempest.Q.IsReady() && eTarget.Distance(ObjectManager.Player, true) <= 200*200)
                        //Run away we useless :^(
                    {
                        SweepingBlade.RunAway(eTarget);
                    }
                    else if (SteelTempest.Q.IsReady())
                    {
                        SweepingBlade.E.Cast(eTarget);
                    }
                }
            }
            if (Config.Param<bool>("ylm.mixed.lasthit"))
            {
                SteelTempest.LastHitQ();
                SweepingBlade.LaneE(true);
            }
        }

        public void LastHit()
        {
            if (Config.Param<bool>("ylm.lasthit.enabled"))
            {
                SteelTempest.LastHitQ();
                SweepingBlade.LaneE(true);
            }
        }

        public void Clear()
        {
            if (Config.Param<bool>("ylm.laneclear.enabled"))
            {
                SteelTempest.ClearQ();
                SweepingBlade.LaneE();
            }
            if (Config.Param<bool>("ylm.jungleclear.enabled"))
            {
                SteelTempest.ClearQ(true);
                SweepingBlade.JungleClearE();
            }
        }

        public void Escape()
        {
            var escapeTarget = YasuoUtils.ClosestMinion(Game.CursorPos);
            if (escapeTarget != null)
            {
                SweepingBlade.GapClose(escapeTarget, true);
            }
            if (SteelTempest.Empowered && SteelTempest.Q.IsReady() && !ObjectManager.Player.IsDashing())
            {
                var q3Target = TargetSelector.GetTarget(SteelTempest.QEmp.Range, TargetSelector.DamageType.Physical);
                if (q3Target != null)
                {
                    SteelTempest.QEmp.Cast(q3Target);
                }
            }
        }
    }
}