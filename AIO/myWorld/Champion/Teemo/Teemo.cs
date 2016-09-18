using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using myWorld.Library.DamageManager;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace myWorld.Champion.Teemo
{
    
    class Teemo
    {
        static Menu Menu;
        static DamageLib DLib = new DamageLib(ObjectManager.Player);
        static Spell Q;

        public Teemo()
        {
            Menu = Program.MainMenu;

            Q = new Spell(SpellSlot.Q, 400);

            Menu Config = new Menu("Testing Champion", "Testing Champion");
            DLib.RegistDamage("Q", DamageType.Physical, 35f, 20f, new List<DamageType>() { DamageType.Physical, DamageType.Physical }, new List<ScalingType>() { ScalingType.AD, ScalingType.AP }, new List<float>() { 1.1f, 0.4f }, delegate(Obj_AI_Base target) { return Q.IsReady(); }, delegate(Obj_AI_Base target) { return 0f; });

            DLib.AddToMenu(Config, new List<string>() { "Q" });

            Menu.AddSubMenu(Config);
        }
    }
}
