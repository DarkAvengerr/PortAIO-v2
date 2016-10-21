#region

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using _Project_Geass.Drawing.Champions;
using _Project_Geass.Functions;
using _Project_Geass.Functions.Objects;
using _Project_Geass.Humanizer.TickTock;
using _Project_Geass.Module.Champions.Core;
using _Project_Geass.Module.Core.Mana.Functions;
using Damage = _Project_Geass.Functions.Calculations.Damage;
using ItemData = LeagueSharp.Common.Data.ItemData;
using Prediction = _Project_Geass.Functions.Prediction;

#endregion

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Module.Champions.Heroes.Events
{

    internal class Ezreal : Base
    {
        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Ezreal" /> class.
        /// </summary>
        /// <param name="manaEnabled">
        ///     if set to <c> true </c> [mana enabled].
        /// </param>
        /// <param name="orbwalker">
        ///     The orbwalker.
        /// </param>
        public Ezreal(bool manaEnabled, Orbwalking.Orbwalker orbwalker)
        {
            Q=new Spell(SpellSlot.Q, 1190);
            W=new Spell(SpellSlot.W, 950);
            R=new Spell(SpellSlot.R, 2200);

            Q.SetSkillshot(.25f, 60, 2000, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(.25f, 80, 1600, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(1, 160, 2000, false, SkillshotType.SkillshotLine);

            _manaManager=new Mana(Q, W, E, R, manaEnabled);
            // ReSharper disable once UnusedVariable
            var temp=new Menus.Ezreal();

            Game.OnUpdate+=OnUpdate;
            Game.OnUpdate+=AutoEvents;
            EloBuddy.Drawing.OnDraw+=OnDraw;
            EloBuddy.Drawing.OnDraw+=OnDrawEnemy;
            Spellbook.OnCastSpell+=OnCastSpell;
            //Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            _damageIndicator=new DamageIndicator(GetDamage, 2000);
            Orbwalker=orbwalker;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        ///     Gets the real R damage.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <returns>
        /// </returns>
        public double GetRealRDamage(Obj_AI_Base target)
        {
            var damage=StaticObjects.Player.GetSpellDamage(target, SpellSlot.R);
            var hits=R.GetCollision(StaticObjects.Player.ServerPosition.To2D(), new List<Vector2> {target.ServerPosition.To2D()}).Count;
            var debuff=hits>7? .7 : hits*.1;
            return damage*(1-debuff*10);
        }

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

        //private const float DelayCheck = 8000;
        //private static float _lastTick;
        //private static float _lastMana;
        private readonly bool _tearFull=false;

        #endregion Private Fields

        #region Private Methods

        /// <summary>
        ///     Automated events.
        /// </summary>
        /// <param name="args">
        ///     The <see cref="EventArgs" /> instance containing the event data.
        /// </param>
        private void AutoEvents(EventArgs args)
        {
            if (!Handler.CheckAutoEvents())
                return;

            if (!_tearFull)
                if (!StaticObjects.Player.IsRecalling())
                {
                    var basename=BaseName+"Misc.";

                    if (StaticObjects.ProjectMenu.Item($"{basename}.UseQ.TearStack").GetValue<bool>()&&(_manaManager.ManaPercent>=StaticObjects.ProjectMenu.Item($"{basename}.UseQ.TearStack.MinMana").GetValue<Slider>().Value))
                        if (Items.HasItem(ItemData.Tear_of_the_Goddess.Id)||Items.HasItem(ItemData.Manamune.Id))
                            if ((Minions.GetEnemyMinions2(1500).Count<1)&&(Functions.Objects.Heroes.GetEnemies(1500).Count<1)&&(MinionManager.GetMinions(1500, MinionTypes.All, MinionTeam.Neutral).Count<1))
                                Q.Cast(Game.CursorPos);
                }
            Handler.UseAutoEvent();
        }

        /// <summary>
        ///     On Clear
        /// </summary>
        private void Clear()
        {
            var basename=BaseName+"Clear.";

            if (StaticObjects.ProjectMenu.Item($"{basename}.UseQ").GetValue<bool>())
                if (_manaManager.CheckClearQ())
                {
                    if (StaticObjects.ProjectMenu.Item($"{basename}.UseQ.Minon.LastHit").GetValue<bool>())
                        foreach (var target in
                            Minions.GetEnemyMinions2(Q.Range).Where(x => (x.Health<Q.GetDamage(x))&&(x.Health>30)).OrderBy(hp => hp.Health))
                        {
                            if (!StaticObjects.Player.Spellbook.IsAutoAttacking&&(StaticObjects.Player.GetAutoAttackDamage(target)<target.Health+25)&&(StaticObjects.Player.Distance(target)<StaticObjects.Player.AttackRange))
                                continue;

                            Q.Cast(target);
                            return;
                        }

                    if (StaticObjects.ProjectMenu.Item($"{basename}.UseQ.OnJungle").GetValue<bool>())
                        foreach (var target in MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral).Where(x => x.IsValidTarget(Q.Range)).OrderBy(hp => hp.Health))
                        {
                            Q.Cast(target);
                            return;
                        }
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
                {
                    var minHitChance=Prediction.GetHitChance(StaticObjects.ProjectMenu.Item($"{basename}.UseQ.Prediction").GetValue<StringList>().SelectedValue);

                    var focusTargetValid=false;
                    //Check if the target in target selector is valid (best target)
                    var focusTarget=TargetSelector.GetTarget(Q.Range, Q.DamageType);
                    if (focusTarget!=null)
                        if (StaticObjects.ProjectMenu.Item($"{basename}.UseQ.On.{focusTarget.ChampionName}").GetValue<bool>())
                            focusTargetValid=Prediction.DoCast(Q, focusTarget, minHitChance, true);

                    if (!focusTargetValid)
                    {
                        var orderedTargets=Prediction.OrderTargets(Q);

                        foreach (var target in orderedTargets)
                        {
                            if (!StaticObjects.ProjectMenu.Item($"{basename}.UseQ.On.{target.ChampionName}").GetValue<bool>())
                                continue;

                            if (Prediction.DoCast(Q, focusTarget, minHitChance, true))
                                break;
                        }
                    }
                }

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
                            if (!StaticObjects.ProjectMenu.Item($"{basename}.UseW.On.{target.ChampionName}").GetValue<bool>())
                                continue;

                            if (Prediction.DoCast(W, focusTarget, minHitChance))
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
                    var focusTarget=TargetSelector.GetTarget(StaticObjects.ProjectMenu.Item($"{basename}.UseR.Range").GetValue<Slider>().Value, R.DamageType);
                    if (focusTarget!=null)
                        if (StaticObjects.ProjectMenu.Item($"{basename}.UseR.On.{focusTarget.ChampionName}").GetValue<bool>())
                            if (focusTarget.Distance(StaticObjects.Player)>Q.Range)
                                if (GetRealRDamage(focusTarget)>focusTarget.Health)
                                    focusTargetValid=Prediction.DoCast(R, focusTarget, minHitChance);

                    if (!focusTargetValid)
                    {
                        var orderedTargets=Prediction.OrderTargets(R);

                        foreach (var target in orderedTargets)
                        {
                            if (!StaticObjects.ProjectMenu.Item($"{basename}.UseR.On.{target.ChampionName}").GetValue<bool>())
                                continue;
                            if (target.Health>GetRealRDamage(target))
                                continue;
                            if (Q.Range>target.Distance(StaticObjects.Player))
                                continue;

                            if (Prediction.DoCast(R, target, minHitChance))
                                break;
                        }
                    }
                }
        }

        /// <summary>
        ///     Gets esimated damage
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

            if (StaticObjects.ProjectMenu.Item($"{basename}.UseQ").GetValue<bool>())
                if (_manaManager.CheckMixedQ())
                {
                    var minHitChance=Prediction.GetHitChance(StaticObjects.ProjectMenu.Item($"{basename}.UseQ.Prediction").GetValue<StringList>().SelectedValue);

                    var focusTargetValid=false;
                    //Check if the target in target selector is valid (best target)
                    var focusTarget=TargetSelector.GetTarget(Q.Range, Q.DamageType);
                    if (focusTarget!=null)
                        if (StaticObjects.ProjectMenu.Item($"{basename}.UseQ.On.{focusTarget.ChampionName}").GetValue<bool>())
                            focusTargetValid=Prediction.DoCast(Q, focusTarget, minHitChance, true);

                    if (!focusTargetValid)
                    {
                        var orderedTargets=Prediction.OrderTargets(Q);

                        foreach (var target in orderedTargets)
                        {
                            if (!StaticObjects.ProjectMenu.Item($"{basename}.UseQ.On.{target.ChampionName}").GetValue<bool>())
                                continue;

                            if (Prediction.DoCast(Q, focusTarget, minHitChance, true))
                                break;
                        }
                    }
                }

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
                            if (!StaticObjects.ProjectMenu.Item($"{basename}.UseW.On.{target.ChampionName}").GetValue<bool>())
                                continue;

                            if (Prediction.DoCast(W, focusTarget, minHitChance))
                                break;
                        }
                    }
                }
        }

        private void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (!sender.Owner.IsMe)
                return;

            if ((args.Slot==SpellSlot.Q)||(args.Slot==SpellSlot.W))
                Orbwalking.ResetAutoAttackTimer();
        }

        //private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        //{
        //    if (_tearFull) return;
        //    if (!sender.IsMe) return;

        // if (args.Slot < SpellSlot.Q || args.Slot > SpellSlot.R) return; // 0-3 (Q-R)

        // if (DelayCheck + _lastTick > Functions.AssemblyTime.CurrentTime()) if (Items.HasItem(LeagueSharp.Common.Data.ItemData.Tear_of_the_Goddess.Id)) { _tearFull = _lastMana >=
        // StaticObjects.Player.MaxMana; _lastMana = StaticObjects.Player.MaxMana; }
        /// <summary>
        ///     Raises the <see cref="E:Draw" /> event.
        /// </summary>
        /// <param name="args">
        ///     The <see cref="EventArgs" /> instance containing the event data.
        /// </param>
        private void OnDraw(EventArgs args)
        {
            if (Q.Level>0)
                if (StaticObjects.ProjectMenu.Item(Names.Menu.DrawingItemBase+StaticObjects.Player.ChampionName+".Boolean.DrawOnSelf.QColor").GetValue<Circle>().Active)
                    Render.Circle.DrawCircle(StaticObjects.Player.Position, Q.Range, StaticObjects.ProjectMenu.Item(Names.Menu.DrawingItemBase+StaticObjects.Player.ChampionName+".Boolean.DrawOnSelf.QColor").GetValue<Circle>().Color, 2);
            if (W.Level>0)
                if (StaticObjects.ProjectMenu.Item(Names.Menu.DrawingItemBase+StaticObjects.Player.ChampionName+".Boolean.DrawOnSelf.WColor").GetValue<Circle>().Active)
                    Render.Circle.DrawCircle(StaticObjects.Player.Position, W.Range, StaticObjects.ProjectMenu.Item(Names.Menu.DrawingItemBase+StaticObjects.Player.ChampionName+".Boolean.DrawOnSelf.WColor").GetValue<Circle>().Color, 2);
        }

        //    _lastTick = Functions.AssemblyTime.CurrentTime();
        //}
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