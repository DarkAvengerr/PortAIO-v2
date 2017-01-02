using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Ekko_master_of_time
{
    class Core
    {
        private string tittle,version;
        public Core()
        {

            tittle = "[Ekko] Master of time Updated June 2016.";
            version = "1.0.0.0";
            OnLoad();
        }
        public Modes _modes;
        public AIHeroClient Hero
        {
            get
            {
                return HeroManager.Player;
            }
        }
        public Spells Spells
        {
            get { return _spells; }
        }

        private Spells _spells;
        private Menu _menu;
        public Menu Menu
        {
            get { return _menu; }
        }
        private void OnLoad()
        {
            if (Hero.ChampionName != "Ekko") return;
            Chat.Print("<b><font color =\"#FF33D6\">"+tittle+ "</font></b> Loaded have fun :)!");

            _menu = new EkkoMenu("Ekko Master of time");

            _spells = new Spells();
            _modes = new EkkoModes(this);
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += Ondraw;
        }

        private void Ondraw(EventArgs args)
        {
            var drawQ = Menu.GetMenu.Item("dq").GetValue<bool>();
            var drawW = Menu.GetMenu.Item("dw").GetValue<bool>();
            var drawE = Menu.GetMenu.Item("de").GetValue<bool>();
            var drawR = Menu.GetMenu.Item("dr").GetValue<bool>();
            if (drawQ)
            {
                if (Spells.Q.IsReady())
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.Q.Range, System.Drawing.Color.GreenYellow);
                }
            }
            if (drawW)
            {
                if (Spells.W.IsReady())
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.W.Range, System.Drawing.Color.BlueViolet);
                }
            }
            if (drawE)
            {
                if (Spells.E.IsReady())
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.E.Range, System.Drawing.Color.DarkViolet);
                }
            }
            if (drawR)
            {
                if (Spells.R.IsReady())
                {
                    var wts = Drawing.WorldToScreen(EkkoUlti().Position);
                    var wtssxt = Drawing.WorldToScreen(HeroManager.Player.ServerPosition);
                    Drawing.DrawLine(wts[0], wts[1], wtssxt[0], wtssxt[1], 5f, System.Drawing.Color.Crimson);
                    Render.Circle.DrawCircle(EkkoUlti().Position, Spells.R.Range, System.Drawing.Color.GreenYellow);
                }
            }
            /*MenuItem dmgAfterComboItem= Menu.GetMenu.Item("DDM");
            //LeagueSharp.Common.Utility.HpBar//DamageIndicator.DamageToUnit += hero => (float)riskCheck.GetDamageInput(new List<Spell> {Spells.Q,Spells.E,Spells.R},hero );
            //LeagueSharp.Common.Utility.HpBar//DamageIndicator.Enabled = true;
            dmgAfterComboItem.ValueChanged += delegate (object sender, OnValueChangeEventArgs eventArgs)
            {
                //LeagueSharp.Common.Utility.HpBar//DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
            };*/
        }

        public Obj_GeneralParticleEmitter EkkoUlti()
        {
      
    return ObjectManager.Get<Obj_GeneralParticleEmitter>().FirstOrDefault(x => x.Name.Equals("Ekko_Base_R_TrailEnd.troy"));
        }
        private void OnUpdate(EventArgs args)
        {
            _modes.Update(this);

        }
    }
}
