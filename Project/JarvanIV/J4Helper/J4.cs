using System;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace J4Helper
{
    internal class J4
    {
        public static AIHeroClient Player;
        public const string ChampionName = "JarvanIV";
        public static Spell Q, W, E, R;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Menu Config;
        public static List<Vector3> EQDrawList; 

        public J4()
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            EQDrawList = new List<Vector3>();
            Player = ObjectManager.Player;
            if (Player.ChampionName != ChampionName) return;
            Q = new Spell(SpellSlot.Q, 700f);
            Q.SetSkillshot(0.5f, 70f, float.MaxValue, false, SkillshotType.SkillshotLine);

            W = new Spell(SpellSlot.W, 300f);

            E = new Spell(SpellSlot.E, 830f);
            E.SetSkillshot(0.5f, 70f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            Config = new Menu("J4Helper", "J4Helper", true);
            //Orbwalker
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            //Misc
            Config.AddSubMenu(new Menu("Keys", "Keys"));
            Config.SubMenu("Keys")
                .AddItem(
                    new MenuItem("EQMouse", "EQ to Mouse").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            Config.AddToMainMenu();
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Chat.Print("J4Helper Loaded.");
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            /*if (args.Target.Equals(Player))
            {
                args.SData.
            }*/
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            /*Drawing.DrawText(Player.HPBarPosition.X + 40, Player.HPBarPosition.Y + 30, Color.NavajoWhite, "Shield: {0}", GetPossibleShieldAmount());
            List<Vector3> vectors = EQDrawList;
            foreach (Vector3 v in vectors)
            {
                Drawing.DrawCircle(v, 30, Color.NavajoWhite);
                Drawing.DrawText(v.X, v.Y, Color.NavajoWhite, "EQ");
            }*/
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (GetPossibleShieldAmount() == MaxShield && W.IsReady())
            {
                W.Cast();
            }
            if (Config.Item("EQMouse").IsActive())
            {
                FlagSwag();
            }
            foreach (var unit in Player.GetEnemiesInRange(E.Range))
            {
                EQDrawList.Add(E.GetPrediction(unit).CastPosition);
            }
        }

        private static void FlagSwag()
        {
            var cursorPos = Game.CursorPos;
            if (Q.IsReady() && E.IsReady())
            {
                E.Cast(cursorPos);
                LeagueSharp.Common.Utility.DelayAction.Add(5, () => { Q.Cast(cursorPos); });
                
            }
        }

        private static int GetPossibleShieldAmount()
        {
            var level = W.Level;

            var baseShield = 50 + (40*(level - 1));
            var baseExtraShield = 20 + (10*(level - 1));
            var enemyCount = Player.CountEnemiesInRange(W.Range);
            var shieldAmount = baseShield + baseExtraShield*enemyCount;
            return shieldAmount > MaxShield ? MaxShield : shieldAmount;
        }

        private static int MaxShield
        {
            get
            {
                return 150 + (90 * (W.Level - 1));
            }
        }


    }
}
