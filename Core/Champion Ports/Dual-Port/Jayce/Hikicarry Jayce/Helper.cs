using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using LeagueSharp;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using HikiCarry_Jayce___Hammer_of_Justice;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry_Jayce___Hammer_of_Justice
{
    class Helper
    {
        private static readonly double[] CannonQDamage = { 70, 120, 170, 220, 270, 320 };
        private static readonly double[] HammerQDamage = { 30, 70, 110, 150, 190, 230 };
        private static readonly double[] HammerWDamage = { 25, 40, 55, 70, 85, 100 };
        private static readonly double[] HammerEDamage = { 8, 10.4, 12.8, 15.2, 17.6, 20 };

        public static void CheckForm()
        {
            if (Program.Jayce.Spellbook.GetSpell(SpellSlot.Q).Name == "jayceshockblast" ||
                Program.Jayce.Spellbook.GetSpell(SpellSlot.W).Name == "jaycehypercharge" ||
                Program.Jayce.Spellbook.GetSpell(SpellSlot.E).Name == "jayceaccelerationgate")
            {
                Program.Hammer = false;
                Program.Cannon = true;
                Program.RangeQ = Program.CannonQ.Range;
                Program.RangeQExt = Program.CannonQExt.Range;
                Program.RangeW = Program.CannonW.Range;

            }
            if (Program.Jayce.Spellbook.GetSpell(SpellSlot.Q).Name == "JayceToTheSkies" ||
                Program.Jayce.Spellbook.GetSpell(SpellSlot.W).Name == "JayceStaticField" ||
                Program.Jayce.Spellbook.GetSpell(SpellSlot.E).Name == "JayceThunderingBlow")
            {
                Program.Hammer = true;
                Program.Cannon = false;
                Program.RangeQ = Program.HammerQ.Range;
                Program.RangeW = Program.HammerW.Range;
                Program.RangeE = Program.HammerE.Range;
            }
        }

        public static void BurstCombo()
        {
            
            /*switch (Program.Config.Item("burst.style").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    CannonBurstOne();
                    break;
                case 1:
                    
                    break;
                case 2:
                    
                    break;
                case 3:

                    break;
            }*/
        }
        public static void Ext()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy))
            {
                var pos2 = Program.Jayce.Position.Extend(new Vector3(hero.Position.X, hero.Position.Y, Program.Jayce.Position.Z), Program.CannonE.Range);
                if (pos2.Distance(hero.Position) < Program.CannonQ.Range && Program.Jayce.Distance(hero.Position) > Program.CannonE.Range)
                {
                    if (Program.CannonQExt.GetPrediction(hero).CollisionObjects.Count == 0)
                    {
                        Program.CannonE.Cast(pos2);
                    }
                    if (Program.CannonQExt.GetPrediction(hero).Hitchance >= HitChance.VeryHigh)
                    {
                        Program.CannonQExt.Cast(pos2);
                    }
                }
                if (pos2.Distance(hero.Position) < Program.CannonQ.Range && Program.Jayce.Distance(hero.Position) < Program.CannonE.Range)
                {
                    var YUZ = Program.Jayce.Position.Extend(new Vector3(hero.Position.X, hero.Position.Y + 200, Program.Jayce.Position.Z), 200);
                    if (Program.CannonQExt.GetPrediction(hero).CollisionObjects.Count == 0)
                    {
                        Program.CannonE.Cast(YUZ);
                    }
                    if (Program.CannonQExt.GetPrediction(hero).Hitchance >= HitChance.VeryHigh)
                    {
                        Program.CannonQExt.Cast(pos2);
                    }
                }
            }
        }
        public static void JungleExt()
        {
            var mob = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            var pos2 = Program.Jayce.Position.Extend(new Vector3(mob[0].Position.X, mob[0].Position.Y, Program.Jayce.Position.Z), Program.CannonE.Range);
            if (pos2.Distance(mob[0].Position) < Program.CannonQ.Range && Program.Jayce.Distance(mob[0].Position) > Program.CannonE.Range)
            {
                Program.CannonE.Cast(pos2);

                if (Program.CannonQExt.GetPrediction(mob[0]).Hitchance >= HitChance.VeryHigh)
                {
                    Program.CannonQExt.Cast(pos2);
                }
            }
            if (pos2.Distance(mob[0].Position) < Program.CannonQ.Range && Program.Jayce.Distance(mob[0].Position) < Program.CannonE.Range)
            {
                var YUZ = Program.Jayce.Position.Extend(new Vector3(mob[0].Position.X, mob[0].Position.Y + 200, Program.Jayce.Position.Z), 200);
                Program.CannonE.Cast(YUZ);
                if (Program.CannonQExt.GetPrediction(mob[0]).Hitchance >= HitChance.VeryHigh)
                {
                    Program.CannonQExt.Cast(pos2);
                }
            }
        }
        public static void CannonBurstOne() // - still coding stage
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            var burstTarget = TargetSelector.GetTarget(Program.HammerQ.Range, TargetSelector.DamageType.Physical);
            if (burstTarget == null || !burstTarget.IsValid)
            {
                return;
            }

            if (burstTarget.Distance(Program.Jayce.Position) < Program.HammerQ.Range && !Program.Jayce.IsMelee)
            {
                if (Program.CannonQ.IsReady() && Program.CannonE.IsReady())
                {
                    Ext();
                    Program.Stage = 1;
                }
                if (Program.Stage == 1 && Program.CannonW.IsReady() && burstTarget.Distance(Program.Jayce.Position) < 600)
                {
                    Program.CannonW.Cast();
                    Program.Stage = 2;
                }
                if (Program.Stage == 2 && !Program.CannonQ.IsReady() && !Program.CannonW.IsReady() && !Program.CannonE.IsReady() && Program.R.IsReady())
                {
                    Program.R.Cast();
                    Program.Stage = 3;
                }
                if (Program.Stage == 3 && Program.HammerQ.IsReady() && Program.HammerQ.CanCast(burstTarget))
                {
                    Program.HammerQ.Cast(burstTarget);
                    Program.Stage = 4;
                }
                if (Program.Stage == 4 && Program.HammerW.IsReady() && burstTarget.Distance(Program.Jayce.Position) < 600)
                {
                    Program.HammerW.Cast();
                    Program.Stage = 5;
                }
                if (Program.Stage == 5 && Program.HammerE.IsReady() && Program.HammerE.CanCast(burstTarget))
                {
                    Program.HammerE.Cast(burstTarget);
                    Program.Stage = 0;
                }

            }
           
        }
        public static void CannonBurstTwo()
        {

        }
        public static void HammerBurstOne()
        {

        }
        public static void HammerBurstTwo()
        {

        }
        public static bool MenuCheck(string menuName)
        {
            return Program.Config.Item(menuName).GetValue<bool>();
        }
        public static void SkillDraw(float range, Color color, int width)
        {
            Render.Circle.DrawCircle(new Vector3(ObjectManager.Player.Position.X, ObjectManager.Player.Position.Y, ObjectManager.Player.Position.Z), range, color, width);
        }
        public static int CountCheckerino(string menuName)
        {
            return Program.Config.Item(menuName).GetValue<Slider>().Value;
        }
        public static Color SkillColor(string menuName)
        {
            return Program.Config.Item(menuName).GetValue<Circle>().Color;
        }
        public static bool SkillDrawActive(string menuName)
        {
            return Program.Config.Item(menuName).GetValue<Circle>().Active;
        }
        public static void HammerJungleClear()
        {
            if (Program.Hammer)
            {
                var mob = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                if (mob == null || (mob.Count == 0))
                {
                    return;
                }
                if (Program.HammerQ.CanCast(mob[0]))
                {
                    Program.HammerQ.CastOnUnit(mob[0]);
                }
                if (Program.Jayce.Distance(mob[0].Position) < 600)
                {
                    Program.HammerW.Cast();
                }
                if (Program.HammerE.CanCast(mob[0]))
                {
                    Program.HammerE.CastOnUnit(mob[0]);
                }
            }
        }
        public static void CannonJungleClear()
        {
            if (Program.Cannon)
            {
                var mob = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                if (mob == null || (mob.Count == 0))
                {
                    return;
                }
                if (Program.CannonQ.IsReady() && Program.CannonE.IsReady())
                {
                   JungleExt();
                }
                if (Program.CannonQ.IsReady() && !Program.CannonE.IsReady() && Program.CannonQ.CanCast(mob[0]))
                {
                    Program.CannonQ.CastOnUnit(mob[0]);
                }
                if (Program.Jayce.Distance(mob[0].Position) < 600)
                {
                    Program.CannonW.Cast();
                }
            }
        }
        public static float TotalDamage(AIHeroClient target)
        {
            if (target == null)
            {
                return 0f;
            }
            var damage = 0f;
            if (Program.Cannon)
            {
                if (Program.CannonQ.IsReady())
                {
                    var cannonQ = CannonQDamage[Program.CannonQ.Level - 1] + 1.2 * Program.Jayce.FlatPhysicalDamageMod;
                    damage += (float)ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical, cannonQ);

                }
            }
            if (Program.Hammer)
            {
                if (Program.HammerQ.IsReady())
                {
                    var hammerQ = HammerQDamage[Program.HammerQ.Level - 1] + 1 * Program.Jayce.FlatPhysicalDamageMod;
                    damage += (float)ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical, hammerQ);

                }
                if (Program.HammerW.IsReady())
                {
                    var hammerW = HammerWDamage[Program.HammerW.Level - 1] + 0.25*Program.Jayce.AbilityPower();
                    damage += (float) ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, hammerW);
                }
                if (Program.HammerE.IsReady())
                {
                    var hammerE = HammerEDamage[Program.HammerE.Level - 1]/100*target.MaxHealth +
                                  1*Program.Jayce.FlatPhysicalDamageMod;
                    damage += (float) ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical, hammerE);
                }
            }
            return (float)damage;
        }
    }
}
