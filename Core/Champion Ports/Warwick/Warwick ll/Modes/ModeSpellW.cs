using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using WarwickII.Champion;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace WarwickII.Modes
{
    internal class ModeSpellW
    {
        public static Menu MenuLocal { get; private set; }
        public static Menu MenuSkins { get; private set; }
        public static Menu MenuSettingQ { get; private set; }
        public static Menu MenuSettingE { get; private set; }
        public static Menu MenuFlame { get; private set; }
        public static Menu MenuSpellR { get; private set; }

        public static void Init(Menu MenuParent)
        {
            MenuLocal = new Menu("W:", "Mode.W");

            foreach (var ally in HeroManager.Allies.Where(a => a != ObjectManager.Player))
            {
                MenuLocal.AddItem(new MenuItem("Mode.W" + ally.ChampionName, ally.ChampionName).SetValue(false));
            }

            MenuParent.AddSubMenu(MenuLocal);
        }

        public virtual void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (PlayerSpells.W.IsReady())
            {
                var objAiHero = sender as AIHeroClient;

                if (objAiHero == null || args.Target == null || !objAiHero.IsAlly)
                {
                    return;
                }

                if (!args.Target.IsEnemy && !(args.Target is Obj_AI_Turret) && !(args.Target is Obj_Barracks))
                {
                    return;
                }

                if (sender.Type != GameObjectType.AIHeroClient || !args.SData.IsAutoAttack())
                {
                    return;
                }

                if (!MenuLocal.Item("Mode.W" + objAiHero.ChampionName).GetValue<bool>())
                {
                    return;
                }

                if (objAiHero.Distance(ObjectManager.Player) > PlayerSpells.W.Range)
                {
                    return;
                }

                PlayerSpells.W.Cast();
            }
        }
    }
}
