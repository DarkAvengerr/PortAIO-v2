using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using _Project_Geass.Drawing.Champions;
using _Project_Geass.Functions;
using _Project_Geass.Functions.Objects;
using _Project_Geass.Humanizer.TickTock;
using _Project_Geass.Module.Champions.Core;
using _Project_Geass.Module.Core.Mana.Functions;
using Damage = _Project_Geass.Functions.Calculations.Damage;
using Prediction = _Project_Geass.Functions.Prediction;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Module.Champions.Heroes.Events
{

    internal class Ashe : Base
    {
        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Ashe" /> class.
        /// </summary>
        /// <param name="manaEnabled">
        ///     if set to <c> true </c> [mana enabled].
        /// </param>
        /// <param name="orbwalker">
        ///     The orbwalker.
        /// </param>
        public Ashe(bool manaEnabled, Orbwalking.Orbwalker orbwalker)
        {
            Q=new Spell(SpellSlot.Q);
            W=new Spell(SpellSlot.W, 1200);
            R=new Spell(SpellSlot.R, 2200);

            W.SetSkillshot(.25f, 57.5f, 2000, true, SkillshotType.SkillshotCone);
            R.SetSkillshot(.25f, 250, 1600, false, SkillshotType.SkillshotLine);

            _manaManager=new Mana(Q, W, E, R, manaEnabled);
            // ReSharper disable once UnusedVariable
            var temp=new Menus.Ashe();

            Game.OnUpdate+=OnUpdate;

            EloBuddy.Drawing.OnDraw+=OnDraw;
            EloBuddy.Drawing.OnDraw+=OnDrawEnemy;
            AntiGapcloser.OnEnemyGapcloser+=OnGapcloser;

            _damageIndicator=new DamageIndicator(GetDamage, 2000);
            Orbwalker=orbwalker;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        ///     Raises the <see cref="E:DrawEnemy" /> event.
        /// </summary>
        /// <param name="args">
        ///     The <see cref="EventArgs" /> instance containing the event data.
        /// </param>
        public void OnDrawEnemy(EventArgs args)
        {
            if (!StaticObjects.ProjectMenu.Item(Names.Menu.DrawingItemBase+StaticObjects.Player.ChampionName+".Boolean.DrawOnEnemy.ComboDamage").GetValue<Circle>().Active)
            {
                _damageIndicator.SetFillEnabled(false);
                _damageIndicator.SetKillableEnabled(false);
                return;
            }

            _damageIndicator.SetFillEnabled(true);
            _damageIndicator.SetFill(StaticObjects.ProjectMenu.Item(Names.Menu.DrawingItemBase+StaticObjects.Player.ChampionName+".Boolean.DrawOnEnemy.ComboDamage").GetValue<Circle>().Color);

            _damageIndicator.SetKillableEnabled(false);
        }

        #endregion Public Methods

        #region Private Fields

        private readonly DamageIndicator _damageIndicator;
        private readonly Mana _manaManager;

        /// <summary>
        ///     On Clear
        /// </summary>
        // ReSharper disable once NotAccessedField.Local
        private int _minonsHit;

        #endregion Private Fields

        #region Private Methods

        private void Clear()
        {
            var basename=BaseName+"Clear.";

            var validMinions=Minions.GetEnemyMinions2(W.Range);

            if (StaticObjects.ProjectMenu.Item($"{basename}.UseW").GetValue<bool>())
                if (_manaManager.CheckClearW())
                {
                    var pos=W.GetLineFarmLocation(validMinions);
                    _minonsHit=pos.MinionsHit;
                    if (pos.MinionsHit>=StaticObjects.ProjectMenu.Item($"{basename}.UseW.Minions").GetValue<Slider>().Value)
                        W.Cast(pos.Position);
                }

            if (StaticObjects.ProjectMenu.Item($"{basename}.UseQ").GetValue<bool>())

                if (_manaManager.CheckClearQ())
                {
                    var aaMinons=validMinions.Where(x => x.Distance(StaticObjects.Player)<StaticObjects.Player.AttackRange);

                    if (aaMinons.Count()>=StaticObjects.ProjectMenu.Item($"{basename}.UseQ.Minions").GetValue<Slider>().Value)
                        Q.Cast();
                }
        }

        /// <summary>
        ///     On Combo
        /// </summary>
        private void Combo()
        {
            var basename=BaseName+"Combo.";

            if (StaticObjects.ProjectMenu.Item($"{basename}.UseQ").GetValue<bool>())
                if (_manaManager.CheckComboQ())
                    if (Functions.Objects.Heroes.GetEnemies(StaticObjects.Player.AttackRange-30).Count>0)
                        Q.Cast();

            if (StaticObjects.ProjectMenu.Item($"{basename}.UseW").GetValue<bool>())
                if (_manaManager.CheckComboW())
                {
                    var minHitChance=Prediction.GetHitChance(StaticObjects.ProjectMenu.Item($"{basename}.UseW.Prediction").GetValue<StringList>().SelectedValue);

                    var focusTargetValid=false;
                    //Check if the target in target selector is valid (best target)
                    var focusTarget=TargetSelector.GetTarget(W.Range, W.DamageType);
                    if (focusTarget!=null)
                        if (StaticObjects.ProjectMenu.Item($"{basename}.UseW.On.{focusTarget.ChampionName}").GetValue<bool>())
                            focusTargetValid=Prediction.DoCast(W, focusTarget, minHitChance);
                    if (!focusTargetValid)
                    {
                        var orderedTargets=Prediction.OrderTargets(W);

                        foreach (var target in orderedTargets)
                        {
                            if (StaticObjects.ProjectMenu.Item($"{basename}.UseW.On.{target.ChampionName}").GetValue<bool>())
                                continue;

                            if (Prediction.DoCast(W, target, minHitChance))
                                break;
                        }
                    }
                }

            if (StaticObjects.ProjectMenu.Item($"{basename}.UseR").GetValue<bool>())
                if (_manaManager.CheckComboR())
                {
                    var minHitChance=Prediction.GetHitChance(StaticObjects.ProjectMenu.Item($"{basename}.UseR.Prediction").GetValue<StringList>().SelectedValue);

                    var focusTargetValid=false;
                    //Check if the target in target selector is valid (best target)
                    var focusTarget=TargetSelector.GetTarget(R.Range, R.DamageType);
                    if (focusTarget!=null)
                        if (StaticObjects.ProjectMenu.Item($"{basename}.UseR.On.{focusTarget.ChampionName}").GetValue<bool>())
                            if (StaticObjects.ProjectMenu.Item($"{basename}.UseR.On.{focusTarget.ChampionName}.HpMin").GetValue<Slider>().Value>focusTarget.HealthPercent)
                                if (StaticObjects.ProjectMenu.Item($"{basename}.UseR.On.{focusTarget.ChampionName}.HpMax").GetValue<Slider>().Value<focusTarget.HealthPercent)
                                    focusTargetValid=Prediction.DoCast(R, focusTarget, minHitChance);

                    if (!focusTargetValid)
                    {
                        var orderedTargets=Prediction.OrderTargets(R);

                        foreach (var target in orderedTargets)
                        {
                            if (!StaticObjects.ProjectMenu.Item($"{basename}.UseR.On.{target.ChampionName}").GetValue<bool>())
                                continue;
                            if (StaticObjects.ProjectMenu.Item($"{basename}.UseR.On.{target.ChampionName}.HpMin").GetValue<Slider>().Value>target.HealthPercent)
                                continue;
                            if (StaticObjects.ProjectMenu.Item($"{basename}.UseR.On.{target.ChampionName}.HpMax").GetValue<Slider>().Value<target.HealthPercent)
                                continue;

                            if (Prediction.DoCast(R, target, minHitChance))
                                break;
                        }
                    }
                }
        }

        /// <summary>
        ///     Returns estimated damage
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <returns>
        /// </returns>
        private float GetDamage(AIHeroClient target)
        {
            var damage=0f;
            if ((target.Distance(StaticObjects.Player)<StaticObjects.Player.AttackRange-25)&&StaticObjects.Player.CanAttack&&!StaticObjects.Player.Spellbook.IsAutoAttacking)
                damage+=(float)StaticObjects.Player.GetAutoAttackDamage(target)-10;

            if (W.IsReady())
                damage+=W.GetDamage(target);

            if (R.IsReady())
                damage+=R.GetDamage(target);

            return Damage.CalcRealDamage(target, damage);
        }

        /// <summary>
        ///     On Mixed
        /// </summary>
        private void Mixed()
        {
            var basename=BaseName+"Mixed.";

            if (StaticObjects.ProjectMenu.Item($"{basename}.UseW").GetValue<bool>())
                if (_manaManager.CheckMixedW())
                {
                    var minHitChance=Prediction.GetHitChance(StaticObjects.ProjectMenu.Item($"{basename}.UseW.Prediction").GetValue<StringList>().SelectedValue);

                    var focusTargetValid=false;
                    //Check if the target in target selector is valid (best target)
                    var focusTarget=TargetSelector.GetTarget(W.Range, W.DamageType);

                    if (focusTarget!=null)
                        if (StaticObjects.ProjectMenu.Item($"{basename}.UseW.On.{focusTarget.ChampionName}").GetValue<bool>())
                            focusTargetValid=Prediction.DoCast(W, focusTarget, minHitChance);
                    if (!focusTargetValid)
                    {
                        var orderedTargets=Prediction.OrderTargets(W);

                        foreach (var target in orderedTargets)
                        {
                            if (StaticObjects.ProjectMenu.Item($"{basename}.UseW.On.{target.ChampionName}").GetValue<bool>())
                                continue;

                            if (Prediction.DoCast(W, target, minHitChance))
                                break;
                        }
                    }
                }

            if (StaticObjects.ProjectMenu.Item($"{basename}.UseQ").GetValue<bool>())
                if (_manaManager.CheckMixedQ())
                    if (Functions.Objects.Heroes.GetEnemies(StaticObjects.Player.AttackRange-50).Count>=1)
                        Q.Cast();
        }

        /// <summary>
        ///     Raises the <see cref="E:Draw" /> event.
        /// </summary>
        /// <param name="args">
        ///     The <see cref="EventArgs" /> instance containing the event data.
        /// </param>
        private void OnDraw(EventArgs args)
        {
            //var heroPosition = EloBuddy.Drawing.WorldToScreen(StaticObjects.Player.Position);
            // EloBuddy.Drawing.DrawText(heroPosition.X + 20, heroPosition.Y - 30, System.Drawing.Color.MintCream, minonsHit.ToString());

            if (W.Level>0)
                if (StaticObjects.ProjectMenu.Item(Names.Menu.DrawingItemBase+StaticObjects.Player.ChampionName+".Boolean.DrawOnSelf.WColor").GetValue<Circle>().Active)
                    Render.Circle.DrawCircle(StaticObjects.Player.Position, W.Range, StaticObjects.ProjectMenu.Item(Names.Menu.DrawingItemBase+StaticObjects.Player.ChampionName+".Boolean.DrawOnSelf.WColor").GetValue<Circle>().Color, 2);
            if (R.Level>0)
                if (StaticObjects.ProjectMenu.Item(Names.Menu.DrawingItemBase+StaticObjects.Player.ChampionName+".Boolean.DrawOnSelf.RColor").GetValue<Circle>().Active)
                    Render.Circle.DrawCircle(StaticObjects.Player.Position, R.Range, StaticObjects.ProjectMenu.Item(Names.Menu.DrawingItemBase+StaticObjects.Player.ChampionName+".Boolean.DrawOnSelf.RColor").GetValue<Circle>().Color, 2);
        }

        /// <summary>
        ///     Called when [gapcloser].
        /// </summary>
        /// <param name="gapcloser">
        ///     The gapcloser.
        /// </param>
        private void OnGapcloser(ActiveGapcloser gapcloser)
        {
            var basename=BaseName+"Auto.";

            if (R.IsReady())
                if (StaticObjects.ProjectMenu.Item($"{basename}.UseR.OnGapClose").GetValue<bool>())
                    if (StaticObjects.ProjectMenu.Item($"{basename}.UseR.OnGapClose.{gapcloser.Sender.ChampionName}").GetValue<bool>())
                        if (gapcloser.End.Distance(ObjectManager.Player.Position)<300)
                            if (StaticObjects.ProjectMenu.Item($"{Names.Menu.BaseItem}.Humanizer").GetValue<bool>())
                            {
                                if (gapcloser.Sender.HasBuffOfType(BuffType.Invulnerability)||gapcloser.Sender.HasBuffOfType(BuffType.SpellImmunity)||gapcloser.Sender.HasBuffOfType(BuffType.SpellShield))
                                    return;

                                LeagueSharp.Common.Utility.DelayAction.Add(Math.Abs(Rng.Next()*(150-50-Game.Ping)+50-Game.Ping), () => {R.Cast(gapcloser.End);});
                            }
                            else
                                R.Cast(gapcloser.End);

            if (W.IsReady())
                if (StaticObjects.ProjectMenu.Item($"{basename}.UseW.OnGapClose").GetValue<bool>())
                    if (StaticObjects.ProjectMenu.Item($"{basename}.UseW.OnGapClose.{gapcloser.Sender.ChampionName}").GetValue<bool>())
                        if (gapcloser.End.Distance(ObjectManager.Player.Position)<200)
                            if (StaticObjects.ProjectMenu.Item($"{Names.Menu.BaseItem}.Humanizer").GetValue<bool>())
                            {
                                if (gapcloser.Sender.HasBuffOfType(BuffType.Invulnerability)||gapcloser.Sender.HasBuffOfType(BuffType.SpellImmunity)||gapcloser.Sender.HasBuffOfType(BuffType.SpellShield))
                                    return;

                                LeagueSharp.Common.Utility.DelayAction.Add(Math.Abs(Rng.Next()*(200-100-Game.Ping)+100-Game.Ping), () => {W.Cast(gapcloser.End);});
                            }
                            else
                                W.Cast(gapcloser.End);
        }

        /// <summary>
        ///     Raises the <see cref="E:Update" /> event.
        /// </summary>
        /// <param name="args">
        ///     The <see cref="EventArgs" /> instance containing the event data.
        /// </param>
        private void OnUpdate(EventArgs args)
        {
            if (!Handler.CheckOrbwalker())
                return;

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                {
                    Combo();
                    break;
                }
                case Orbwalking.OrbwalkingMode.Mixed:
                {
                    Mixed();
                    break;
                }
                case Orbwalking.OrbwalkingMode.LaneClear:
                {
                    Clear();
                    break;
                }
            }

            Handler.UseOrbwalker();
        }

        #endregion Private Methods
    }

}