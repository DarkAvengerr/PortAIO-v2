using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy;
using LeagueSharp.Common;
namespace LeeSin_EloClimber
{
    internal class LeeSin
    {
        internal static Spell Q;
        internal static Spell W;
        internal static Spell E;
        internal static Spell R;
        internal static Spell Summoner1;
        internal static Spell Summoner2;
        internal static Orbwalking.Orbwalker Orbwalker;
        internal static AIHeroClient myHero;
        internal static int PassiveStack;
        internal static Spell SmiteSpell;
        internal static float lastWard;
        internal static float idWard;
        internal static float lastQ;
        internal static float qCast;

        internal static void Load()
        {
            // Spell Variable     
            Q = new Spell(SpellSlot.Q, true);
            W = new Spell(SpellSlot.W, true);
            E = new Spell(SpellSlot.E, true);
            R = new Spell(SpellSlot.R, true);
            Summoner1 = new Spell(SpellSlot.Summoner1);
            Summoner2 = new Spell(SpellSlot.Summoner2);

            // Other Variable
            myHero = ObjectManager.Player;
            PassiveStack = 0;
            InitSmite();
            lastWard = Environment.TickCount;

            // Load Class
            MenuManager.LoadMenu();
            Smite.Load();
            Jungle.Load();
            LaneClear.Load();
            WardJump.Load();
            Insec.Load();
            Combo.Load();
            myPred.load();

            // Callback
            Obj_AI_Base.OnBuffGain += OnBuffAdd;
            Obj_AI_Base.OnBuffUpdate += OnBuffUpdateCount;
            Obj_AI_Base.OnBuffLose += OnBuffLose;
            GameObject.OnCreate += Obj_AI_Base_OnCreate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;

        }

        private static void Obj_AI_Base_OnCreate(GameObject obj, EventArgs args)
        {

            if (W.IsReady() && obj.Name.ToLower().Contains("ward") && !obj.Name.ToLower().Contains("corpse") && !obj.Name.ToLower().Contains(".troy"))
            {
                if (!MenuManager.myMenu.Item("wardjump.key").GetValue<KeyBind>().Active && !MenuManager.myMenu.Item("insec.key").GetValue<KeyBind>().Active && Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                    return;

                W.Cast((Obj_AI_Base)obj);
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender.IsMe)
            {
                if (Args.Slot == Q.Slot)
                    lastQ = Environment.TickCount;
            }
            if (sender.IsMe && Args.Slot == Q.Slot)
                qCast = Environment.TickCount;
        }

        private static void OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs Args)
        {
            if (sender.IsMe && Args.Buff.Name == "blindmonkpassive_cosmetic")
                PassiveStack = 2;
        }

        private static void OnBuffUpdateCount(Obj_AI_Base sender, Obj_AI_BaseBuffUpdateEventArgs Args)
        {
            if (sender.IsMe && Args.Buff.Name == "blindmonkpassive_cosmetic")
                PassiveStack = 1;
        }

        private static void OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs Args)
        {
            if (sender.IsMe && Args.Buff.Name == "blindmonkpassive_cosmetic")
                PassiveStack = 0;
        }

        internal static bool IsSecondCast(Spell spell)
        {
            return spell.Instance.SData.Name.ToLower().Contains("two");
        }

        internal static double GetDamage_Q1(Obj_AI_Base unit)
        {
            List<double> baseDmg = new List<double> { 50, 80, 110, 140, 170 };
            double bonus = (myHero.TotalAttackDamage - myHero.BaseAttackDamage) * 0.9;
            double total = baseDmg[Q.Level - 1] + bonus;
            return myHero.CalcDamage(unit, Damage.DamageType.Physical, total);
        }

        internal static double GetDamage_Q2(Obj_AI_Base unit, double offSet)
        {
            List<double> baseDmg = new List<double> { 50, 80, 110, 140, 170 };
            double bonus = (myHero.TotalAttackDamage - myHero.BaseAttackDamage) * 0.9;
            double bonus_second = (unit.MaxHealth - (unit.Health - offSet)) * 0.08;
            double total = baseDmg[Q.Level - 1] + bonus + bonus_second;
            return myHero.CalcDamage(unit, Damage.DamageType.Physical, total);
        }

        internal static double GetDamage_Q(Obj_AI_Base unit, double offSet)
        {
            double Q1_dmg = GetDamage_Q1(unit);
            double Q2_dmg = GetDamage_Q2(unit, (Q1_dmg + offSet));
            return (Q1_dmg + Q2_dmg);
        }

        internal static double GetDamage_R(Obj_AI_Base unit)
        {
            List<double> baseDmg = new List<double> { 200, 400, 600 };
            double bonus = (myHero.TotalAttackDamage - myHero.BaseAttackDamage) * 2;
            double total = baseDmg[R.Level - 1] + bonus;
            return myHero.CalcDamage(unit, Damage.DamageType.Physical, total);
        }

        private static void InitSmite()
        {
            if (LeeSin.Summoner1.Instance.SData.Name.ToLower().Contains("smite"))
            {
                SmiteSpell = LeeSin.Summoner1;
            }
            else if (LeeSin.Summoner2.Instance.SData.Name.ToLower().Contains("smite"))
            {
                SmiteSpell = LeeSin.Summoner2;
            }
        }

        internal static Spell FindWard()
        {
            var slot = Items.GetWardSlot();

            if (slot != default(InventorySlot))
                return new Spell(slot.SpellSlot);

            return null;
        }

        internal static void WardJump_Position(Vector3 endPos)
        {

            Spell wardSpell = FindWard();
            if (Environment.TickCount - lastWard < 500 || !W.IsReady() || IsSecondCast(W) || wardSpell == null)
                return;

            wardSpell.Cast(endPos);
            lastWard = Environment.TickCount;
        }
    }
}
