using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Vi.Common;
using SharpDX;
using Vi.Champion;
using Color = SharpDX.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Vi.Modes
{
    internal static class ModeCombo
    {
        public static Menu MenuLocal { get; private set; }
        private static Spell Q => Champion.PlayerSpells.Q;
        private static Spell E => Champion.PlayerSpells.E;
        private static Spell R => Champion.PlayerSpells.R;

                
        public static void Init()
        {
            MenuLocal = new Menu("Combo", "Combo").SetFontStyle(FontStyle.Regular, Color.Aqua);
            MenuLocal.AddItem(new MenuItem("Combo.Mode", "Mode:").SetValue(new StringList(new[] { "Q -> E", "Q -> AA -> E -> AA - > E -> AA" }, 1)).SetFontStyle(FontStyle.Regular, Q.MenuColor())).SetTooltip("Vi's W / Youmuu", Color.AliceBlue);
            MenuLocal.AddItem(new MenuItem("Combo.Q", "Q:").SetValue(new StringList(new[] {"Off", "On"}, 1)).SetFontStyle(FontStyle.Regular, Q.MenuColor()));
            MenuLocal.AddItem(new MenuItem("Combo.Q.KillSteal", "Q Kill Steal:").SetValue(new StringList(new[] { "Off", "On" }, 1)).SetFontStyle(FontStyle.Regular, Q.MenuColor())).SetTooltip("Vi's W / Youmuu", Color.AliceBlue);
            MenuLocal.AddItem(new MenuItem("Combo.E", "E:").SetValue(new StringList(new[] { "Off", "On" }, 1)).SetFontStyle(FontStyle.Regular, E.MenuColor()));
            MenuLocal.AddItem(new MenuItem("Combo.R", "R:").SetValue(new StringList(new[] { "Off", "On" }, 1)).SetFontStyle(FontStyle.Regular, E.MenuColor()));

            ModeConfig.MenuConfig.AddSubMenu(MenuLocal);

            Game.OnUpdate += OnUpdate;
            Orbwalking.BeforeAttack += OrbwalkingOnBeforeAttack;
        }


        private static Dictionary<int, int> JumpingObjects = new Dictionary<int, int>();

        private static void OrbwalkingOnBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {

        }

        private static void OnUpdate(EventArgs args)
        {

            //if (PlayerSpells.LastAutoAttackTick > PlayerSpells.LastSpellCastTick)
            //{
            //    Chat.Print("Last AA");
            //}
            //if (PlayerSpells.LastAutoAttackTick < PlayerSpells.LastSpellCastTick)
            //{
            //    Chat.Print("Last Spell");
            //}
            //ViE

            foreach (var e in HeroManager.Enemies.Where(e => e.ChampionName == "Renekton"))
            {
                foreach (var b in e.Buffs)
                {
                    Console.WriteLine(b.DisplayName + " : " + b.Count);
                }
                
            }
            Console.WriteLine("-------------------------------------------------");


            if (ModeConfig.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                return;
            }

            ExecuteCombo();
        }

        private static void ExecuteCombo()
        {
            var t = CommonTargetSelector.GetTarget(R.Range);

            Q.CastSpellSlot(t);
            E.CastSpellSlot(t);
            R.CastSpellSlot(t);
        }
    }
}
