using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Color = SharpDX.Color;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Marksman.Common
{
    class CommonWarningSpelss
    {
        internal class WarningSpells
        {
            public string ChampionName { get; set; }
            public SpellSlot SpellSlot { get; set; }
            public int StartTime{ get; set; }

            public WarningSpells(string championName, SpellSlot spellSlot, int startTime = 0)
            {
                ChampionName = championName;
                SpellSlot = spellSlot;
                StartTime = startTime;
            }

            public WarningSpells() { }
        }

        public static LeagueSharp.Common.Menu MenuLocal { get; private set; }

        public static List<WarningSpells> WarningSpellList = new List<WarningSpells>();

        private static bool Active => MenuLocal.Item("WarningSpells.Enable").GetValue<bool>();

        public static void Initialize(LeagueSharp.Common.Menu nParentMenu)
        {
            WarningSpellList.Add(new WarningSpells
            {
                ChampionName = "LeeSin",
                SpellSlot = SpellSlot.Q
            });

            WarningSpellList.Add(new WarningSpells
            {
                ChampionName = "Rengar",
                SpellSlot = SpellSlot.Q
            });

            WarningSpellList.Add(new WarningSpells
            {
                ChampionName = "Kled",
                SpellSlot = SpellSlot.R
            });

            WarningSpellList.Add(new WarningSpells
            {
                ChampionName = "Rengar",
                SpellSlot = SpellSlot.R
            });

            WarningSpellList.Add(new WarningSpells
            {
                ChampionName = "Shaco",
                SpellSlot = SpellSlot.Q
            });

            WarningSpellList.Add(new WarningSpells
            {
                ChampionName = "MonkeyKing",
                SpellSlot = SpellSlot.W
            });

            WarningSpellList.Add(new WarningSpells
            {
                ChampionName = "KhaZix",
                SpellSlot = SpellSlot.R
            });

            WarningSpellList.Add(new WarningSpells
            {
                ChampionName = "Twitch",
                SpellSlot = SpellSlot.Q
            });

            MenuLocal = new LeagueSharp.Common.Menu("Warning Spells", "WarningSpells");
            {
                MenuLocal.AddItem(new MenuItem("WarningSpells.Enable", "Enable").SetValue(true).SetFontStyle(FontStyle.Regular, Color.GreenYellow));

                foreach (var ws in WarningSpellList.OrderBy(o => o.ChampionName))
                {
                    MenuLocal.AddItem(new MenuItem("ws_" + ws.ChampionName + ws.SpellSlot, ws.ChampionName + ": " + ws.SpellSlot).SetValue(true));
                }
            }
            nParentMenu.AddSubMenu(MenuLocal);

            Obj_AI_Base.OnProcessSpellCast += ObjAiBaseOnOnProcessSpellCast;
            Drawing.OnEndScene += DrawingOnOnEndScene;
        }

        private static void ObjAiBaseOnOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!Active)
            {
                return;
            }

            if (sender.Type != GameObjectType.AIHeroClient)
            {
                return;
            }

            if (sender.Team == ObjectManager.Player.Team)
            {
                return;
            }

            if (sender.IsEnemy && sender is AIHeroClient && !sender.IsVisible)
            {
                foreach (
                    var ws in
                        WarningSpellList
                            .Where(w => MenuLocal.Item("ws_" + w.ChampionName + w.SpellSlot).GetValue<bool>())
                            .Where(w => ((AIHeroClient) sender).ChampionName.ToLower() == w.ChampionName.ToLower())
                            .Where(w => args.Slot == w.SpellSlot))
                {
                    ws.StartTime = Environment.TickCount;
                }
            }
        }

        private static void DrawingOnOnEndScene(EventArgs args)
        {
            if (!Active)
            {
                return;
            }

            if (Drawing.Direct3DDevice == null || Drawing.Direct3DDevice.IsDisposed)
            {
                return;
            }

            foreach (var w in WarningSpellList.Where(w => w.StartTime + 2000 > Environment.TickCount))
            {

                CommonGeometry.DrawText(CommonGeometry.TextWarning, "WARNING! " + w.ChampionName + " casted " + w.SpellSlot + "!", Drawing.Width * 0.22f, Drawing.Height * 0.44f, SharpDX.Color.White);
            }

            //CommonGeometry.DrawText(CommonGeometry.TextPassive, time, position.X - 50 + 105, +position.Y - 52, SharpDX.Color.AntiqueWhite)
        }

    }
}
