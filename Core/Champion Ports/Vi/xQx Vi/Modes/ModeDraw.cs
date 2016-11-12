using System;
using System.Collections.Generic;
using System.Drawing;
using LeagueSharp.Common;
using Vi.Champion;
using Vi.Common;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using CommonGeometry = Vi.Common.CommonGeometry;
using Font = SharpDX.Direct3D9.Font;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Vi.Modes
{
    using System.Linq;
    using LeagueSharp;
    internal class ViQ
    {
        public GameObject Object { get; set; }
        public float NetworkId { get; set; }
        public Vector3 QPos { get; set; }
        public double ExpireTime { get; set; }
    }

    internal class BlueBuff
    {
        public static float StartTime { get; set; }
        public static float EndTime { get; set; }
    }

    internal class RedBuff
    {
        public static float StartTime { get; set; }
        public static float EndTime { get; set; }
    }

    enum PcMode
    {
        NewComputer,
        NormalComputer,
        OldComputer
    }

    internal class ModeDraw
    {
        public static Menu MenuLocal { get; private set; }
        public static Menu SubMenuSpells { get; private set; }
        public static Menu SubMenuBuffs { get; private set; }
        public static Menu SubMenuTimers { get; private set; }
        public static Menu SubMenuManaBarIndicator { get; private set; }
        private static Spell Q => PlayerSpells.Q;
        private static Spell E => PlayerSpells.E;
        private static Spell R => PlayerSpells.R;
        public static PcMode PcMode { get; set; }

        private static readonly List<MenuItem> MenuLocalSubMenuItems = new List<MenuItem>();

        private static readonly string[] pcMode = new[] { "newpc.", "oldpc." };

        private static readonly List<ViQ> ViQ = new List<ViQ>();
        public void Init()
        {
            MenuLocal = new Menu("Drawings", "Drawings");
            {
                MenuLocal.AddItem(new MenuItem("Draw.E1nable", "Enable/Disable Drawings:").SetValue(true)).SetFontStyle(FontStyle.Bold, SharpDX.Color.GreenYellow);
                MenuLocal.AddItem(new MenuItem("DrawPc.Mode", "Adjust settings to your own computer:").SetValue(new StringList(new[] { "New Computer", "Old Computer" }, 0)).SetFontStyle(FontStyle.Regular, SharpDX.Color.Coral)).ValueChanged +=
                 (sender, args) =>
                 {
                     InitRefreshMenuItems();
                 };

                SubMenuManaBarIndicator = new Menu("Mana Bar Combo Indicator", "ManaBarIndicator");
                {
                    for (int i = 0; i < 2; i++)
                    {
                        SubMenuManaBarIndicator.AddItem(new MenuItem(pcMode[i] + "DrawManaBar.Q", "Q:").SetValue(true).SetFontStyle(FontStyle.Regular, Q.MenuColor()));
                        SubMenuManaBarIndicator.AddItem(new MenuItem(pcMode[i] + "DrawManaBar.E", "E:").SetValue(true).SetFontStyle(FontStyle.Regular, E.MenuColor()));
                        SubMenuManaBarIndicator.AddItem(new MenuItem(pcMode[i] + "DrawManaBar.R", "R:").SetValue(true).SetFontStyle(FontStyle.Regular, R.MenuColor()));
                    }
                    MenuLocal.AddSubMenu(SubMenuManaBarIndicator);
                }


                SubMenuSpells = new Menu("Spell Ranges", "DrawSpellRanges");
                {
                    for (int i = 0; i < 2; i++)
                    {
                        SubMenuSpells.AddItem(new MenuItem(pcMode[i] + "Draw.Q1", "Q:").SetValue(new Circle(false, Color.FromArgb(102, 255, 15, 15))).SetFontStyle(FontStyle.Regular, Q.MenuColor()));
                        SubMenuSpells.AddItem(new MenuItem(pcMode[i] + "Draw.E1", "E:").SetValue(new Circle(false, Color.FromArgb(102, 0, 255, 118))).SetFontStyle(FontStyle.Regular, E.MenuColor()));
                        SubMenuSpells.AddItem(new MenuItem(pcMode[i] + "Draw.R1", "R:").SetValue(new Circle(false, Color.FromArgb(102, 255, 13, 236))).SetFontStyle(FontStyle.Regular, R.MenuColor()));
                    }
                    MenuLocal.AddSubMenu(SubMenuSpells);
                }

                SubMenuBuffs = new Menu("Buff Times", "DrawBuffTimes");
                {
                    for (int i = 0; i < 2; i++)
                    {
                        SubMenuBuffs.AddItem(new MenuItem(pcMode[i] + "DrawBuffs", "Show Red/Blue Time Circle").SetValue(new StringList(new[] {"Off", "Blue Buff", "Red Buff", "Both"}, 3)));
                    }
                    MenuLocal.AddSubMenu(SubMenuBuffs);
                }

                for (int i = 0; i < 2; i++)
                {
                    MenuLocal.AddItem(new MenuItem(pcMode[i] + "DrawKillableEnemy", "Killable Enemy Notification").SetValue(true));
                    MenuLocal.AddItem(new MenuItem(pcMode[i] + "DrawKillableEnemyMini", "Killable Enemy [Mini Map]").SetValue(new Circle(true, Color.GreenYellow)));
                }

                for (int i = 0; i < 2; i++)
                {
                    MenuLocal.AddItem(new MenuItem(pcMode[i] + "Draw.MinionLastHit", "Draw Minion Last Hit").SetValue(new StringList(new []{ "Off", "Auto Attack", "Q Damage" }, 2)));
                }


                for (int i = 0; i < 2; i++)
                {
                    var dmgAfterComboItem = new MenuItem(pcMode[i] + "DrawDamageAfterCombo", "Combo Damage").SetValue(true);
                    {
                        MenuLocal.AddItem(dmgAfterComboItem);

                        LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = Common.CommonMath.GetComboDamage;
                        LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = dmgAfterComboItem.GetValue<bool>();
                        dmgAfterComboItem.ValueChanged += delegate (object sender, OnValueChangeEventArgs eventArgs)
                        {
                            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                        };
                    }
                }

                CommonManaBar.Init(MenuLocal);
            }
            ModeConfig.MenuConfig.AddSubMenu(MenuLocal);
            InitRefreshMenuItems();


            Game.OnUpdate += GameOnOnUpdate;
       
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += DrawingOnOnEndScene;
        }

        private void GameOnOnUpdate(EventArgs args)
        {
            
            var drawBuffs = MenuLocal.Item(GetPcModeStringValue + "DrawBuffs").GetValue<StringList>().SelectedIndex;
            if ((drawBuffs == 1 | drawBuffs == 3) && ObjectManager.Player.HasBlueBuff())
            {
                BuffInstance b = ObjectManager.Player.Buffs.Find(buff => buff.DisplayName == "CrestoftheAncientGolem");
                if (BlueBuff.EndTime < Game.Time || b.EndTime > BlueBuff.EndTime)
                {
                    BlueBuff.StartTime = b.StartTime;
                    BlueBuff.EndTime = b.EndTime;
                }
            }

            if ((drawBuffs == 2 | drawBuffs == 3) && ObjectManager.Player.HasRedBuff())
            {
                BuffInstance b = ObjectManager.Player.Buffs.Find(buff => buff.DisplayName == "BlessingoftheLizardElder");
                if (RedBuff.EndTime < Game.Time || b.EndTime > RedBuff.EndTime)
                {
                    RedBuff.StartTime = b.StartTime;
                    RedBuff.EndTime = b.EndTime;
                }
            }
        }

        private static MenuItem GetMenuItems(Menu menu)
        {
            foreach (var j in menu.Children.Cast<Menu>().SelectMany(GetMenu).SelectMany(i => i.Items))
            {
                MenuLocalSubMenuItems.Add(j);
            }

            foreach (var j in menu.Items)
            {
                MenuLocalSubMenuItems.Add(j);
            }
            return null;
        }
        private static IEnumerable<Menu> GetMenu(Menu menu)
        {
            yield return menu;

            foreach (var childChild in menu.Children.SelectMany(GetMenu))
                yield return childChild;
        }
        public static PcMode GetPcModeEnum
        {
            get
            {
                if (MenuLocal.Item("DrawPc.Mode").GetValue<StringList>().SelectedIndex == 0)
                {
                    return PcMode.NewComputer;
                }

                if (MenuLocal.Item("DrawPc.Mode").GetValue<StringList>().SelectedIndex == 1)
                {
                    return PcMode.NormalComputer;
                }

                if (MenuLocal.Item("DrawPc.Mode").GetValue<StringList>().SelectedIndex == 2)
                {
                    return PcMode.OldComputer;
                }

                return PcMode.NormalComputer;
            }
        }

        public static string GetPcModeStringValue => pcMode[MenuLocal.Item("DrawPc.Mode").GetValue<StringList>().SelectedIndex];

        private static void InitRefreshMenuItems()
        {
            int argsValue = MenuLocal.Item("DrawPc.Mode").GetValue<StringList>().SelectedIndex;
            MenuLocalSubMenuItems.Clear();
            GetMenuItems(MenuLocal);

            foreach (var item in MenuLocalSubMenuItems)
            {
                item.Show(true);
                switch (argsValue)
                {
                    case 0:
                        if (!item.Name.StartsWith("newpc.") && !item.Name.StartsWith("DrawPc.Mode") && !item.Name.StartsWith("Draw.E1nable"))
                        {
                            item.Show(false);
                        }
                        break;
                    case 1:
                        if (!item.Name.StartsWith("oldpc.") && !item.Name.StartsWith("DrawPc.Mode") && !item.Name.StartsWith("Draw.E1nable"))
                        {
                            item.Show(false);
                        }
                        break;
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            DrawSpells();
            DrawMinionLastHit();
            KillableEnemy();
            DrawBuffs();
        }


        private static void DrawBuffs()
        {
            var drawBuffs = MenuLocal.Item(GetPcModeStringValue + "DrawBuffs").GetValue<StringList>().SelectedIndex;

            if ((drawBuffs == 1 | drawBuffs == 3) && ObjectManager.Player.HasBlueBuff() )
            {
                if (BlueBuff.EndTime >= Game.Time)
                {
                    var circle1 = new CommonGeometry.Circle2(new Vector2(ObjectManager.Player.Position.X + 3, ObjectManager.Player.Position.Y - 3), 170f, Game.Time - BlueBuff.StartTime, BlueBuff.EndTime - BlueBuff.StartTime).ToPolygon();
                    circle1.Draw(Color.Black, 4);

                    var circle = new CommonGeometry.Circle2(ObjectManager.Player.Position.To2D(), 170f, Game.Time - BlueBuff.StartTime, BlueBuff.EndTime - BlueBuff.StartTime ).ToPolygon();
                    circle.Draw(Color.Blue, 4);
                }
            }

            if ((drawBuffs == 2 || drawBuffs == 3) && ObjectManager.Player.HasRedBuff())
            {
                if (RedBuff.EndTime >= Game.Time)
                {
                    var circle1 = new CommonGeometry.Circle2(new Vector2(ObjectManager.Player.Position.X + 3, ObjectManager.Player.Position.Y - 3), 150f, Game.Time - RedBuff.StartTime, RedBuff.EndTime - RedBuff.StartTime).ToPolygon();
                    circle1.Draw(Color.Black, 4);

                    var circle = new CommonGeometry.Circle2(ObjectManager.Player.Position.To2D(), 150f, Game.Time - RedBuff.StartTime, RedBuff.EndTime - RedBuff.StartTime).ToPolygon();
                    circle.Draw(Color.Red, 4);
                }
            }
        }

        private static void DrawingOnOnEndScene(EventArgs args)
        {
            var drawKillableEnemyMini = MenuLocal.Item(GetPcModeStringValue + "DrawKillableEnemyMini").GetValue<Circle>();
            if (drawKillableEnemyMini.Active)
            {
                foreach (
                    var e in
                        HeroManager.Enemies.Where(
                            e => e.IsHPBarRendered && !e.IsDead && !e.IsZombie && e.Health < Common.CommonMath.GetComboDamage(e)))
                {
                    if ((int) Game.Time%2 == 1)
                    {
                        #pragma warning disable 618
                        LeagueSharp.Common.Utility.DrawCircle(e.Position, 850, drawKillableEnemyMini.Color, 2, 30, true);
                        #pragma warning restore 618
                    }
                }
            }
        }

        private static void DrawSpells()
        {
            var t = TargetSelector.GetTarget(Q.Range + 500, TargetSelector.DamageType.Physical);
            if (t.IsValidTarget())
            {
                var targetBehind = t.Position + Vector3.Normalize(t.ServerPosition - ObjectManager.Player.Position)*80;
                Render.Circle.DrawCircle(targetBehind, 75f, Color.Red, 2);
            }

            var drawQ = MenuLocal.Item(GetPcModeStringValue + "Draw.Q1").GetValue<Circle>();
            if (drawQ.Active && Q.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Q.IsReady() ? drawQ.Color: Color.LightGray, Q.IsReady() ? 5 : 1);
            }

            var drawE = MenuLocal.Item(GetPcModeStringValue + "Draw.E1").GetValue<Circle>();
            if (drawE.Active && E.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, E.IsReady() ? drawE.Color: Color.LightGray, E.IsReady() ? 5 : 1);
            }

            var drawR = MenuLocal.Item(GetPcModeStringValue + "Draw.R1").GetValue<Circle>();
            if (drawR.Active && R.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, E.IsReady() ? drawR.Color : Color.LightGray, E.IsReady() ? 5 : 1);
            }
        }

        public static AIHeroClient GetKillableEnemy
        {
            get
            {
                if (MenuLocal.Item(GetPcModeStringValue + "DrawKillableEnemy").GetValue<bool>())
                {
                    return HeroManager.Enemies.FirstOrDefault(e => e.IsHPBarRendered && !e.IsDead && !e.IsZombie && e.Health < Common.CommonMath.GetComboDamage(e));
                }
                return null;
            }
        }

        private static void KillableEnemy()
        {
            if (MenuLocal.Item(GetPcModeStringValue + "DrawKillableEnemy").GetValue<bool>())
            {
                var t = KillableEnemyAa;
                if (t.Item1 != null && t.Item1.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 800) && t.Item2 > 0)
                {
                    CommonHelper.DrawText(CommonHelper.Text, $"{t.Item1.ChampionName}: {t.Item2} Combo = Kill", (int)t.Item1.HPBarPosition.X + 85, (int)t.Item1.HPBarPosition.Y + 5, SharpDX.Color.GreenYellow);
                }
            }
        }
        private static void DrawMinionLastHit()
        {
            
            var drawMinionLastHit = MenuLocal.Item(GetPcModeStringValue + "Draw.MinionLastHit").GetValue<StringList>().SelectedIndex;
            if (drawMinionLastHit != 0)
            {

                var minions = MinionManager.GetMinions(ObjectManager.Player.Position, (float) (Q.Range * 1.5), MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);

                switch (drawMinionLastHit)
                {
                    case 1:
                    {
                        foreach (var m in minions.ToList().Where(m => m.Health < ObjectManager.Player.TotalAttackDamage)
                            )
                        {
                            Render.Circle.DrawCircle(m.Position, m.BoundingRadius, Color.Wheat);
                        }
                        break;
                    }
                    case 2:
                    {
                        foreach (var m in minions.ToList().Where(m => m.Health < Q.GetDamage(m)))
                        {
                            Render.Circle.DrawCircle(m.Position, m.BoundingRadius, Color.Wheat);
                        }
                        break;
                    }
                }
            }
        }

        private static Tuple<AIHeroClient, int> KillableEnemyAa
        {
            get
            {
                var x = 0;
                var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
                {
                    if (t.IsValidTarget())
                    {
                            if (t.Health <= Common.CommonMath.GetComboDamage(t))
                            {
                            x = (int)Math.Ceiling(t.Health / ObjectManager.Player.TotalAttackDamage);
                        }
                        return new Tuple<AIHeroClient, int>(t, x);
                    }

                }
                return new Tuple<AIHeroClient, int>(t, x);
            }
        }

        public static void DrawText(Font aFont, String aText, int aPosX, int aPosY, SharpDX.Color aColor)
        {
            aFont.DrawText(null, aText, aPosX + 2, aPosY + 2, aColor != SharpDX.Color.Black ? SharpDX.Color.Black : SharpDX.Color.White);
            aFont.DrawText(null, aText, aPosX, aPosY, aColor);
        }
    }
}
