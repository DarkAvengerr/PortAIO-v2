using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SCommon;
using SCommon.PluginBase;
using SCommon.Prediction;
using SCommon.Orbwalking;
using SUtility.Drawings;
using SharpDX;


namespace SAutoCarry.Champions
{
    public class Kalista : SCommon.PluginBase.Champion
    {
        public Kalista()
            : base ("Kalista", "SAutoCarry - Kalista")
        {

        }

        public override void CreateConfigMenu()
        {
            Menu combo = new Menu("Combo", "SAutoCarry.Kalista.Combo");
            combo.AddItem(new MenuItem("SAutoCarry.Kalista.Combo.UseQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("SAutoCarry.Kalista.Combo.UseE", "Use E").SetValue(true));
        }

        public override void SetSpells()
        {
            
        }
    }
}
