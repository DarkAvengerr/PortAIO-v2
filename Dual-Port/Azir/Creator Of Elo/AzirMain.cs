using Azir_Free_elo_Machine;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Azir_Creator_of_Elo
{
    internal class AzirMain
    {
        public GapcloserList gapcloserList;
        public List<String> InterruptSpell;
      public readonly string[] Gapcloser = new[]
            {
                "AkaliShadowDance", "Headbutt", "DianaTeleport", "IreliaGatotsu", "JaxLeapStrike", "JayceToTheSkies",
                "MaokaiUnstableGrowth", "MonkeyKingNimbus", "Pantheon_LeapBash", "PoppyHeroicCharge", "QuinnE",
                "XenZhaoSweep", "blindmonkqtwo", "FizzPiercingStrike", "RengarLeap"
            };
       public readonly string[] Interrupt = new[]
           {
                "KatarinaR", "GalioIdolOfDurand", "Crowstorm", "Drain", "AbsoluteZero", "ShenStandUnited", "UrgotSwap2",
                "AlZaharNetherGrasp", "FallenOne", "Pantheon_GrandSkyfall_Jump", "VarusQ", "CaitlynAceintheHole",
                "MissFortuneBulletTime", "InfiniteDuress", "LucianR"
            };
        public SpellSlot Trans(int i)
        {
            switch (i)
            {
                case 0:
                    return SpellSlot.Q;
                case 1:
                    return SpellSlot.W;
                case 2:
                    return SpellSlot.E;
                case 3:
                    return SpellSlot.R;
            }
            return SpellSlot.Q;
        }
        public Menu _menu;
        public AzirModes _modes;
        private string tittle, version;

        public Azir_Creator_of_Elo.Spells Spells { get; private set; }

        public Azir_Creator_of_Elo.Menu Menu => _menu;

        public SoldierManager SoldierManager;
        internal int InterruptNum;
        internal int GapcloserNum;

        public AzirMain()
        {

            tittle = "[Azir]Azir Updated June 2016";
            version = "1.0.1.2";
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        public AIHeroClient Hero => HeroManager.Player;

        private void OnLoad(EventArgs args)
        {
            if (Hero.ChampionName != "Azir") return;

            Chat.Print("<b><font color =\"#FF33D6\">Azir Elo Machine Loaded!</font></b>");
            this.gapcloserList=new GapcloserList();
            _menu = new AzirMenu("Azir Elo Machine",this);
            SoldierManager = new SoldierManager();
            Spells = new Spells();
            _modes = new AzirModes(this);
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += Ondraw;
           
        }

        public void Orbwalk(Vector3 pos)
        {
            Orbwalking.Orbwalk(null, pos);
        }

        private void Ondraw(EventArgs args)
        {
            var drawControl = Menu.GetMenu.Item("dcr").GetValue<bool>();
            var drawFleeMaxRange = Menu.GetMenu.Item("dfr").GetValue<bool>();
            if (drawControl)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 925, System.Drawing.Color.GreenYellow,2);
            var drawLane = Menu.GetMenu.Item("dsl").GetValue<bool>();
            var x = 0;
            if (drawLane)

                foreach (var m in SoldierManager.Soldiers)
                {
                
                    if (m.IsDead) continue;

                    x += HeroManager.Enemies.Count(h => m.LSDistance(h) < 315);

                    Render.Circle.DrawCircle(m.Position, 315,
                        x > 0 ? System.Drawing.Color.GreenYellow : System.Drawing.Color.PaleVioletRed);
                    var wts = Drawing.WorldToScreen(m.Position);
                    var wtssxt = Drawing.WorldToScreen(HeroManager.Player.ServerPosition);

                     Drawing.DrawLine(wts[0], wts[1], wtssxt[0], wtssxt[1], 2f,
                        m.LSDistance(HeroManager.Player) < 950
                            ? System.Drawing.Color.GreenYellow
                            : System.Drawing.Color.PaleVioletRed);
                }

            if (drawFleeMaxRange)
                Render.Circle.DrawCircle(Hero.Position, 1150 + 350, System.Drawing.Color.GreenYellow,2);
            


        }

        private void OnUpdate(EventArgs args)
        {
            _modes.Update(this);


        }
    }
}
