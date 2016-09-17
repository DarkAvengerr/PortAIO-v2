using System;
using DZLib.Logging;
using LeagueSharp;
using LeagueSharp.Common;
using SoloVayne.Modules;
using SoloVayne.Utility;
using SoloVayne.Utility.Enums;
using SoloVayne.Utility.General;
using SOLOVayne.Skills.Tumble.WardTracker;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloVayne
{
    class SOLOVayne
    {
        /**
         * TODO List
         * Safe enemies around check for Q into Wall
         * Don't aa while stealthed should be on I guess with 3 enemies, but if you have an ally near it shouldn't aa with 2. Maybe it should just always stealth.
         * Add Condemn To Trundle / J4 / Anivia Walls
         * Q Away if targetted from turret and no killable low health enemy is near.
         */
        private float lastTick = 0f;

        /// <summary>
        /// Initializes a new instance of the <see cref="SOLOVayne"/> class.
        /// </summary>
        public SOLOVayne()
        {
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Obj_AI_Base.OnProcessSpellCast += YasuoWall.OnProcessSpellCast;

            Obj_AI_Base.OnProcessSpellCast += WardDetector.OnProcessSpellCast;
            GameObject.OnCreate += WardDetector.OnCreate;
            GameObject.OnDelete += WardDetector.OnDelete;

            foreach (var module in Variables.ModuleList)
            {
                module.OnLoad();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Draw" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnDraw(EventArgs args)
        {}

        /// <summary>
        /// Called when an unit has executed the windup time for a skill.
        /// </summary>
        /// <param name="sender">The unit.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs"/> instance containing the event data.</param>
        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                if (sender.IsMe 
                    && Orbwalking.IsAutoAttack(args.SData.Name) 
                    && (args.Target is Obj_AI_Base))
                {
                        foreach (var skill in Variables.skills)
                        {
                            if (skill.GetSkillMode() == SkillMode.OnAfterAA)
                            {
                                if ((Variables.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo ||
                                     Variables.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed))
                                {
                                    skill.Execute(args.Target as Obj_AI_Base);
                                }

                                if (Variables.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                                {
                                    skill.ExecuteFarm(args.Target as Obj_AI_Base);
                                }
                            }
                        }      
                }

            }catch (Exception e)
            {
                LogHelper.AddToLog(new LogItem("OnSpellCast", e, LogSeverity.Error));
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Update" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnUpdate(EventArgs args)
        {
            try
            {
                if (Environment.TickCount - this.lastTick < 80)
                {
                    return;
                }
                this.lastTick = Environment.TickCount;

                WardDetector.OnTick();

                if (ObjectManager.Player.IsDead)
                {
                    return;
                }

                foreach (var skill in Variables.skills)
                {
                    if (skill.GetSkillMode() == SkillMode.OnUpdate)
                    {
                        skill.Execute(Variables.Orbwalker.GetTarget() is Obj_AI_Base ? Variables.Orbwalker.GetTarget() as Obj_AI_Base : null);
                    }
                }

                foreach (var module in Variables.ModuleList)
                {
                    if (module.ShouldGetExecuted() && module.GetModuleType() == ModuleType.OnUpdate)
                    {
                        module.OnExecute();
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.AddToLog(new LogItem("OnUpdate", e, LogSeverity.Error));
            }
        }
    }
}
