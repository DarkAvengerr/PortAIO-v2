using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using LeagueSharp;
using LeagueSharp.Common;
using ShineCommon;
using SPrediction;
using ShineCommon.Activator;
using SharpDX;
using SharpDX.Direct3D9;
//typedefs
using Prediction = SPrediction.Prediction;
using Geometry = SPrediction.Geometry;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ShineCommon
{
    public abstract class BaseChamp
    {
        public const int Q = 0, W = 1, E = 2, R = 3;

        public Menu Config, combo, ult, harass, laneclear, misc, drawing, evade, activator;
        public Orbwalking.Orbwalker Orbwalker;
        public Spell[] Spells = new Spell[4];
        public Evader m_evader;
        public Font Text;

        public delegate void dVoidDelegate();
        public dVoidDelegate BeforeOrbWalking, BeforeDrawing;
        public dVoidDelegate[] OrbwalkingFunctions = new dVoidDelegate[4];

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

            Config = new Menu(String.Format("Shine# {0} !", szChampName), szChampName, true);
            
            TargetSelector.AddToMenu(Config.SubMenu("Target Selector"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));
            
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
            //
        }

        public virtual void SetSpells()
        {
            //
        }

        public virtual void Game_OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || ObjectManager.Player.LSIsRecalling() || args == null)
                return;

            if (BeforeOrbWalking != null) BeforeOrbWalking();

            if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None && OrbwalkingFunctions[(int)Orbwalker.ActiveMode] != null)
                OrbwalkingFunctions[(int)Orbwalker.ActiveMode]();
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

        public void CastSkillshot(AIHeroClient t, Spell s, HitChance hc = HitChance.High)
        {
            s.SPredictionCast(t, hc);
        }


        public bool ComboReady()
        {
            return Spells[Q].LSIsReady() && Spells[W].LSIsReady() && Spells[E].LSIsReady() && Spells[R].LSIsReady();
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
            if (item != null && item.GetValue<bool>() && Spells[Q].LSIsReady())
                return ObjectManager.Player.LSGetSpellDamage(target, SpellSlot.Q);

            return 0.0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual double CalculateDamageW(AIHeroClient target)
        {
            var item = Config.Item("CUSEW");
            if (item != null && item.GetValue<bool>() && Spells[W].LSIsReady())
                return ObjectManager.Player.LSGetSpellDamage(target, SpellSlot.W);

            return 0.0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual double CalculateDamageE(AIHeroClient target)
        {
            var item = Config.Item("CUSEE");
            if (item != null && item.GetValue<bool>() && Spells[E].LSIsReady())
                return ObjectManager.Player.LSGetSpellDamage(target, SpellSlot.E);

            return 0.0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual double CalculateDamageR(AIHeroClient target)
        {
            var item = Config.Item("CUSER");
            if (item != null && item.GetValue<bool>() && Spells[R].LSIsReady())
                return ObjectManager.Player.LSGetSpellDamage(target, SpellSlot.R);

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
            var ignite = ObjectManager.Player.LSGetSpellSlot("summonerdot");
            if (ignite != SpellSlot.Unknown && ObjectManager.Player.Spellbook.CanUseSpell(ignite) == SpellState.Ready && ObjectManager.Player.LSDistance(target, false) < 550)
                return ObjectManager.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite); //ignite

            return 0.0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual double CalculateItemsDamage(AIHeroClient target)
        {
            double dmg = 0.0;

            if (Items.CanUseItem(3144) && ObjectManager.Player.LSDistance(target, false) < 550)
                dmg += ObjectManager.Player.GetItemDamage(target, Damage.DamageItems.Bilgewater); //bilgewater cutlass

            if (Items.CanUseItem(3153) && ObjectManager.Player.LSDistance(target, false) < 550)
                dmg += ObjectManager.Player.GetItemDamage(target, Damage.DamageItems.Botrk); //botrk

            if(Items.HasItem(3057))
                dmg += ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, ObjectManager.Player.BaseAttackDamage); //sheen

            if (Items.HasItem(3100))
                dmg += ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, (0.75 * ObjectManager.Player.BaseAttackDamage) + (0.50 * ObjectManager.Player.FlatMagicDamageMod)); //lich bane
            
            if(Items.HasItem(3285))
                dmg += ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, 100 + (0.1 * ObjectManager.Player.FlatMagicDamageMod)); //luden

            return dmg;
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual double CalculateAADamage(AIHeroClient target, int aacount = 2)
        {
            double dmg = ObjectManager.Player.LSGetAutoAttackDamage(target) * aacount;

            if (Items.HasItem(3115))
                dmg += ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, 15 + (0.15 * ObjectManager.Player.FlatMagicDamageMod)); //nashor

            return dmg;
        }
        #endregion
    }
}
