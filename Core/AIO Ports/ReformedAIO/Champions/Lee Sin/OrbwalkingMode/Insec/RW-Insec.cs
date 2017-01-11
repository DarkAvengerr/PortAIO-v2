using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lee_Sin.OrbwalkingMode.Insec
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lee_Sin.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    using SharpDX;

    using HitChance = SebbyLib.Movement.HitChance;

    internal sealed class RwInsec : OrbwalkingChild
    {
        public override string Name { get; set; } = "Insec";

        private readonly WSpell wSpell;

        private readonly RSpell rSpell;

        private readonly QSpell qSpell;

        public RwInsec(WSpell wSpell, RSpell rSpell, QSpell qSpell)
        {
            this.wSpell = wSpell;
            this.rSpell = rSpell;
            this.qSpell = qSpell;
        }

#region Fields & Properties

        private bool JustPlacedWard => Utils.TickCount - wSpell.wardTick < 5000;

        private bool CanWardJump
            => wSpell.Trinket
            && wSpell.Spell.IsReady()
            && wSpell.W1
            && GetInsecPosition().Distance(ObjectManager.Player.ServerPosition) <= WardjumpRange + 100
            && Utils.TickCount - wSpell.jumpTick > 3000;

        private int gapcloseTick = 0;

        private const int FlashRange = 425;

        private const int WardjumpRange = 600;

        private const int WardJumpFlashRange = 1125;

        private int JumpRange()
        {
            return CanWardJump ? WardjumpRange : FlashRange;
        }

        private static Obj_AI_Base BubbaKushTargets
            =>
                HeroManager.Enemies.Where(x => x != Target && x.IsValidTarget(1300) && x.Distance(Target) < 750)
                    .OrderBy(x => x.Distance(Target)).FirstOrDefault();

        private Obj_AI_Base GapcloseTargets
             => HeroManager.Enemies
            .Where(x => x != Target)
            .Concat(MinionManager.GetMinions(ObjectManager.Player.Position, qSpell.Spell.Range))
            .LastOrDefault(x => x.Distance(Target) <= 750);

        private static AIHeroClient Target => TargetSelector.GetSelectedTarget();

        private bool WardFlash()
        {
            return Menu.Item("LeeSin.Insec.W.Gapclose").GetValue<bool>() && (!qSpell.Dashing
                   || (Target.Distance(GetInsecPosition()) > WardjumpRange
                    && Target.Distance(GetInsecPosition()) < WardJumpFlashRange + 100));
        }

        private bool CanFlashKick()
        {
            return Menu.Item("LeeSin.Insec.R.Kick").GetValue<bool>()
                   && rSpell.Flash.IsReady()
                   && GetInsecPosition().Distance(ObjectManager.Player.ServerPosition) < FlashRange
                   && (!JustPlacedWard || GetInsecPosition().Distance(ObjectManager.Player.ServerPosition) > 400);
        }

        private Vector3 BubbaKush()
        {
            return this.rSpell.MultipleInsec(Target, BubbaKushTargets);
        }

        private Vector3 InsecSolo()
        {
           return this.wSpell.InsecPositioner(
               Target,
               Menu.Item("LeeSin.Insec.W.Turret").GetValue<bool>(),
               Menu.Item("LeeSin.Insec.W.InsecAllies").GetValue<bool>());
        }

        private Vector3 GetInsecPosition()
        {
            if (!Menu.Item("LeeSin.Insec.R.Kush").GetValue<KeyBind>().Active)
            {
                return InsecSolo();
            }
            return BubbaKushTargets != null
                ? this.BubbaKush()
                : this.InsecSolo();
        }

        #endregion

#region Methods
        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians() || Target == null)
            {
                return;
            }

            if (qSpell.Spell.IsReady())
            {
                if (Target.IsValidTarget(qSpell.Spell.Range))
                {
                    HandleQ();
                }

                if (Menu.Item("LeeSin.Insec.Q.Minion").GetValue<bool>() && GapcloseTargets != null)
                {
                    if (qSpell.IsQ1 && GapcloseTargets.HealthPercent >= 50)
                    {
                        qSpell.Spell.Cast(GapcloseTargets);
                    }

                    if (!qSpell.IsQ1)
                    {
                        qSpell.Spell.Cast(GapcloseTargets);
                    }
                }
            }

            if (!rSpell.Spell.IsReady())
            {
                return;
            }

            if (WardFlash())
            {
                if (CanWardJump)
                {
                    wSpell.Jump(GetInsecPosition(), false, false, true);
                }
                else if (CanFlashKick() && (!JustPlacedWard || Target.Distance(ObjectManager.Player) > 500))
                {
                    rSpell.Spell.CastOnUnit(Target);
                    LeagueSharp.Common.Utility.DelayAction.Add(80, () =>
                    ObjectManager.Player.Spellbook.CastSpell(rSpell.Flash, GetInsecPosition()));
                }
            }

            if (CanWardJump)
            {
                wSpell.Jump(GetInsecPosition(), false, false, true);
            }
            else if (CanFlashKick() && !JustPlacedWard)
            {
                if (Target.Distance(ObjectManager.Player) < 375)
                {
                    rSpell.Spell.CastOnUnit(Target);
                    LeagueSharp.Common.Utility.DelayAction.Add(80, () => 
                    ObjectManager.Player.Spellbook.CastSpell(rSpell.Flash, GetInsecPosition()));
                }
                else
                {
                    rSpell.Spell.CastOnUnit(Target);
                    ObjectManager.Player.Spellbook.CastSpell(rSpell.Flash, GetInsecPosition());
                }
            }

            if (Utils.TickCount - wSpell.wardTick > 100 && Utils.TickCount - wSpell.wardTick < 1000)
            {
                rSpell.Spell.CastOnUnit(Target);
            }
        }

        private void HandleQ()
        {
            if (qSpell.IsQ1)
            {
                qSpell.SmiteCollision(Target);

                var prediction = qSpell.Prediction(Target);

                switch (Menu.Item("LeeSin.Insec.Q.Hitchance").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        if (prediction.Hitchance >= HitChance.High)
                        {
                            qSpell.Spell.Cast(prediction.CastPosition);
                        }
                        break;
                    case 1:
                        if (prediction.Hitchance >= HitChance.VeryHigh)
                        {
                            qSpell.Spell.Cast(prediction.CastPosition);
                        }
                        break;
                }
            }
            else
            {
                qSpell.Spell.Cast();
            }
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!CheckGuardians()
                || Target == null
                || !sender.IsMe
                || !args.SData.IsAutoAttack()
                || wSpell.W1)
            {
                return;
            }

            wSpell.Spell.Cast();
        }
#endregion
        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Game.OnUpdate -= OnUpdate;

            Obj_AI_Base.OnSpellCast -= OnSpellCast;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Game.OnUpdate += OnUpdate;

            Obj_AI_Base.OnSpellCast += OnSpellCast;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("LeeSin.Insec.R.Kush", "BubbaKush").SetValue(new KeyBind('N', KeyBindType.Toggle)));

            Menu.AddItem(new MenuItem("LeeSin.Insec.W.Gapclose", "Gapclose Ward").SetValue(false).SetTooltip("W -> R Flash"));

            Menu.AddItem(new MenuItem("LeeSin.Insec.R.Kick", "Flash Kick").SetValue(true));

            Menu.AddItem(new MenuItem("LeeSin.Insec.W.Turret", "Insec To: Turret").SetValue(true));

            Menu.AddItem(new MenuItem("LeeSin.Insec.W.InsecAllies", "Insec To: Allies").SetValue(true));

            Menu.AddItem(new MenuItem("LeeSin.Insec.W.Ward", "Jump On: Ward").SetValue(true));

            Menu.AddItem(new MenuItem("LeeSin.Insec.Q.Minion", "Q To: Minions").SetValue(true));

            Menu.AddItem(new MenuItem("LeeSin.Insec.Q.Hitchance", "Q Hitchance").SetValue(new StringList(new[] { "High", "Very High" })));
        }
    }
}