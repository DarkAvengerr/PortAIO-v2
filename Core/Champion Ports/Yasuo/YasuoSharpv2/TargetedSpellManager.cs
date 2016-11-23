using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LeagueSharp;
using LeagueSharp.Common;
using Menu = LeagueSharp.Common.Menu;
using MenuItem = LeagueSharp.Common.MenuItem;


using EloBuddy; 
using LeagueSharp.Common; 
 namespace YasuoSharpV2
{
    
    public class TargetedSpell
    {
        public AIHeroClient owner;
        public SpellDataInst spellInst;
        public SpellSlot slot;
        public String name;
        public String missleName;
    }

    public class TargetedMissle
    {
        public MissileClient missle;
        public int blockBelowHP;
    }

    public class TargetedSpellManager
    {
        public static List<TargetedSpell> blockTargetedSpells = new List<TargetedSpell>();

        public static List<TargetedMissle> targatedMissales = new List<TargetedMissle>(); 


        public static Menu setUp()
        {
            var menu = new Menu("Targeted spells", "tSpells");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.IsEnemy))
            {
                menu.AddSubMenu(new Menu(enemy.ChampionName, "tar_" + enemy.ChampionName));
                foreach (var spell in enemy.Spellbook.Spells.Where(spl => (spl.SData.TargettingType == SpellDataTargetType.Unit || spl.SData.TargettingType == SpellDataTargetType.SelfAndUnit) && spl.SData.MissileSpeed >300))
                {

                    if(spell.Slot != SpellSlot.Q && spell.Slot != SpellSlot.W && spell.Slot != SpellSlot.E && spell.Slot != SpellSlot.R)
                        continue;


                    //All targeted spells
                    blockTargetedSpells.Add(new TargetedSpell()
                    {
                        owner = enemy,
                        spellInst = spell,
                        slot = spell.Slot,
                        name = spell.SData.DisplayName,
                        missleName = spell.SData.MissileBoneName
                    });

                    menu.SubMenu("tar_" + enemy.ChampionName)
                        .AddItem(new MenuItem("block_" + enemy.ChampionName + "_" + spell.Name, " " + spell.Slot + ": "+spell.SData.Name + ": % HP"))
                        .SetValue(new Slider(100));
                }
            }
            return menu;
        }

        public static int blockSpellOnHP(String champName, String spellName)
        {
            var mItemName = "block_" + champName + "_" + spellName;
            if (YasuoSharp.Config.Item(mItemName) != null)
            {
                return YasuoSharp.Config.Item(mItemName).GetValue<Slider>().Value;
            }
            return 0;
        }

        public static void deleteActiveSpell(int netId)
        {
        }

        public static void addActiveSpell(MissileClient spell)
        {
        }

    }

}
