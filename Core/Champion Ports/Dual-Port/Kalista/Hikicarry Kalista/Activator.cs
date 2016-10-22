using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry_Kalista
{
    class Activator
    {
        public static void Youmuu()
        {
            if (Items.CanUseItem(3142))
            {
                Items.UseItem(3142);
            }
        }
        public static void Blade(AttackableUnit target,int theirhp,int myhp)
        {
            var tar = (AIHeroClient)target;
            if (((tar.Health / tar.MaxHealth) < theirhp) && ((ObjectManager.Player.Health / ObjectManager.Player.MaxHealth) < myhp))
            {
                if (Items.CanUseItem(3144))
                {
                    Items.UseItem(3144, tar);
                }
            }
        }
        public static void QuickSilver(string clearignite, string clearexhaust,string zedult,string fizzr,string malzr,string vladr)
        {
            if (!Items.HasItem(3140) && !Items.CanUseItem(3140) && ObjectManager.Player.IsDead && ObjectManager.Player.IsZombie)
            {
                return;
            }
            if (ObjectManager.Player.HasBuffOfType(BuffType.Charm) || ObjectManager.Player.HasBuffOfType(BuffType.Flee) ||
                ObjectManager.Player.HasBuffOfType(BuffType.Polymorph) || ObjectManager.Player.HasBuffOfType(BuffType.Snare) ||
                ObjectManager.Player.HasBuffOfType(BuffType.Stun) || ObjectManager.Player.HasBuffOfType(BuffType.Suppression) ||
                ObjectManager.Player.HasBuffOfType(BuffType.Taunt) && ObjectManager.Player.HasBuffOfType(BuffType.SpellShield) && ObjectManager.Player.HasBuffOfType(BuffType.SpellImmunity))
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
            if (Program.Config.Item(clearignite).GetValue<bool>())
                {
                    if (ObjectManager.Player.HasBuff("summonerdot"))
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
            if (Program.Config.Item(clearexhaust).GetValue<bool>())
                {
                    if (ObjectManager.Player.HasBuff("summonerexhaust"))
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
            if (Program.Config.Item(zedult).GetValue<bool>())
                {
                    if (ObjectManager.Player.HasBuff("zedulttargetmark"))
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
            if (Program.Config.Item(fizzr).GetValue<bool>())
                {
                    if (ObjectManager.Player.HasBuff("FizzMarinerDoom"))
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
            if (Program.Config.Item(malzr).GetValue<bool>())
                {
                    if (ObjectManager.Player.HasBuff("AlZaharNetherGrasp"))
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
            if (Program.Config.Item(vladr).GetValue<bool>())
                {
                    if (ObjectManager.Player.HasBuff("VladimirHemoplague"))
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
        public static void Bilgewater(AttackableUnit target, int theirhp, int myhp)
        {
            var tar = (AIHeroClient)target;
            if (((tar.Health / tar.MaxHealth) < theirhp) && ((ObjectManager.Player.Health / ObjectManager.Player.MaxHealth) < myhp))
            {
                if (Items.CanUseItem(3144))
                {
                    Items.UseItem(3144, tar);
                }
            }
        }
    }
}
