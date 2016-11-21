using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;


using EloBuddy; 
using LeagueSharp.Common; 
 namespace SergixBrand
{
    public class Core
    {
        private string TittleAssembly, VersionAssembly;
        private string championName, menuTittle;
        public AIHeroClient Hero => HeroManager.Player;
        private Menu _menu;
        private Modes _modes;
        private BrandSpells _spells;
        public BrandSpells GetSpells => _spells;
        public Menu GetMenu => _menu;
        public Modes getModes => _modes;
        public void SetVersion(string version)
        {
            this.VersionAssembly = version;
        }

        public void SetTittleAssembly(string tittleA)
        {
            this.TittleAssembly = tittleA;
        }

        public Core(string championName, string menuTittle)
        {
            this.championName = championName;
            this.menuTittle = this.menuTittle;
            OnLoadChampion();
        }
        public void cast(Obj_AI_Base target, Spell spell, int h)
        {
            // spell.cast
            // OktwCommon.
            if (target == null) return;
            var mode = this.GetMenu.GetMenu.Item("Prediction").GetValue<StringList>().SelectedIndex;
            if (mode == 0)
            {
                try
                {
                    var time = this.Hero.Distance(target) / spell.Speed;
                    var pred = SebbyLib.Prediction.Prediction.GetPrediction(target, time);
                    if (pred.Hitchance >= SebbyLib.Prediction.HitChance.VeryHigh)
                        spell.Cast(pred.CastPosition);
                }
                catch
                {
                    
                    spell.CastIfHitchanceEquals(
                         target, LeagueSharp.Common.HitChance.VeryHigh);
                }
            }
            else
            {


                spell.CastIfHitchanceEquals(
                     target,
                     LeagueSharp.Common.HitChance.VeryHigh);
            }

        }
        private void OnLoadChampion()
        {
            if (Hero.ChampionName != championName) return;
            _menu = new BrandMenu("Brand", this);
            _spells = new BrandSpells();
            new DrawDamage(this,
                new List<Damage>()
                {
                    new Damage("Q", GetSpells.GetQ, Color.AliceBlue),
                    new Damage("W", GetSpells.GetW, Color.AliceBlue),
                    new Damage("E", GetSpells.GetE, Color.AliceBlue)
                });
         //   var events = new GameEvents();
            _modes = new BrandModes();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;

        }

        public virtual void OnUpdate(EventArgs args)
        {
            _modes.Update(this);
        }

        public virtual void OnDraw(EventArgs args)
        {
            var drawQ = GetMenu.GetMenu.Item("DQ").GetValue<bool>();
            var drawW = GetMenu.GetMenu.Item("DW").GetValue<bool>();
            var drawE = GetMenu.GetMenu.Item("DE").GetValue<bool>();
            var drawR = GetMenu.GetMenu.Item("DR").GetValue<bool>();
            if (GetSpells.GetQ.IsReady() && drawQ)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, GetSpells.GetQ.Range, System.Drawing.Color.DarkCyan, 2);
            if (GetSpells.GetW.IsReady() && drawW)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, GetSpells.GetW.Range, System.Drawing.Color.DarkCyan, 2);
            if (GetSpells.GetE.IsReady() && drawE)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, GetSpells.GetE.Range, System.Drawing.Color.DarkCyan, 2);
            if (GetSpells.GetR.IsReady() && drawR)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, GetSpells.GetR.Range, System.Drawing.Color.DarkCyan, 2);

        }
    }
}

