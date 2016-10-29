using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace adcUtility.Activator
{
    public static class Quick_Silver_Sash
    {
        private static Obj_AI_Base adCarry = null;
        public static bool hikiQSS { get; set; }
        public static Obj_AI_Base adcQSS
        {
            get
            {
                if (adCarry != null && adCarry.IsValid)
                {
                    return adCarry;
                }
                return null;
            }
        }
        static Quick_Silver_Sash()
        {
            Game.OnUpdate += Game_OnUpdate;
            adCarry = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(x => x.IsMe);
            if (adCarry != null)
            {
                Console.Write(adCarry.CharData.BaseSkinName);
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (adCarry == null)
            {
                adCarry = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(x => x.IsMe);
            }
            if (adCarry != null)
            {
                //AhriSeduce = Ahri E Buff
                var useQSS = Program.Config.Item("use.qss").GetValue<bool>();  // use qss
                var clearIgnite = Program.Config.Item("clear.ignite").GetValue<bool>(); // clear ignite
                var clearExhaust = Program.Config.Item("clear.exhaust").GetValue<bool>(); // clear exhaust
                var clearZedR = Program.Config.Item("clear.zedult").GetValue<bool>(); // clear zed r
                var clearFizzR = Program.Config.Item("clear.fizzult").GetValue<bool>(); // clear fizz r
                var clearMalzR = Program.Config.Item("clear.malzaharult").GetValue<bool>(); // clear malz r
                var clearVladR = Program.Config.Item("clear.vladulti").GetValue<bool>(); // clear vlad r

                if (useQSS)
                {
                    if (adCarry.HasBuffOfType(BuffType.Charm) || adCarry.HasBuffOfType(BuffType.Flee) ||
                    adCarry.HasBuffOfType(BuffType.Polymorph) || adCarry.HasBuffOfType(BuffType.Snare) ||
                    adCarry.HasBuffOfType(BuffType.Stun) || adCarry.HasBuffOfType(BuffType.Suppression) ||
                    adCarry.HasBuffOfType(BuffType.Taunt) || adCarry.HasBuff("AhriSeduce") && 
                    !adCarry.HasBuffOfType(BuffType.SpellShield) && 
                    !adCarry.HasBuffOfType(BuffType.SpellImmunity))
                    {
                        if (Items.HasItem(3140) && Items.CanUseItem(3140))
                        {
                            Items.UseItem(3140);
                        }
                        if (Items.HasItem(3139) && Items.CanUseItem(3139))
                        {
                            Items.UseItem(3139);
                        }
                    }
                    if (clearIgnite)
                    {
                        if (adCarry.HasBuff("summonerdot"))
                        {
                            if (Items.HasItem(3140) && Items.CanUseItem(3140))
                            {
                                Items.UseItem(3140);
                            }
                            if (Items.HasItem(3139) && Items.CanUseItem(3139))
                            {
                                Items.UseItem(3139);
                            }
                        }
                    }
                    if (clearExhaust)
                    {
                        if (adCarry.HasBuff("summonerexhaust"))
                        {
                            if (Items.HasItem(3140) && Items.CanUseItem(3140))
                            {
                                Items.UseItem(3140);
                            }
                            if (Items.HasItem(3139) && Items.CanUseItem(3139))
                            {
                                Items.UseItem(3139);
                            }
                        }
                    }
                    if (clearZedR)
                    {
                        if (adCarry.HasBuff("zedulttargetmark"))
                        {
                            if (Items.HasItem(3140) && Items.CanUseItem(3140))
                            {
                                Items.UseItem(3140);
                            }
                            if (Items.HasItem(3139) && Items.CanUseItem(3139))
                            {
                                Items.HasItem(3140);
                            }
                        }
                    }
                    if (clearFizzR)
                    {
                        if (adCarry.HasBuff("FizzMarinerDoom"))
                        {
                            if (Items.HasItem(3140) && Items.CanUseItem(3140))
                            {
                                Items.UseItem(3140);
                            }
                            if (Items.HasItem(3139) && Items.CanUseItem(3139))
                            {
                                Items.HasItem(3140);
                            }
                        }

                    }
                    if (clearMalzR)
                    {
                        if (adCarry.HasBuff("AlZaharNetherGrasp"))
                        {
                            if (Items.HasItem(3140) && Items.CanUseItem(3140))
                            {
                                Items.UseItem(3140);
                            }
                            if (Items.HasItem(3139) && Items.CanUseItem(3139))
                            {
                                Items.HasItem(3140);
                            }
                        }
                    }
                    if (clearVladR)
                    {
                        if (adCarry.HasBuff("VladimirHemoplague"))
                        {
                            if (Items.HasItem(3140) && Items.CanUseItem(3140))
                            {
                                Items.UseItem(3140);
                            }
                            if (Items.HasItem(3139) && Items.CanUseItem(3139))
                            {
                                Items.HasItem(3140);
                            }
                        }
                    }
                }

            }
        }
    }
}