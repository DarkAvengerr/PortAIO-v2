using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hikiMarksmanRework.Champions;
using hikiMarksmanRework.Core.Menus;
using hikiMarksmanRework.Core.Spells;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace hikiMarksmanRework.Core.Utilitys
{
    class Helper
    {
        public static bool Enabled(string menuName)
        {
            return GravesMenu.Config.Item(menuName).GetValue<bool>();
        }

        public static int Slider(string menuName)
        {
            return GravesMenu.Config.Item(menuName).GetValue<Slider>().Value;
        }

        public static bool SEnabled(string menuName)
        {
            return SivirMenu.Config.Item(menuName).GetValue<bool>();
        }

        public static int SSlider(string menuName)
        {
            return SivirMenu.Config.Item(menuName).GetValue<Slider>().Value;
        }

        public static int CSlider(string menuName)
        {
            return CorkiMenu.Config.Item(menuName).GetValue<Slider>().Value;
        }
        public static bool CEnabled(string menuName)
        {
            return CorkiMenu.Config.Item(menuName).GetValue<bool>();
        }
        public static bool EEnabled(string menuName)
        {
            return EzrealMenu.Config.Item(menuName).GetValue<bool>();
        }

        public static int ESlider(string menuName)
        {
            return EzrealMenu.Config.Item(menuName).GetValue<Slider>().Value;
        }

        public static bool LEnabled(string menuName)
        {
            return LucianMenu.Config.Item(menuName).GetValue<bool>();
        }

        public static int LSlider(string menuName)
        {
            return LucianMenu.Config.Item(menuName).GetValue<Slider>().Value;
        }

        public static bool VEnabled(string menuName)
        {
            return VayneMenu.Config.Item(menuName).GetValue<bool>();
        }

        public static int VSlider(string menuName)
        {
            return VayneMenu.Config.Item(menuName).GetValue<Slider>().Value;
        }
        public static bool DEnabled(string menuName)
        {
            return DravenMenu.Config.Item(menuName).GetValue<bool>();
        }

        public static int DSlider(string menuName)
        {
            return DravenMenu.Config.Item(menuName).GetValue<Slider>().Value;
        }

        public static void GravesAntiGapcloser(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs spell)
        {
            if (sender.IsEnemy && spell.End.LSDistance(ObjectManager.Player.Position) < GravesSpells.E.Range && !spell.SData.LSIsAutoAttack() && spell.Target.IsMe)
            {
                foreach (var gapclose in AntiGapcloseSpell.GapcloseableSpells.Where(x => spell.SData.Name == ((AIHeroClient)sender).GetSpell(x.Slot).Name).OrderByDescending(c => Slider("gapclose.slider." + sender.CharData.BaseSkinName)))
                {
                    if (Enabled("gapclose." + ((AIHeroClient)sender).ChampionName))
                    {
                        GravesSpells.E.Cast(ObjectManager.Player.Position.LSExtend(spell.End, -GravesSpells.E.Range));
                    }
                }
            }
        }

        public static void EzrealAntiGapcloser(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs spell)
        {
            if (sender.IsEnemy && spell.End.LSDistance(ObjectManager.Player.Position) < EzrealSpells.E.Range && !spell.SData.LSIsAutoAttack() && spell.Target.IsMe)
            {
                foreach (var gapclose in AntiGapcloseSpell.GapcloseableSpells.Where(x => spell.SData.Name == ((AIHeroClient)sender).GetSpell(x.Slot).Name).OrderByDescending(c => Slider("gapclose.slider."+sender.CharData.BaseSkinName)))
                {
                    if (Enabled("gapclose." + ((AIHeroClient)sender).ChampionName))
                    {
                        EzrealSpells.E.Cast(ObjectManager.Player.Position.LSExtend(spell.End, -EzrealSpells.E.Range));
                    }
                }
            }
        }

        public static void CorkiAntiGapcloser(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs spell)
        {
            if (sender.IsEnemy && spell.End.LSDistance(ObjectManager.Player.Position) < CorkiSpells.E.Range && !spell.SData.LSIsAutoAttack() && spell.Target.IsMe)
            {
                foreach (var gapclose in AntiGapcloseSpell.GapcloseableSpells.Where(x => spell.SData.Name == ((AIHeroClient)sender).GetSpell(x.Slot).Name).OrderByDescending(c => Slider("gapclose.slider." + sender.CharData.BaseSkinName)))
                {
                    if (Enabled("gapclose." + ((AIHeroClient)sender).ChampionName))
                    {
                        CorkiSpells.W.Cast(ObjectManager.Player.Position.LSExtend(spell.End, -CorkiSpells.W.Range));
                    }
                }
            }
        }

        public static void LucianAntiGapcloser(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs spell)
        {
            if (sender.IsEnemy && spell.End.LSDistance(ObjectManager.Player.Position) < LucianSpells.E.Range && !spell.SData.LSIsAutoAttack() && spell.Target.IsMe)
            {
                foreach (var gapclose in AntiGapcloseSpell.GapcloseableSpells.Where(x => spell.SData.Name == ((AIHeroClient)sender).GetSpell(x.Slot).Name).OrderByDescending(c => Slider("gapclose.slider." + sender.CharData.BaseSkinName)))
                {
                    if (Enabled("gapclose." + ((AIHeroClient)sender).ChampionName))
                    {
                        CorkiSpells.W.Cast(ObjectManager.Player.Position.LSExtend(spell.End, -LucianSpells.W.Range));
                    }
                }
            }
        }

        public static void VayneAntiGapcloser(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs spell)
        {
            if (sender.IsEnemy && spell.End.LSDistance(ObjectManager.Player.Position) < VayneSpells.E.Range && !spell.SData.LSIsAutoAttack() && spell.Target.IsMe)
            {
                foreach (var gapclose in AntiGapcloseSpell.GapcloseableSpells.Where(x => spell.SData.Name == ((AIHeroClient)sender).GetSpell(x.Slot).Name).OrderByDescending(c => Slider("gapclose.slider." + sender.CharData.BaseSkinName)))
                {
                    if (Enabled("gapclose." + ((AIHeroClient)sender).ChampionName))
                    {
                        VayneSpells.E.Cast(sender);
                    }
                }
            }
        }

        public static void SivirSpellBlockInit(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs spell)
        {
            if (sender.IsEnemy && !spell.SData.LSIsAutoAttack() && spell.Target.IsMe)
            {
                foreach (var gapclose in EvadeDb.SpellData.SpellDatabase.Spells.Where(x => x.spellName == spell.SData.Name))
                {
                    switch (gapclose.spellType)
                    {
                        case EvadeDb.SpellType.Cone:
                            if (ObjectManager.Player.LSDistance(spell.End) <= 250 && SivirSpells.E.LSIsReady())
                            {
                                SivirSpells.E.Cast();
                            }
                            break;
                        case EvadeDb.SpellType.Line:
                            if (ObjectManager.Player.LSDistance(spell.End) <= 100 && SivirSpells.E.LSIsReady())
                            {
                                SivirSpells.E.Cast();
                            }
                            break;
                        case EvadeDb.SpellType.Circular:
                            if (ObjectManager.Player.LSDistance(spell.End) <= 250 && SivirSpells.E.LSIsReady())
                            {
                                SivirSpells.E.Cast();
                            }
                            break;
                    }
                }
            }
        }
    }
}
