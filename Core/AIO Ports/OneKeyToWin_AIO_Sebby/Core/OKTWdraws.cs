using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using SebbyLib;

using EloBuddy;
using LeagueSharp.Common;
using PortAIO.Core.AIO_Ports.OneKeyToWin_AIO_Sebby;

namespace OneKeyToWin_AIO_Sebby.Core
{
    class OKTWdraws
    {
        private Menu Config = Program.Config;
        public static SebbyLib.Orbwalking.Orbwalker Orbwalker = Program.Orbwalker;
        private AIHeroClient Player { get { return ObjectManager.Player; } }
        public Spell Q, W, E, R, DrawSpell;
        public static Font Tahoma13, Tahoma13B, TextBold;
        private float IntroTimer = Game.Time;
        private Render.Sprite Intro;

        public void LoadOKTW()
        {
            Config.SubMenu("About OKTW©").AddItem(new MenuItem("logo", "Intro logo OKTW").SetValue(true));

            if (Config.Item("logo").GetValue<bool>())
            {
                Intro = new Render.Sprite(LoadImg("intro"), new Vector2((Drawing.Width / 2) - 500, (Drawing.Height / 2) - 350));
                Intro.Add(0);
                Intro.OnDraw();
            }

            LeagueSharp.Common.Utility.DelayAction.Add(7000, () => Intro.Remove());

            Config.SubMenu("Utility, Draws OKTW©").AddItem(new MenuItem("disableDraws", "DISABLE UTILITY DRAWS").SetValue(false));

            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Enemy info grid").AddItem(new MenuItem("championInfo", "Game Info").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Enemy info grid").AddItem(new MenuItem("ShowKDA", "Show flash and R CD").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Enemy info grid").AddItem(new MenuItem("ShowRecall", "Show recall").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Enemy info grid").AddItem(new MenuItem("posX", "posX").SetValue(new Slider(20, 100, 0)));
            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Enemy info grid").AddItem(new MenuItem("posY", "posY").SetValue(new Slider(10, 100, 0)));

            Config.SubMenu("Utility, Draws OKTW©").AddItem(new MenuItem("GankAlert", "Gank Alert").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").AddItem(new MenuItem("HpBar", "Dmg indicators BAR OKTW© style").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").AddItem(new MenuItem("ShowClicks", "Show enemy clicks").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").AddItem(new MenuItem("SS", "SS notification").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").AddItem(new MenuItem("RF", "R and Flash notification").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").AddItem(new MenuItem("showWards", "Show hidden objects, wards").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").AddItem(new MenuItem("minimap", "Mini-map hack").SetValue(true));

            Tahoma13B = new Font( Drawing.Direct3DDevice, new FontDescription
               { FaceName = "Tahoma", Height = 14, Weight = FontWeight.Bold, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType });

            Tahoma13 = new Font( Drawing.Direct3DDevice, new FontDescription
                { FaceName = "Tahoma", Height = 14, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType });

            TextBold = new Font(Drawing.Direct3DDevice, new FontDescription
                {FaceName = "Impact", Height = 30, Weight = FontWeight.Normal, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType});

            Q = new Spell(SpellSlot.Q);
            E = new Spell(SpellSlot.E);
            W = new Spell(SpellSlot.W);
            R = new Spell(SpellSlot.R);

            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private static System.Drawing.Bitmap LoadImg(string imgName)
        {
            var bitmap = Resource1.ResourceManager.GetObject(imgName) as System.Drawing.Bitmap;
            if (bitmap == null)
            {
                Console.WriteLine(imgName + ".png not found.");
            }
            return bitmap;
        }

        public static void drawText(string msg, Vector3 Hero, System.Drawing.Color color, int weight = 0)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1] + weight, color, msg);
        }

        public static void DrawFontTextScreen(Font vFont, string vText, float vPosX, float vPosY, ColorBGRA vColor)
        {
            vFont.DrawText(null, vText, (int)vPosX, (int)vPosY, vColor);
        }

        public static void DrawFontTextMap(Font vFont, string vText, Vector3 Pos, ColorBGRA vColor)
        {
            var wts = Drawing.WorldToScreen(Pos);
            vFont.DrawText(null, vText, (int)wts[0] , (int)wts[1], vColor);
        }

        public static void drawLine(Vector3 pos1, Vector3 pos2, int bold, System.Drawing.Color color)
        {
            var wts1 = Drawing.WorldToScreen(pos1);
            var wts2 = Drawing.WorldToScreen(pos2);

            Drawing.DrawLine(wts1[0], wts1[1], wts2[0], wts2[1], bold, color);
        }

        private void Drawing_OnEndScene(EventArgs args)
        {
            if (Config.Item("minimap").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies)
                {
                    if (!enemy.IsVisible)
                    {
                        var ChampionInfoOne = Core.OKTWtracker.ChampionInfoList.Find(x => x.NetworkId == enemy.NetworkId);
                        if (ChampionInfoOne != null)
                        {
                            var wts = Drawing.WorldToMinimap(ChampionInfoOne.LastVisablePos);
                            DrawFontTextScreen(Tahoma13, enemy.ChampionName[0].ToString() + enemy.ChampionName[1].ToString(), wts[0], wts[1], Color.Yellow);
                        }
                    }
                }
            }
            if (Config.Item("showWards").GetValue<bool>())
            {
                foreach (var obj in OKTWward.HiddenObjList)
                {
                    if (obj.type == 1)
                    {
                        LeagueSharp.Common.Utility.DrawCircle(obj.pos, 100, System.Drawing.Color.Yellow, 3, 20,true);
                    }

                    if (obj.type == 2)
                    {
                        LeagueSharp.Common.Utility.DrawCircle(obj.pos, 100, System.Drawing.Color.HotPink, 3, 20,true);
                    }

                    if (obj.type == 3)
                    {
                        LeagueSharp.Common.Utility.DrawCircle(obj.pos, 100, System.Drawing.Color.Orange, 3, 20,true);
                    }
                }
            }   
        }

        private void OnDraw(EventArgs args)
        {
            if (Config.Item("disableDraws").GetValue<bool>())
                return;

            if (Config.Item("showWards").GetValue<bool>())
            {
                var circleSize = 30;
                foreach (var obj in OKTWward.HiddenObjList.Where(obj =>Render.OnScreen(Drawing.WorldToScreen(obj.pos))))
                {
                    if (obj.type == 1)
                    {
                        OktwCommon.DrawTriangleOKTW(circleSize, obj.pos, System.Drawing.Color.Yellow);
                        DrawFontTextMap(Tahoma13, "" + (int)(obj.endTime - Game.Time), obj.pos, SharpDX.Color.Yellow);
                    }

                    if (obj.type == 2)
                    {
                        OktwCommon.DrawTriangleOKTW(circleSize, obj.pos, System.Drawing.Color.HotPink);
                        DrawFontTextMap(Tahoma13, "VW", obj.pos, SharpDX.Color.HotPink);
                    }
                    if (obj.type == 3)
                    {
                        OktwCommon.DrawTriangleOKTW(circleSize, obj.pos, System.Drawing.Color.Orange);
                        DrawFontTextMap(Tahoma13, "! " + (int)(obj.endTime - Game.Time), obj.pos, SharpDX.Color.Orange);
                    }
                }
            }

            bool blink = true;

            if ((int)(Game.Time * 10) % 2 == 0)
                blink = false;

            var HpBar = Config.Item("HpBar").GetValue<bool>();
            var championInfo = Config.Item("championInfo").GetValue<bool>();
            var GankAlert = Config.Item("GankAlert").GetValue<bool>();
            var ShowKDA = Config.Item("ShowKDA").GetValue<bool>();
            var ShowRecall = Config.Item("ShowRecall").GetValue<bool>();
            var ShowClicks = Config.Item("ShowClicks").GetValue<bool>();
            float posY = ((float)Config.Item("posY").GetValue<Slider>().Value * 0.01f) * Drawing.Height;
            float posX = ((float)Config.Item("posX").GetValue<Slider>().Value * 0.01f) * Drawing.Width;
            float positionDraw = 0;
            float positionGang = 500;
            int Width = 103;
            int Height = 8;
            int XOffset = 10;
            int YOffset = 20;
            var FillColor = System.Drawing.Color.GreenYellow;
            var Color = System.Drawing.Color.Azure;
            float offset = 0;
                
            foreach (var enemy in HeroManager.Enemies)
            {
                offset += 0.15f;

                if (Config.Item("SS").GetValue<bool>())
                {
                    if (!enemy.IsVisible && !enemy.IsDead)
                    {
                        var ChampionInfoOne = OKTWtracker.ChampionInfoList.Find(x => x.NetworkId == enemy.NetworkId);
                        if (ChampionInfoOne != null && enemy != Program.jungler)
                        {
                            if ((int)(Game.Time * 10) % 2 == 0 && Game.Time - ChampionInfoOne.LastVisableTime > 3 && Game.Time - ChampionInfoOne.LastVisableTime < 7)
                            {
                                DrawFontTextScreen(TextBold, "SS " + enemy.ChampionName + " " + (int)(Game.Time - ChampionInfoOne.LastVisableTime), Drawing.Width * offset, Drawing.Height * 0.02f, SharpDX.Color.Orange);
                            }
                            if (Game.Time - ChampionInfoOne.LastVisableTime >= 7)
                            {
                                DrawFontTextScreen(TextBold, "SS " + enemy.ChampionName + " " + (int)(Game.Time - ChampionInfoOne.LastVisableTime), Drawing.Width * offset, Drawing.Height * 0.02f, SharpDX.Color.Orange);
                            }
                        }
                    }
                }
                
                if (enemy.IsValidTarget() && ShowClicks)
                {
                    var lastWaypoint = enemy.GetWaypoints().Last().To3D();
                    if (lastWaypoint.IsValid())
                    {
                        drawLine(enemy.Position, lastWaypoint, 1, System.Drawing.Color.Red);
                            
                        if (enemy.GetWaypoints().Count() > 1)
                            DrawFontTextMap(Tahoma13, enemy.ChampionName, lastWaypoint, SharpDX.Color.WhiteSmoke);
                    }
                }

                if (HpBar && enemy.IsHPBarRendered && Render.OnScreen(Drawing.WorldToScreen(enemy.Position)))
                {
                    var barPos = enemy.HPBarPosition;

                    float QdmgDraw = 0, WdmgDraw = 0, EdmgDraw = 0, RdmgDraw = 0, damage = 0; ;

                    if (Q.IsReady())
                        damage = damage + Q.GetDamage(enemy);

                    if (W.IsReady() && Player.ChampionName != "Kalista")
                        damage = damage + W.GetDamage(enemy);

                    if (E.IsReady())
                        damage = damage + E.GetDamage(enemy);

                    if (R.IsReady())
                        damage = damage + R.GetDamage(enemy);

                    if (Q.IsReady())
                        QdmgDraw = (Q.GetDamage(enemy) / damage);

                    if (W.IsReady() && Player.ChampionName != "Kalista")
                        WdmgDraw = (W.GetDamage(enemy) / damage);

                    if (E.IsReady())
                        EdmgDraw = (E.GetDamage(enemy) / damage);

                    if (R.IsReady())
                        RdmgDraw = (R.GetDamage(enemy) / damage);

                    var percentHealthAfterDamage = Math.Max(0, enemy.Health - damage) / enemy.MaxHealth;

                    var yPos = barPos.Y + YOffset;
                    var xPosDamage = barPos.X + XOffset + Width * percentHealthAfterDamage;
                    var xPosCurrentHp = barPos.X + XOffset + Width * enemy.Health / enemy.MaxHealth;

                    float differenceInHP = xPosCurrentHp - xPosDamage;
                    var pos1 = barPos.X + XOffset + (107 * percentHealthAfterDamage);

                    for (int i = 0; i < differenceInHP; i++)
                    {
                        if (Q.IsReady() && i < QdmgDraw * differenceInHP)
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, System.Drawing.Color.Cyan);
                        else if (W.IsReady() && i < (QdmgDraw + WdmgDraw) * differenceInHP)
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, System.Drawing.Color.Orange);
                        else if (E.IsReady() && i < (QdmgDraw + WdmgDraw + EdmgDraw) * differenceInHP)
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, System.Drawing.Color.Yellow);
                        else if (R.IsReady() && i < (QdmgDraw + WdmgDraw + EdmgDraw + RdmgDraw) * differenceInHP)
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, System.Drawing.Color.YellowGreen);
                    }
                }

                var kolor = System.Drawing.Color.GreenYellow;

                if (enemy.IsDead)
                    kolor = System.Drawing.Color.Gray;
                else if (!enemy.IsVisible)
                    kolor = System.Drawing.Color.OrangeRed;

                var kolorHP = System.Drawing.Color.GreenYellow;

                if (enemy.IsDead)
                    kolorHP = System.Drawing.Color.GreenYellow;
                else if ((int)enemy.HealthPercent < 30)
                    kolorHP = System.Drawing.Color.Red;
                else if ((int)enemy.HealthPercent < 60)
                    kolorHP = System.Drawing.Color.Orange;

                if (championInfo)
                {
                    positionDraw += 15;
                    DrawFontTextScreen(Tahoma13, "" + enemy.Level, posX - 25, posY + positionDraw, SharpDX.Color.White);
                    DrawFontTextScreen(Tahoma13, enemy.ChampionName, posX, posY + positionDraw, SharpDX.Color.White);

                    if (true)
                    {
                        var ChampionInfoOne = Core.OKTWtracker.ChampionInfoList.Find(x => x.NetworkId == enemy.NetworkId);
                        if (Game.Time - ChampionInfoOne.FinishRecallTime < 4 )
                        {
                            DrawFontTextScreen(Tahoma13, "FINISH" , posX - 90, posY + positionDraw, SharpDX.Color.GreenYellow);
                        }
                        else if (ChampionInfoOne.StartRecallTime <= ChampionInfoOne.AbortRecallTime && Game.Time - ChampionInfoOne.AbortRecallTime < 4)
                        {
                            DrawFontTextScreen(Tahoma13, "ABORT", posX - 90, posY + positionDraw, SharpDX.Color.Yellow);
                        }
                        else if (Game.Time - ChampionInfoOne.StartRecallTime < 8)
                        {

                                int recallPercent = (int)(((Game.Time - ChampionInfoOne.StartRecallTime) / 8) * 100);
                                float recallX1 = posX - 90;
                                float recallY1 = posY + positionDraw + 3;
                                float recallX2 = (recallX1 + ((int)recallPercent / 2)) + 1;
                                float recallY2 = posY + positionDraw + 3;
                                Drawing.DrawLine(recallX1, recallY1, recallX1 + 50, recallY2, 8, System.Drawing.Color.Red);
                                Drawing.DrawLine(recallX1, recallY1, recallX2, recallY2, 8, System.Drawing.Color.White);
                        }
                    }

                        var fSlot = enemy.Spellbook.Spells[4];
   
                        if (fSlot.Name != "SummonerFlash")
                            fSlot = enemy.Spellbook.Spells[5];

                        if (fSlot.Name == "SummonerFlash")
                        {
                            var fT = fSlot.CooldownExpires - Game.Time;
                            if (ShowKDA)
                            {
                                if (fT < 0)
                                    DrawFontTextScreen(Tahoma13, "F rdy", posX + 110, posY + positionDraw, SharpDX.Color.GreenYellow);
                                else
                                    DrawFontTextScreen(Tahoma13, "F " + (int)fT, posX + 110, posY + positionDraw, SharpDX.Color.Yellow);
                            }
                            if (Config.Item("RF").GetValue<bool>())
                            {
                                if (fT < 2 && fT > -3)
                                {
                                    DrawFontTextScreen(TextBold, enemy.ChampionName + " FLASH READY!", Drawing.Width * offset, Drawing.Height * 0.1f, SharpDX.Color.Yellow);
                                }
                                else if (fSlot.Cooldown - fT < 5)
                                {
                                    DrawFontTextScreen(TextBold, enemy.ChampionName + " FLASH LOST!", Drawing.Width * offset, Drawing.Height * 0.1f, SharpDX.Color.Red);
                                }
                            }
                        }

                        if (enemy.Level > 5)
                        {
                            var rSlot = enemy.Spellbook.Spells[3];
                            var t = rSlot.CooldownExpires - Game.Time;
                            if (ShowKDA)
                            {
                                if (t < 0)
                                    DrawFontTextScreen(Tahoma13, "R rdy", posX + 145, posY + positionDraw, SharpDX.Color.GreenYellow);
                                else
                                    DrawFontTextScreen(Tahoma13, "R " + (int)t, posX + 145, posY + positionDraw, SharpDX.Color.Yellow);
                            }
                            if (Config.Item("RF").GetValue<bool>())
                            {
                                if (t < 2 && t > -3)
                                {
                                    DrawFontTextScreen(TextBold, enemy.ChampionName + " R READY!", Drawing.Width * offset, Drawing.Height * 0.2f, SharpDX.Color.YellowGreen);
                                }
                                else if (rSlot.Cooldown - t < 5)
                                {
                                    DrawFontTextScreen(TextBold, enemy.ChampionName + " R LOST!", Drawing.Width * offset, Drawing.Height * 0.1f, SharpDX.Color.Red);
                                }
                            }
                        }
                        else if (ShowKDA)
                            DrawFontTextScreen(Tahoma13, "R ", posX + 145, posY + positionDraw, SharpDX.Color.Yellow);  
                    //Drawing.DrawText(posX - 70, posY + positionDraw, kolor, enemy.Level + " lvl");
                }

                var Distance = Player.Distance(enemy.Position);
                if (GankAlert && !enemy.IsDead && Distance > 1200)
                {
                    var wts = Drawing.WorldToScreen(ObjectManager.Player.Position.Extend(enemy.Position, positionGang));

                    wts[0] = wts[0];
                    wts[1] = wts[1] + 15;

                    if ((int)enemy.HealthPercent > 0)
                        Drawing.DrawLine(wts[0], wts[1], (wts[0] + ((int)enemy.HealthPercent) / 2) + 1, wts[1], 8, kolorHP);

                    if ((int)enemy.HealthPercent < 100)
                        Drawing.DrawLine((wts[0] + ((int)enemy.HealthPercent) / 2), wts[1], wts[0] + 50, wts[1], 8, System.Drawing.Color.White);

                    if (enemy.IsVisible)
                    {
                        
                        if (Program.jungler.NetworkId == enemy.NetworkId)
                        {
                            DrawFontTextMap(Tahoma13B, enemy.ChampionName, Player.Position.Extend(enemy.Position, positionGang), SharpDX.Color.OrangeRed);
                        }
                        else
                            DrawFontTextMap(Tahoma13, enemy.ChampionName, Player.Position.Extend(enemy.Position, positionGang), SharpDX.Color.White);

                    }
                    else 
                    {
                        var ChampionInfoOne = Core.OKTWtracker.ChampionInfoList.Find(x => x.NetworkId == enemy.NetworkId);
                        if (ChampionInfoOne != null )
                        {
                            if (Game.Time - ChampionInfoOne.LastVisableTime > 3 && Game.Time - ChampionInfoOne.LastVisableTime < 7)
                            {
                                DrawFontTextMap(Tahoma13, "SS " + enemy.ChampionName + " " + (int)(Game.Time - ChampionInfoOne.LastVisableTime), Player.Position.Extend(enemy.Position, positionGang), SharpDX.Color.Yellow);
                            }
                            else
                            {
                                DrawFontTextMap(Tahoma13, "SS " + enemy.ChampionName + " " + (int)(Game.Time - ChampionInfoOne.LastVisableTime), Player.Position.Extend(enemy.Position, positionGang), SharpDX.Color.Yellow);
                            }
                        }
                        else
                            DrawFontTextMap(Tahoma13, "SS " + enemy.ChampionName, Player.Position.Extend(enemy.Position, positionGang), SharpDX.Color.Yellow);
                    }
  

                    if (Distance < 3500 && enemy.IsVisible && !Render.OnScreen(Drawing.WorldToScreen(enemy.Position)) && Program.jungler != null)
                    {
                        if (Program.jungler.NetworkId == enemy.NetworkId)
                        {
                            drawLine(Player.Position.Extend(enemy.Position, 100), Player.Position.Extend(enemy.Position, positionGang - 100), (int)((3500 - Distance) / 280), System.Drawing.Color.Crimson);
                        }
                        else
                        {
                            if(enemy.IsFacing(Player))
                                drawLine(Player.Position.Extend(enemy.Position, 100), Player.Position.Extend(enemy.Position, positionGang - 100), (int)((3500 - Distance) / 280), System.Drawing.Color.Orange);
                            else
                                drawLine(Player.Position.Extend(enemy.Position, 100), Player.Position.Extend(enemy.Position, positionGang - 100), (int)((3500 - Distance) / 280), System.Drawing.Color.Gold);
                        }
                    }
                    else if (Distance < 3500 && !enemy.IsVisible && !Render.OnScreen(Drawing.WorldToScreen(Player.Position.Extend(enemy.Position, Distance + 500))))
                    {
                        var need = Core.OKTWtracker.ChampionInfoList.Find(x => x.NetworkId == enemy.NetworkId);
                        if (need != null && Game.Time - need.LastVisableTime < 5)
                        {
                            
                            drawLine(Player.Position.Extend(enemy.Position, 100), Player.Position.Extend(enemy.Position, positionGang - 100), (int)((3500 - Distance) / 300), System.Drawing.Color.Gray);
                        }
                    }
                }
                positionGang = positionGang + 100;
            }

            if (Program.AIOmode == 2)
            {
                Drawing.DrawText(Drawing.Width * 0.2f, Drawing.Height * 1f, System.Drawing.Color.Cyan, "OKTW AIO only utility mode ON");
            }
        }
    }
}
