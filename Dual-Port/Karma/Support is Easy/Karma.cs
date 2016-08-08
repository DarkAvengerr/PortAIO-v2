using EloBuddy; namespace Support.Plugins
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Support.Util;

    using ActiveGapcloser = LeagueSharp.Common.ActiveGapcloser;

    public class Karma : PluginBase
    {
        public Karma()
        {
            this.Q = new Spell(SpellSlot.Q, 1050);
            this.W = new Spell(SpellSlot.W, 700);
            this.E = new Spell(SpellSlot.E, 800);
            this.R = new Spell(SpellSlot.R, 0);

            this.Q.SetSkillshot(0.25f, 60f, 1700f, true, SkillshotType.SkillshotLine);
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("Combo.Q", "Use Q", true);
            config.AddBool("Combo.W", "Use W", true);
        }

        public override void HarassMenu(Menu config)
        {
            config.AddBool("Harass.Q", "Use Q", true);
        }

        public override void InterruptMenu(Menu config)
        {
            config.AddBool("Gapcloser.W", "Use W to Interrupt Gapcloser", true);
        }

        public override void MiscMenu(Menu config)
        {
            config.AddSlider("Misc.Q.Count", "R/Q Enemy in Range", 2, 0, 4);
            config.AddSlider("Misc.W.Hp", "R/W HP", 40, 1, 100);
            config.AddSlider("Misc.E.Count", "R/E Ally in Range", 3, 0, 4);
        }

        public override void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly)
            {
                return;
            }

            if (this.W.CastCheck(gapcloser.Sender, "Gapcloser.W"))
            {
                this.W.CastOnUnit(gapcloser.Sender);
            }
        }

        public override void OnUpdate(EventArgs args)
        {
            if (this.ComboMode)
            {
                if (this.Q.CastCheck(this.Target, "Combo.Q") && this.R.LSIsReady()
                    && this.Q.GetPrediction(this.Target).Hitchance >= HitChance.High
                    && this.Q.GetPrediction(this.Target).CollisionObjects.Count == 0
                    && this.Q.GetPrediction(this.Target).UnitPosition.LSCountEnemiesInRange(250)
                    >= this.ConfigValue<Slider>("Misc.Q.Count").Value)
                {
                    this.R.CastOnUnit(this.Player);
                    LeagueSharp.Common.Utility.DelayAction.Add(200, () => this.Q.Cast(this.Target));
                }
                if (this.Q.CastCheck(this.Target, "Combo.Q"))
                {
                    this.Q.Cast(this.Target);
                }

                if (this.W.CastCheck(this.Target, "Combo.W") && this.R.LSIsReady()
                    && this.Player.HealthPercent <= this.ConfigValue<Slider>("Misc.W.Hp").Value)
                {
                    this.R.CastOnUnit(this.Player);
                    LeagueSharp.Common.Utility.DelayAction.Add(200, () => this.W.CastOnUnit(this.Target));
                }
                if (this.W.CastCheck(this.Target, "Combo.W"))
                {
                    this.W.CastOnUnit(this.Target);
                }

                if (this.E.LSIsReady() && this.R.LSIsReady()
                    && Helpers.AllyInRange(600).Count >= this.ConfigValue<Slider>("Misc.E.Count").Value)
                {
                    this.R.CastOnUnit(this.Player);
                    LeagueSharp.Common.Utility.DelayAction.Add(200, () => this.E.CastOnUnit(this.Player));
                }
            }

            if (this.HarassMode)
            {
                if (this.Q.CastCheck(this.Target, "Harass.Q") && this.R.LSIsReady()
                    && this.Q.GetPrediction(this.Target).Hitchance >= HitChance.High
                    && this.Q.GetPrediction(this.Target).CollisionObjects.Count == 0
                    && this.Q.GetPrediction(this.Target).UnitPosition.LSCountEnemiesInRange(250)
                    >= this.ConfigValue<Slider>("Misc.Q.Count").Value)
                {
                    this.R.CastOnUnit(this.Player);
                    LeagueSharp.Common.Utility.DelayAction.Add(200, () => this.Q.Cast(this.Target));
                }
                if (this.Q.CastCheck(this.Target, "Harass.Q"))
                {
                    this.Q.Cast(this.Target);
                }

                if (this.E.LSIsReady() && this.R.LSIsReady()
                    && Helpers.AllyInRange(600).Count >= this.ConfigValue<Slider>("Misc.E.Count").Value)
                {
                    this.R.CastOnUnit(this.Player);
                    LeagueSharp.Common.Utility.DelayAction.Add(200, () => this.E.CastOnUnit(this.Player));
                }
            }
        }
    }
}