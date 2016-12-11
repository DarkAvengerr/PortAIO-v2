using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
using LeagueSharp.Common; 
namespace MasterOfInsec
{
    using System.Runtime.Serialization;

    using ObjectManager = EloBuddy.ObjectManager;

    internal static class NormalInsec
    {
        /// <summary>
        /// The steps.
        /// </summary>
        public enum steps
        {
            Q1 = 0,

            Q2 = 1,

            WardJump = 2,

            Flash = 3,

            R = 4,

        }

        public static steps Steps;

        public static AIHeroClient insecAlly;

        public static AIHeroClient insecEnemy;

        private static bool insecActive;

        public static void ResetInsecStats()
        {
            //     beforeall = false;
            insecActive = false;
            Steps = steps.Q1;
        }

        public static Vector3 Insecpos(AIHeroClient ts)
        {
            return Game.CursorPos.Extend(ts.Position, Game.CursorPos.Distance(ts.Position) + 250);
        }

        public static Vector3 GetInsecPos(AIHeroClient target)
        {
            if (Program.menu.Item("Mode").GetValue<StringList>().SelectedIndex == 0)
            {
                return WardJump.InsecposTower(target); // insec torre
                //  Chat.Print("");
            }
            else if (Program.menu.Item("Mode").GetValue<StringList>().SelectedIndex == 1)
            {
                return WardJump.InsecposToAlly(insecEnemy, insecAlly); //insec ally  
            }
            else if (Program.menu.Item("Mode").GetValue<StringList>().SelectedIndex == 2)
            {
                return WardJump.Insecpos(target); // insec normal
            }

            return WardJump.Insecpos(target);
        }

        /// <summary>
        /// The r cast.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        public static void RCast(AIHeroClient target)
        {

            Program.R.CastOnUnit(target);

        }

        /// <summary>
        /// The combo where we select target
        /// </summary>
        public static void Combo()
        {
            if (Program.menu.Item("OrbwalkInsec").GetValue<bool>())
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Program.Player.Position.Extend(Game.CursorPos, 150));
            }
            if (!Program.R.IsReady())
            {
                return;
            }
            var target = TargetSelector.GetTarget(1300, TargetSelector.DamageType.Physical);
            if (target == null) return;
            Do(target);
        }

        /*    public static bool ObjisInRange(Obj_AI_Base target , Obj_AI_Base target2 , float range)
        {
                if (target2.Distance(target) < range)
                {
                    return true;
                }        
            return false;
        }*/

        public static void Do(AIHeroClient target)
        {
            var minion =
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(x => x.IsEnemy && Program.Q.CanCast(x) && Program.Q.IsInRange(x))
                    .FirstOrDefault<Obj_AI_Base>();

            if (insecActive == false)
            {
                if (Program.Q.IsReady()
                    && ((Program.W.IsReady() && WardJump.getBestWardItem().IsValidSlot())
                        || (Program.menu.Item("useflash").GetValue<bool>()
                            && ObjectManager.Player.Spellbook.GetSpell(
                                ObjectManager.Player.GetSpellSlot("SummonerFlash")).IsReady())) && Program.R.IsReady()
                    && Program.Player.Mana >= 130)
                {
                    if (Program.Player.Distance(target) <= 500)
                    {
                        Steps = steps.WardJump;

                    }
                    insecActive = true;
                    WardJump.wardj = false;
                }
            }
            if (!insecActive) return;
            if (target.IsValidTarget(Program.Q.Range))
            {
                if (Steps == steps.Q1)
                {
                    if (Program.Q.IsReady()
                        && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Name == "BlindMonkQOne")
                    {
                       Program.cast(target,Program.Q);

                    }


                }
                if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Name.ToLower() == "blindmonkqtwo")
                {
                    if (Program.Q.Cast())
                    {
                        if (!WardJump.getBestWardItem().IsValidSlot() && Program.menu.Item("useflash").GetValue<bool>())
                        {
                            Steps = steps.Flash;
                        }
                        else
                        {
                            Steps = steps.WardJump;
                        }
                    }

                }
            }
             if (Steps == steps.WardJump) // put ward
            {
                if (Program.W.IsReady())
                {

                    WardJump.JumpTo(GetInsecPos(target));


                }
            }
            else if (Steps == steps.Flash) // hit w
            {
                if (WardJump.Insecpos(target).Distance(Program.Player.Position) < 400)
                {
                    ObjectManager.Player.Spellbook.CastSpell(
                        ObjectManager.Player.GetSpellSlot("SummonerFlash"),
                        GetInsecPos(target));
                    Steps = steps.R;
                }
            }
            else if (Steps == steps.R) // and hit the kick
            {
                RCast(target);
            }
            else
            {
                //    insecActive = false;
                //         Steps = steps.Q1;
            }

        }
    }

}
