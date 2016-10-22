using System;
using System.Runtime.CompilerServices;
using LeagueSharp;
using LeagueSharp.Common;
using SCommon.Orbwalking;
using SCommon.Evade;
using SCommon.Prediction;
using SharpDX;
using SharpDX.Direct3D9;
//typedefs
using Prediction = SCommon.Prediction.Prediction;
using Geometry = SCommon.Maths.Geometry;
using Color = System.Drawing.Color;
using TargetSelector = SCommon.TS.TargetSelector;
using EloBuddy;

namespace SCommon.PluginBase
{
    public abstract class Champion : IChampion
    {
        public const int Q = 0, W = 1, E = 2, R = 3;

        public Menu ConfigMenu, DrawingMenu;
        public Orbwalking.Orbwalker Orbwalker;
        public Spell[] Spells = new Spell[4];
        public Font Text;

        public delegate void dVoidDelegate();
        public dVoidDelegate OnUpdate, OnDraw, OnCombo, OnHarass, OnLaneClear, OnLastHit;

        /// <summary>
        /// Champion constructor
        /// </summary>
        /// <param name="szChampName">The champion name.</param>
        /// <param name="szMenuName">The menu name.</param>
        /// <param name="enableRangeDrawings">if <c>true</c>, enables the spell range drawings</param>
        /// <param name="enableEvader">if <c>true</c>, enables the spell evader if the champion is supported</param>
        public Champion(string szChampName, string szMenuName, bool enableRangeDrawings = true, bool enableEvader = true)
        {
            Text = new Font(Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Malgun Gothic",
                    Height = 15,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.ClearTypeNatural
                });

            ConfigMenu = new Menu(szMenuName, String.Format("SAutoCarry.{0}.Root", szChampName), true);

            TargetSelector.Initialize(ConfigMenu);
            Orbwalker = new Orbwalking.Orbwalker(ConfigMenu);

            SetSpells();

            DrawingMenu = new Menu("Drawings", "drawings");
            if (enableRangeDrawings)
            {
                if (this.Spells[Q] != null && this.Spells[0].Range > 0 && this.Spells[Q].Range < 3000)
                    this.DrawingMenu.AddItem(new MenuItem("DDRAWQ", "Draw Q").SetValue(new Circle(true, Color.Red, this.Spells[Q].Range)));

                if (this.Spells[W] != null && this.Spells[1].Range > 0 && this.Spells[W].Range < 3000)
                    this.DrawingMenu.AddItem(new MenuItem("DDRAWW", "Draw W").SetValue(new Circle(true, Color.Aqua, this.Spells[W].Range)));

                if (this.Spells[E] != null && this.Spells[2].Range > 0 && this.Spells[E].Range < 3000)
                    this.DrawingMenu.AddItem(new MenuItem("DDRAWE", "Draw E").SetValue(new Circle(true, Color.Bisque, this.Spells[E].Range)));

                if (this.Spells[R] != null && this.Spells[3].Range > 0 && this.Spells[R].Range < 3000) //global ult ?
                    this.DrawingMenu.AddItem(new MenuItem("DDRAWR", "Draw R").SetValue(new Circle(true, Color.Chartreuse, this.Spells[R].Range)));
            }
            ConfigMenu.AddSubMenu(DrawingMenu);

            if (enableEvader)
            {
                Menu evaderMenu = null;
                Evader evader;
                switch (szChampName.ToLower())
                {
                    case "ezreal":
                        evader = new Evader(out evaderMenu, Database.EvadeMethods.Blink, Spells[E]);
                        break;
                    case "sivir":
                    case "morgana":
                        evader = new Evader(out evaderMenu, Database.EvadeMethods.SpellShield, Spells[E]);
                        break;
                    case "fizz":
                        evader = new Evader(out evaderMenu, Database.EvadeMethods.Dash, Spells[E]);
                        break;
                    case "lissandra":
                        evader = new Evader(out evaderMenu, Database.EvadeMethods.Invulnerability, Spells[R]);
                        break;
                    case "nocturne":
                        evader = new Evader(out evaderMenu, Database.EvadeMethods.SpellShield, Spells[W]);
                        break;
                    case "vladimir":
                        evader = new Evader(out evaderMenu, Database.EvadeMethods.Invulnerability, Spells[W]);
                        break;
                    case "graves":
                    case "gnar":
                    case "lucian":
                    case "riven":
                    case "shen":
                        evader = new Evader(out evaderMenu, Database.EvadeMethods.Dash, Spells[E]);
                        break;
                    case "zed":
                    case "leblanc":
                    case "corki":
                        evader = new Evader(out evaderMenu, Database.EvadeMethods.Dash, Spells[W]);
                        break;
                    case "vayne":
                        evader = new Evader(out evaderMenu, Database.EvadeMethods.Dash, Spells[Q]);
                        break;
                }
                if (evaderMenu != null)
                    ConfigMenu.AddSubMenu(evaderMenu);
            }
            CreateConfigMenu();

            Menu credits = new Menu("Credits", "SAutoCarry.Credits.Root");
            credits.AddItem(new MenuItem("SAutoCarry.Credits.Root.Author", "SAutoCarry - Made By Synx"));
            credits.AddItem(new MenuItem("SAutoCarry.Credits.Root.Upvote", "Dont Forget to upvote in DB!"));

            Menu supportedChamps = new Menu("Supported Champions", "SAutoCarry.Credits.Supported");

            Menu adc = new Menu("ADC (5)", "SAutoCarry.Credits.ADC");
            adc.AddItem(new MenuItem("SAutoCarry.Credits.ADC.Supported1", "  ->Corki        "));
            adc.AddItem(new MenuItem("SAutoCarry.Credits.ADC.Supported2", "  ->Lucian       "));
            adc.AddItem(new MenuItem("SAutoCarry.Credits.ADC.Supported3", "  ->Miss Fortune "));
            adc.AddItem(new MenuItem("SAutoCarry.Credits.ADC.Supported4", "  ->Twitch       "));
            adc.AddItem(new MenuItem("SAutoCarry.Credits.ADC.Supported5", "  ->Vayne        "));
            //
            supportedChamps.AddSubMenu(adc);
            //
            Menu mid = new Menu("Mid (6)", "SAutoCarry.Credits.Mid");
            mid.AddItem(new MenuItem("SAutoCarry.Credits.Mid.Supported1", "  ->Azir         "));
            mid.AddItem(new MenuItem("SAutoCarry.Credits.Mid.Supported2", "  ->Cassiopeia   "));
            mid.AddItem(new MenuItem("SAutoCarry.Credits.Mid.Supported3", "  ->Orianna      "));
            mid.AddItem(new MenuItem("SAutoCarry.Credits.Mid.Supported4", "  ->Twisted Fate "));
            mid.AddItem(new MenuItem("SAutoCarry.Credits.Mid.Supported5", "  ->Veigar       "));
            mid.AddItem(new MenuItem("SAutoCarry.Credits.Mid.Supported6", "  ->Viktor       "));
            //
            supportedChamps.AddSubMenu(mid);
            //
            Menu top = new Menu("Top (5)", "SAutoCarry.Credits.Top");
            top.AddItem(new MenuItem("SAutoCarry.Credits.Top.Supported1", "  ->Darius      "));
            top.AddItem(new MenuItem("SAutoCarry.Credits.Top.Supported2", "  ->Dr. Mundo   "));
            top.AddItem(new MenuItem("SAutoCarry.Credits.Top.Supported3", "  ->Pantheon    "));
            top.AddItem(new MenuItem("SAutoCarry.Credits.Top.Supported4", "  ->Rengar      "));
            top.AddItem(new MenuItem("SAutoCarry.Credits.Top.Supported5", "  ->Riven       "));
            //
            supportedChamps.AddSubMenu(top);
            //
            Menu jungle = new Menu("Jungle (3)", "SAutoCarry.Credits.Jungle");
            jungle.AddItem(new MenuItem("SAutoCarry.Credits.Jungle.Supported1", "  ->Jax          "));
            jungle.AddItem(new MenuItem("SAutoCarry.Credits.Jungle.Supported2", "  ->Master Yi    "));
            jungle.AddItem(new MenuItem("SAutoCarry.Credits.Jungle.Supported3", "  ->Shyvana      "));
            //
            supportedChamps.AddSubMenu(jungle);
            //
            Menu support = new Menu("Support (1)", "SAutoCarry.Credits.Support");
            support.AddItem(new MenuItem("SAutoCarry.Credits.Support.Support1", "  ->Blitzcrank   "));
            //
            supportedChamps.AddSubMenu(support);
            //

            credits.AddSubMenu(supportedChamps);

            #region Events
            Game.OnUpdate += this.Game_OnUpdate;
            Drawing.OnDraw += this.Drawing_OnDraw;
            Orbwalking.Events.BeforeAttack += this.OrbwalkingEvents_BeforeAttack;
            Orbwalking.Events.AfterAttack += this.OrbwalkingEvents_AfterAttack;
            AntiGapcloser.OnEnemyGapcloser += this.AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += this.Interrupter_OnPossibleToInterrupt;
            Obj_AI_Base.OnBuffGain += this.Obj_AI_Base_OnBuffAdd;
            Obj_AI_Base.OnSpellCast += this.Obj_AI_Base_OnProcessSpellCast;
            CustomEvents.Unit.OnDash += this.Unit_OnDash;
            TargetedSpellDetector.OnDetected += this.TargetedSpellDetector_OnDetected;
            #endregion

            Prediction.Prediction.Initialize(ConfigMenu);
            ConfigMenu.AddSubMenu(credits);
        }

        /// <summary>
        /// Creates the config menu.
        /// </summary>
        public virtual void CreateConfigMenu()
        {
            ConfigMenu.AddToMainMenu();
        }

        /// <summary>
        /// Sets spell values of the hero.
        /// </summary>
        public virtual void SetSpells()
        {
            Spells[Q] = new Spell(SpellSlot.Q);
            Spells[W] = new Spell(SpellSlot.W);
            Spells[E] = new Spell(SpellSlot.E);
            Spells[R] = new Spell(SpellSlot.R);
        }

        public virtual void Game_OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || ObjectManager.Player.IsRecalling() || args == null)
                return;

            if (OnUpdate != null)
                OnUpdate();

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.Orbwalker.Mode.Combo:
                    {
                        if (OnCombo != null)
                            OnCombo();
                    }
                    break;
                case Orbwalking.Orbwalker.Mode.Mixed:
                    {
                        if (OnHarass != null)
                            OnHarass();
                    }
                    break;
                case Orbwalking.Orbwalker.Mode.LaneClear:
                    {
                        if (OnLaneClear != null)
                            OnLaneClear();
                    }
                    break;
                case Orbwalking.Orbwalker.Mode.LastHit:
                    {
                        if (OnLastHit != null)
                            OnLastHit();
                    }
                    break;

            }
        }

        public virtual void Drawing_OnDraw(EventArgs args)
        {
            if (OnDraw != null)
                OnDraw();

            foreach (MenuItem it in DrawingMenu.Items)
            {
                Circle c = it.GetValue<Circle>();
                if (c.Active)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, c.Radius, c.Color, 2);
            }
        }

        /// <summary>
        /// The BeforeAttack event which called by orbwalker.
        /// </summary>
        /// <param name="args">The args.</param>
        protected virtual void OrbwalkingEvents_BeforeAttack(BeforeAttackArgs args)
        {
            //
        }

        /// <summary>
        /// The AfterAttack event which called by orbwalker.
        /// </summary>
        /// <param name="args">The args.</param>
        protected virtual void OrbwalkingEvents_AfterAttack(AfterAttackArgs args)
        {
            //
        }

        /// <summary>
        /// The AntiGapCloser event.
        /// </summary>
        /// <param name="gapcloser">The gapcloser.</param>
        protected virtual void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            //
        }

        /// <summary>
        /// The OnPossibleToInterrupt event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        protected virtual void Interrupter_OnPossibleToInterrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            //
        }

        /// <summary>
        /// The OnBuffAdd event
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        protected virtual void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            //
        }

        /// <summary>
        /// The OnProcessSpellCast event
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        protected virtual void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            //
        }

        /// <summary>
        /// The OnDash event
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        protected virtual void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            //
        }

        protected virtual void TargetedSpellDetector_OnDetected(DetectedTargetedSpellArgs data)
        {
            //
        }

        /// <summary>
        /// Checks if combo is ready
        /// </summary>
        /// <returns>true if combo is ready</returns>
        public bool ComboReady()
        {
            return Spells[Q].IsReady() && Spells[W].IsReady() && Spells[E].IsReady() && Spells[R].IsReady();
        }

        #region Damage Calculation Funcitons
        /// <summary>
        /// Calculates combo damage to given target
        /// </summary>
        /// <param name="target">Target</param>
        /// <param name="aacount">Auto Attack Count</param>
        /// <returns>Combo damage</returns>
        public double CalculateComboDamage(AIHeroClient target, int aacount = 2)
        {
            return CalculateSpellDamage(target) + CalculateSummonersDamage(target) + CalculateItemsDamage(target) + CalculateAADamage(target, aacount);
        }

        /// <summary>
        /// Calculates Spell Q damage to given target
        /// </summary>
        /// <param name="target">Target</param>
        /// <returns>Spell Q Damage</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual double CalculateDamageQ(AIHeroClient target)
        {
            if (Spells[Q].IsReady())
                return ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q);

            return 0.0;
        }

        /// <summary>
        /// Calculates Spell W damage to given target
        /// </summary>
        /// <param name="target">Target</param>
        /// <returns>Spell W Damage</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual double CalculateDamageW(AIHeroClient target)
        {
            if (Spells[W].IsReady())
                return ObjectManager.Player.GetSpellDamage(target, SpellSlot.W);

            return 0.0;
        }

        /// <summary>
        /// Calculates Spell E damage to given target
        /// </summary>
        /// <param name="target">Target</param>
        /// <returns>Spell E Damage</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual double CalculateDamageE(AIHeroClient target)
        {
            if (Spells[E].IsReady())
                return ObjectManager.Player.GetSpellDamage(target, SpellSlot.E);

            return 0.0;
        }

        /// <summary>
        /// Calculates Spell R damage to given target
        /// </summary>
        /// <param name="target">Target</param>
        /// <returns>Spell R Damage</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual double CalculateDamageR(AIHeroClient target)
        {
            if (Spells[R].IsReady())
                return ObjectManager.Player.GetSpellDamage(target, SpellSlot.R);

            return 0.0;
        }

        /// <summary>
        /// Calculates all spell's damage to given target
        /// </summary>
        /// <param name="target">Target</param>
        /// <returns>All spell's damage</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double CalculateSpellDamage(AIHeroClient target)
        {
            return CalculateDamageQ(target) + CalculateDamageW(target) + CalculateDamageE(target) + CalculateDamageR(target);
        }

        /// <summary>
        /// Calculates summoner spell damages to given target
        /// </summary>
        /// <param name="target">Target</param>
        /// <returns>Summoner spell damage</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double CalculateSummonersDamage(AIHeroClient target)
        {
            var ignite = ObjectManager.Player.GetSpellSlot("summonerdot");
            if (ignite != SpellSlot.Unknown && ObjectManager.Player.Spellbook.CanUseSpell(ignite) == SpellState.Ready && ObjectManager.Player.Distance(target, false) < 550)
                return ObjectManager.Player.GetSummonerSpellDamage(target, LeagueSharp.Common.Damage.SummonerSpell.Ignite); //ignite

            return 0.0;
        }

        /// <summary>
        /// Calculates Item's active damages to given target
        /// </summary>
        /// <param name="target">Target</param>
        /// <returns>Item's damage</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual double CalculateItemsDamage(AIHeroClient target)
        {
            double dmg = 0.0;

            if (Items.CanUseItem(3144) && ObjectManager.Player.Distance(target, false) < 550)
                dmg += ObjectManager.Player.GetItemDamage(target, LeagueSharp.Common.Damage.DamageItems.Bilgewater); //bilgewater cutlass

            if (Items.CanUseItem(3153) && ObjectManager.Player.Distance(target, false) < 550)
                dmg += ObjectManager.Player.GetItemDamage(target, LeagueSharp.Common.Damage.DamageItems.Botrk); //botrk

            if (Items.HasItem(3057))
                dmg += ObjectManager.Player.CalcDamage(target, LeagueSharp.Common.Damage.DamageType.Magical, ObjectManager.Player.BaseAttackDamage); //sheen

            if (Items.HasItem(3100))
                dmg += ObjectManager.Player.CalcDamage(target, LeagueSharp.Common.Damage.DamageType.Magical, (0.75 * ObjectManager.Player.BaseAttackDamage) + (0.50 * ObjectManager.Player.FlatMagicDamageMod)); //lich bane

            if (Items.HasItem(3285))
                dmg += ObjectManager.Player.CalcDamage(target, LeagueSharp.Common.Damage.DamageType.Magical, 100 + (0.1 * ObjectManager.Player.FlatMagicDamageMod)); //luden

            return dmg;

        }

        /// <summary>
        /// Calculates Auto Attack damage to given target
        /// </summary>
        /// <param name="target">Targetparam>
        /// <param name="aacount">Auto Attack count</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual double CalculateAADamage(AIHeroClient target, int aacount = 2)
        {
            return Damage.AutoAttack.GetDamage(target) * aacount;
        }
        #endregion
    }
}
