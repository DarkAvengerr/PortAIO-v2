using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using myWorld.Library.MenuWarpper;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace myWorld.Library.STS
{
    class SimpleTS
    {
        static AIHeroClient myHero = ObjectManager.Player;
        static STS_DefineModeType NEAR_MOUSE = new STS_DefineModeType(0, "Near mouse", delegate(AIHeroClient x, AIHeroClient y) 
            {
                return x.Distance(Game.CursorPos).CompareTo(y.Distance(Game.CursorPos));
            });
        static STS_DefineModeType LESS_CAST_MAGIC = new STS_DefineModeType(1, "Less cast (magic)", delegate(AIHeroClient x, AIHeroClient y)
            {
                return myHero.CalcDamage(x, Damage.DamageType.Magical, 100).CompareTo(myHero.CalcDamage(y, Damage.DamageType.Magical, 100));
            });
        static STS_DefineModeType LESS_CAST_PHYSICAL = new STS_DefineModeType(2, "Less cast (physical)", delegate(AIHeroClient x, AIHeroClient y)
            {
                return myHero.CalcDamage(x, Damage.DamageType.Physical, 100).CompareTo(myHero.CalcDamage(y, Damage.DamageType.Physical, 100));
            });
        static STS_DefineModeType LOW_HP = new STS_DefineModeType(3, "Low HP", delegate(AIHeroClient x, AIHeroClient y)
            {
                return x.Health.CompareTo(y.Health);
            });
        static STS_DefineModeType MOST_AD = new STS_DefineModeType(4, "Most AD", delegate(AIHeroClient x, AIHeroClient y)
            {
                return (x.BaseAttackDamage + x.FlatPhysicalDamageMod).CompareTo(y.BaseAttackDamage + y.FlatPhysicalDamageMod);
            });
        static STS_DefineModeType MOST_AP = new STS_DefineModeType(5, "Most AP", delegate(AIHeroClient x, AIHeroClient y)
            {
                return (x.BaseAbilityDamage + x.FlatMagicDamageMod).CompareTo(y.BaseAbilityDamage + y.FlatMagicDamageMod);
            });
        static STS_DefineModeType CLOSET = new STS_DefineModeType(6, "Closet", delegate(AIHeroClient x, AIHeroClient y)
            {
                return Geometry.Distance(x, myHero).CompareTo(Geometry.Distance(y, myHero));
            });

        STS_DefineModeType[] AVAILABLE_MODE = new STS_DefineModeType[7] { NEAR_MOUSE, LESS_CAST_MAGIC, LESS_CAST_PHYSICAL, LOW_HP, MOST_AD, MOST_AP, CLOSET };

        int basicMode;
        AIHeroClient ForceTarget = (AIHeroClient)null;

        Menu Config;

        public SimpleTS(int mode = 2)
        {
            this.basicMode = mode;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnWndProc += Game_OnWndProc;
        }

        public void AddToMenu(Menu m)
        {
            Config = new Menu("SimpleTS", "SimpleTS");

            Menu tp = new Menu("Target Priority", "SimpleTS.STS");
            List<AIHeroClient> enemys = HeroManager.Enemies;
            for(int i = 0; i < enemys.Count; i ++)
            {
                tp.AddItem(new MenuItem("SimpleTS.STS." + enemys[i].GetHashCode(), enemys[i].ChampionName).SetValue(new Slider(1, 1, 5)));
            }
            tp.AddItem(new MenuItem("SimpleTS.STS.Info", "5 Highest priority"));
            Config.AddSubMenu(tp);

            List<string> l1 = new List<string>();
            foreach(STS_DefineModeType data in AVAILABLE_MODE)
            {
                l1.Add(data.name);
            }
            Config.AddItem(new MenuItem("SimpleTS.mode", "Targetting Mode: ").SetValue(new StringList(l1.ToArray(), basicMode)));
            Config.AddItem(new MenuItem("SimpleTS.selected", "Focus selected target").SetValue(true));
            m.AddSubMenu(Config);

        }

        void Game_OnWndProc(WndEventArgs args)
        {
            if(args.Msg == (uint)WindowsMessages.WM_LBUTTONDOWN)
            {
                ForceTarget = HeroManager.Enemies.FindAll(hero => hero.IsValid && hero.Distance(Game.CursorPos, true) < 40000).OrderBy(h => h.Distance(Game.CursorPos, true)).FirstOrDefault();
            }
        }

        void Drawing_OnDraw(EventArgs args)
        {
            //throw new NotImplementedException();
            AIHeroClient selected = SelectedTarget();
            if (selected != null && Config.Item("SimpleTS.selected").GetValue<bool>() && selected.IsValid)
            {
                Render.Circle.DrawCircle(selected.ServerPosition, 100, System.Drawing.Color.Aqua);
            }
        }

        public AIHeroClient SelectedTarget()
        {
            return ForceTarget;
        }


        public AIHeroClient GetTarget(float range)
        {
            return HeroManager.Enemies.FindAll(x => !x.IsDead && Geometry.Distance(x, myHero) < range && LeagueSharp.Common.Utility.IsValidTarget(x)).OrderBy(x => AVAILABLE_MODE[Config.GetListIndex("SimpleTS.mode")].sortfunc).FirstOrDefault();
        }

        public List<AIHeroClient> GetTarget(float range, int n = 1)
        {
            IEnumerable<AIHeroClient> heroes = HeroManager.Enemies.FindAll(x => !x.IsDead && Geometry.Distance(x, myHero) < range && LeagueSharp.Common.Utility.IsValidTarget(x)).OrderBy(x => AVAILABLE_MODE[Config.GetListIndex("SimpleTS.mode")].sortfunc);

            List<AIHeroClient> l1 = new List<AIHeroClient>();
            foreach(AIHeroClient hero in heroes)
            {
                l1.Add(hero);
            }
            return l1;
        }
    }
}
