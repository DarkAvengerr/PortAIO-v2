using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using myWorld.Library.MenuWarpper;
using myWorld.Library.DamageManager;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace myWorld.Library.SummonerManager
{
    public enum SummonerType
    {
        CLEAN,
        ATTACK,
        REVIVE,
        HEAL,
    }
    class SummonerManager
    {
        Dictionary<string, Summoner> SummonerSpells = new Dictionary<string, Summoner>();
        DamageLib DLib = new DamageLib(ObjectManager.Player);

        public SummonerManager()
        {
            SummonerSpells.Add("IGNITE", new Summoner("sommonerdot", 600));
        }

        public void AddToMenu(Menu Menu)
        {
            Menu SummonerManager = new Menu("SummonerManager", "SummonerManager");

            if(SummonerSpells["IGNITE"].IsExist())
            {
                Menu Ignite = new Menu("Ignite", "Ignite");
                Ignite.AddBool("Ignite.Use", "Use Ignite");
                Ignite.AddBool("Ignite.Killsteal", "Use Ignite to killsteal");
            }
        }
    }
}
