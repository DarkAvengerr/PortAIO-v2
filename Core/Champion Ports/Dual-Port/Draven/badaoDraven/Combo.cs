using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoDraven
{
    class Combo
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static void useCombo()
        {
            var Qbuff = Player.Buffs.Find(b => b.Name.ToLower() == "dravenspinning");
            var Wbuffmove = Player.Buffs.Find(b => b.Name.ToLower() == "dravenfury");
            var Wbuffas = Player.Buffs.Find(b => b.Name.ToLower() == "dravenfurybuff");

            if(Program.Q.IsReady() && Program.Menu.Item("Use Q Combo").GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(700, TargetSelector.DamageType.Physical);
                if (Orbwalking.InAutoAttackRange(target) && target != null && target.IsValidTarget() && !target.IsZombie)
                {
                    if (Qbuff == null) { Program.Q.Cast(); }
                    else if (Qbuff.Count + Program.Riu.Count < 2 && Qbuff.Count <= 1) { Program.Q.Cast(); }
                }
            }
            if (Program.W.IsReady() && Program.Menu.Item("Use W Combo").GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(700, TargetSelector.DamageType.Physical);
                if (Orbwalking.InAutoAttackRange(target) && Program.W.IsReady() && target != null && target.IsValidTarget() && !target.IsZombie)
                {
                    if (Wbuffas == null) { Program.W.Cast(); }
                }
                else if (!Orbwalking.InAutoAttackRange(target) && Program.W.IsReady() && target != null && target.IsValidTarget() && !target.IsZombie)
                {
                    if (Wbuffmove == null) { Program.W.Cast(); }
                }
            }
            if (Program.W.IsReady() && Program.Menu.Item("Use W to gap Combo").GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(700, TargetSelector.DamageType.Physical);
                if (target == null || !target.IsValidTarget() || target.IsZombie)
                { 
                    var target1 = TargetSelector.GetTarget(1100, TargetSelector.DamageType.Physical);
                    if (target != null && target.IsValidTarget() && !target.IsZombie) { Program.W.Cast(); }
                }
            }
            if (Program.E.IsReady() && Program.Menu.Item("Use E Combo").GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(Program.E.Range , TargetSelector.DamageType.Physical);
                if (!Player.Spellbook.IsAutoAttacking && target != null && target.IsValidTarget() && !target.IsZombie) { Program.E.Cast(target); }
            }
            if (Program.R.IsReady())
            {
                var target =TargetSelector.GetTarget(1400 , TargetSelector.DamageType.Physical);
                if (Program.R.Instance.Name.ToLower().Contains("dravenrcast") && target != null && target.IsValidTarget() && !target.IsZombie && Program.Menu.Item("Use R Combo").GetValue<bool>() && !Player.Spellbook.IsAutoAttacking && target.Health / target.MaxHealth * 100 < Program.Menu.Item("Use R If EnemyHP below").GetValue<Slider>().Value)
                { Program.R.Cast(target); }
                else if (Program.R.Instance.Name.ToLower().Contains("dravenrdoublecast") && target != null && target.IsValidTarget() && !target.IsZombie && (Utils.GameTimeTickCount - Program.Rcount) * 2 >= Player.Distance(target.Position) && Program.Menu.Item("Use R Return Combo").GetValue<bool>())
                { Program.R.Cast(); }
            }
        }
        public static void Harass()
        {
            var Qbuff = Player.Buffs.Find(b => b.Name.ToLower() == "dravenspinning");
            var Wbuffmove = Player.Buffs.Find(b => b.Name.ToLower() == "dravenfury");
            var Wbuffas = Player.Buffs.Find(b => b.Name.ToLower() == "dravenfurybuff");
            if (Program.Q.IsReady() && Program.Menu.Item("Use Q Harass").GetValue<bool>() && Player.Mana / Player.MaxMana * 100 > Program.Menu.Item("minimum Mana HR").GetValue<Slider>().Value)
            {
                var target = TargetSelector.GetTarget(700, TargetSelector.DamageType.Physical);
                if (Orbwalking.InAutoAttackRange(target) && target != null && target.IsValidTarget() && !target.IsZombie)
                {
                    if (Qbuff == null && Program.Riu.Count < 1) { Program.Q.Cast(); }
                }
            }
            if (Program.W.IsReady() && Program.Menu.Item("Use W Harass").GetValue<bool>() && Player.Mana / Player.MaxMana * 100 > Program.Menu.Item("minimum Mana HR").GetValue<Slider>().Value)
            {
                var target = TargetSelector.GetTarget(700, TargetSelector.DamageType.Physical);
                if (Orbwalking.InAutoAttackRange(target) && Program.W.IsReady() && target != null && target.IsValidTarget() && !target.IsZombie)
                {
                    if (Wbuffas == null) { Program.W.Cast(); }
                }
                else if (!Orbwalking.InAutoAttackRange(target) && Program.W.IsReady() && target != null && target.IsValidTarget() && !target.IsZombie)
                {
                    if (Wbuffmove == null) { Program.W.Cast(); }
                }
            }
            if (Program.W.IsReady() && Program.Menu.Item("Use W to gap Harass").GetValue<bool>() && Player.Mana / Player.MaxMana * 100 > Program.Menu.Item("minimum Mana HR").GetValue<Slider>().Value)
            {
                var target = TargetSelector.GetTarget(700, TargetSelector.DamageType.Physical);
                if (target == null || !target.IsValidTarget() || target.IsZombie)
                {
                    var target1 = TargetSelector.GetTarget(900, TargetSelector.DamageType.Physical);
                    if (target != null && target.IsValidTarget() && !target.IsZombie) { Program.W.Cast(); }
                }
            }
            if (Program.E.IsReady() && Program.Menu.Item("Use E Harass").GetValue<bool>() && Player.Mana / Player.MaxMana * 100 > Program.Menu.Item("minimum Mana HR").GetValue<Slider>().Value)
            {
                var target = TargetSelector.GetTarget(Program.E.Range, TargetSelector.DamageType.Physical);
                if (!Player.Spellbook.IsAutoAttacking && target != null && target.IsValidTarget() && !target.IsZombie) { Program.E.Cast(target); }
            }
        }
        public static void LaneClear()
        {
            var Qbuff = Player.Buffs.Find(b => b.Name.ToLower() == "dravenspinning");
            var Wbuffmove = Player.Buffs.Find(b => b.Name.ToLower() == "dravenfury");
            var Wbuffas = Player.Buffs.Find(b => b.Name.ToLower() == "dravenfurybuff");
            var target = MinionManager.GetMinions(Player.Position, 700, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health).FirstOrDefault();
            if (Program.Q.IsReady() && Program.Menu.Item("Use Q LaneClear").GetValue<bool>() && Player.Mana / Player.MaxMana * 100 > Program.Menu.Item("minimum Mana LC").GetValue<Slider>().Value)
            {
                if (Orbwalking.InAutoAttackRange(target) && target != null && target.IsValidTarget() && !target.IsZombie)
                {
                    if (Qbuff == null && Program.Riu.Count < 1) { Program.Q.Cast(); }
                }
            }
            if (Program.W.IsReady() && Program.Menu.Item("Use W LaneClear").GetValue<bool>() && Player.Mana / Player.MaxMana * 100 > Program.Menu.Item("minimum Mana LC").GetValue<Slider>().Value)
            {
                if (Orbwalking.InAutoAttackRange(target) && Program.W.IsReady() && target != null && target.IsValidTarget() && !target.IsZombie)
                {
                    if (Wbuffas == null) { Program.W.Cast(); }
                }
                else if (!Orbwalking.InAutoAttackRange(target) && Program.W.IsReady() && target != null && target.IsValidTarget() && !target.IsZombie)
                {
                    if (Wbuffmove == null) { Program.W.Cast(); }
                }
            }
        }
        public static void JungClear()
        {
            var Qbuff = Player.Buffs.Find(b => b.Name.ToLower() == "dravenspinning");
            var Wbuffmove = Player.Buffs.Find(b => b.Name.ToLower() == "dravenfury");
            var Wbuffas = Player.Buffs.Find(b => b.Name.ToLower() == "dravenfurybuff");
            var target = MinionManager.GetMinions(Player.Position, 600, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (Program.Q.IsReady() && Program.Menu.Item("Use Q JungClear").GetValue<bool>() && Player.Mana / Player.MaxMana * 100 > Program.Menu.Item("minimum Mana JC").GetValue<Slider>().Value)
            {
                if (Orbwalking.InAutoAttackRange(target) && target != null && target.IsValidTarget() && !target.IsZombie)
                {
                    if (Qbuff == null && Program.Riu.Count < 1) { Program.Q.Cast(); }
                }
            }
            if (Program.W.IsReady() && Program.Menu.Item("Use W JungClear").GetValue<bool>() && Player.Mana / Player.MaxMana * 100 > Program.Menu.Item("minimum Mana JC").GetValue<Slider>().Value)
            {
                if (Orbwalking.InAutoAttackRange(target) && Program.W.IsReady() && target != null && target.IsValidTarget() && !target.IsZombie)
                {
                    if (Wbuffas == null) { Program.W.Cast(); }
                }
                else if (!Orbwalking.InAutoAttackRange(target) && Program.W.IsReady() && target != null && target.IsValidTarget() && !target.IsZombie)
                {
                    if (Wbuffmove == null) { Program.W.Cast(); }
                }
            }
        }
    }
}
