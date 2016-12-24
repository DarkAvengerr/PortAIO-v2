using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using LeagueSharp;
using LeagueSharp.Common;
using ShineCommon;
using ShineCommon.Activator;
using SPrediction;

using SharpDX;
using SharpDX.Direct3D9;
//typedefs
using Prediction = SPrediction.Prediction;
using Geometry = SPrediction.Geometry;
using EloBuddy;
using LeagueSharp.Common;
namespace VeigShineCommon
{
    public abstract class BaseChamp
    {
        public const int Q = 0, W = 1, E = 2, R = 3;

        public Menu Config, combo, ult, harass, laneclear, misc, drawing, evade, activator, orb;
        public Orbwalking.Orbwalker Orbwalker;
        public Spell[] Spells = new Spell[4];
        public Evader m_evader;
        public Font Text;

        public delegate void dVoidDelegate();
        public dVoidDelegate BeforeOrbWalking, BeforeDrawing;
        public dVoidDelegate[] OrbwalkingFunctions = new dVoidDelegate[6];

        public BaseChamp(string szChampName)
        {
            Text = new Font(Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Malgun Gothic",
                    Height = 15,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.ClearTypeNatural
                });

            Config = new Menu("Placebo Veigar", szChampName, true);

            TargetSelector.AddToMenu(Config.SubMenu("Target Selector"));
            orb = Config.SubMenu("Orbwalking");

            Orbwalker = new Orbwalking.Orbwalker(orb.SubMenu("Common Orbwalker"));
            orb.AddItem(new MenuItem("orbmode", "Orbwalk Mode").SetValue<StringList>(new StringList(new string[] { "Common Orbwalker" }, 0)))
                        .ValueChanged += (s, ar) =>
                        {
                            dVoidDelegate fnCombo, fnHarass, fnLastHit, fnLaneClear;
                            fnCombo = fnHarass = fnLastHit = fnLaneClear = null;

                            if (fnCombo != null)
                                OrbwalkingFunctions[(int)Orbwalking.OrbwalkingMode.Combo] += fnCombo;
                            if (fnHarass != null)
                                OrbwalkingFunctions[(int)Orbwalking.OrbwalkingMode.Mixed] += fnHarass;
                            if (fnLaneClear != null)
                                OrbwalkingFunctions[(int)Orbwalking.OrbwalkingMode.LaneClear] += fnLaneClear;
                            if (fnLastHit != null)
                                OrbwalkingFunctions[(int)Orbwalking.OrbwalkingMode.LastHit] += fnLastHit;
                        };

            activator = new Menu("Activator", "activator");
            new Smite(TargetSelector.DamageType.Magical, activator);
            new Ignite(TargetSelector.DamageType.Magical, activator);

            drawing = new Menu("Drawings", "drawings");

            Config.AddSubMenu(activator);
            Config.AddSubMenu(drawing);
            SpellDatabase.InitalizeSpellDatabase();
        }

        public virtual void CreateConfigMenu()
        {
            Config.AddToMainMenu();
        }

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

            if (BeforeOrbWalking != null) BeforeOrbWalking();

            if (OrbwalkingActiveMode != OrbwalkingNoneMode && OrbwalkingFunctions[(int)OrbwalkingActiveMode] != null)
                OrbwalkingFunctions[(int)OrbwalkingActiveMode]();
        }

        public virtual void Drawing_OnDraw(EventArgs args)
        {

            if (BeforeDrawing != null) BeforeDrawing();

            foreach (MenuItem it in drawing.Items)
            {
                Circle c = it.GetValue<Circle>();
                if (c.Active)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, c.Radius, c.Color, 2);
            }
        }

        public virtual void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            //
        }

        public virtual void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            //
        }

        public virtual void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            //
        }

        public virtual void Interrupter_OnPossibleToInterrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            //
        }

        public virtual void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            //
        }

        public virtual void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            //
        }

        public virtual void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            //
        }

        public bool ComboReady()
        {
            return Spells[Q].IsReady() && Spells[W].IsReady() && Spells[E].IsReady() && Spells[R].IsReady();
        }

        #region Damage Calculation Funcitons
        public double CalculateComboDamage(AIHeroClient target, int aacount = 2)
        {
            return CalculateSpellDamage(target) + CalculateSummonersDamage(target) + CalculateItemsDamage(target) + CalculateAADamage(target, aacount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual double CalculateDamageQ(AIHeroClient target)
        {
            var item = Config.Item("CUSEQ");
            if (item != null && item.GetValue<bool>() && Spells[Q].IsReady())
                return ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q);

            return 0.0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual double CalculateDamageW(AIHeroClient target)
        {
            var item = Config.Item("CUSEW");
            if (item != null && item.GetValue<bool>() && Spells[W].IsReady())
                return ObjectManager.Player.GetSpellDamage(target, SpellSlot.W);

            return 0.0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual double CalculateDamageE(AIHeroClient target)
        {
            var item = Config.Item("CUSEE");
            if (item != null && item.GetValue<bool>() && Spells[E].IsReady())
                return ObjectManager.Player.GetSpellDamage(target, SpellSlot.E);

            return 0.0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual double CalculateDamageR(AIHeroClient target)
        {
            var item = Config.Item("CUSER");
            if (item != null && item.GetValue<bool>() && Spells[R].IsReady())
                return ObjectManager.Player.GetSpellDamage(target, SpellSlot.R);

            return 0.0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double CalculateSpellDamage(AIHeroClient target)
        {
            return CalculateDamageQ(target) + CalculateDamageW(target) + CalculateDamageE(target) + CalculateDamageR(target);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double CalculateSummonersDamage(AIHeroClient target)
        {
            var ignite = ObjectManager.Player.GetSpellSlot("summonerdot");
            if (ignite != SpellSlot.Unknown && ObjectManager.Player.Spellbook.CanUseSpell(ignite) == SpellState.Ready && ObjectManager.Player.Distance(target, false) < 550)
                return ObjectManager.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite); //ignite

            return 0.0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual double CalculateItemsDamage(AIHeroClient target)
        {
            double dmg = 0.0;

            if (Items.CanUseItem(3144) && ObjectManager.Player.Distance(target, false) < 550)
                dmg += ObjectManager.Player.GetItemDamage(target, Damage.DamageItems.Bilgewater); //bilgewater cutlass

            if (Items.CanUseItem(3153) && ObjectManager.Player.Distance(target, false) < 550)
                dmg += ObjectManager.Player.GetItemDamage(target, Damage.DamageItems.Botrk); //botrk

            if (Items.HasItem(3057))
                dmg += ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, ObjectManager.Player.BaseAttackDamage); //sheen

            if (Items.HasItem(3100))
                dmg += ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, (0.75 * ObjectManager.Player.BaseAttackDamage) + (0.50 * ObjectManager.Player.FlatMagicDamageMod)); //lich bane

            if (Items.HasItem(3285))
                dmg += ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, 100 + (0.1 * ObjectManager.Player.FlatMagicDamageMod)); //luden

            return dmg;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual double CalculateAADamage(AIHeroClient target, int aacount = 2)
        {
            double dmg = ObjectManager.Player.GetAutoAttackDamage(target) * aacount;

            if (Items.HasItem(3115))
                dmg += ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, 15 + (0.15 * ObjectManager.Player.FlatMagicDamageMod)); //nashor

            return dmg;
        }
        #endregion

        public bool LXOrbwalkerEnabled
        {
            get
            {
                return Config.Item("orbmode").GetValue<StringList>().SelectedIndex == 1;
            }
        }

        public int OrbwalkingComboMode
        {
            get
            {
                return (int)Orbwalking.OrbwalkingMode.Combo;
            }
        }

        public int OrbwalkingHarassMode
        {
            get
            {
                return (int)Orbwalking.OrbwalkingMode.Mixed;
            }
        }

        public int OrbwalkingLaneClearMode
        {
            get
            {
                return (int)Orbwalking.OrbwalkingMode.LaneClear;
            }
        }

        public int OrbwalkingLastHitMode
        {
            get
            {
                return (int)Orbwalking.OrbwalkingMode.LastHit;
            }
        }

        public int OrbwalkingNoneMode
        {
            get
            {
                return (int)Orbwalking.OrbwalkingMode.None;
            }
        }

        public int OrbwalkingActiveMode
        {
            get
            {
                return (int)Orbwalker.ActiveMode;
            }
        }
    }
}
