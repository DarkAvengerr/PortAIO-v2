using LeagueSharp;
using System;
using PrideStalker_Rengar.Main;
using LeagueSharp.SDK.Utils;
using Nechrito_Rengar;
using LeagueSharp.SDK.Enumerations;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace PrideStalker_Rengar.Draw
{
    internal class DRAW : Core
    {
        public static HpBarDraw DrawHpBar = new HpBarDraw();
        public static void DrawAnimation()
        {
            var heropos = Drawing.WorldToScreen(ObjectManager.Player.Position);
            var textPos = Drawing.WorldToScreen(ObjectManager.Player.Position + 175);

            if (Orbwalker.ActiveMode == OrbwalkingMode.None)
            {
                if (MenuConfig.DrawCombo && !MenuConfig.DrawAnim)
                {
                    if (MenuConfig.ComboMode.SelectedValue == "Ap Combo")
                    {
                        if (MenuConfig.DrawHelp)
                        {
                            Drawing.DrawText(textPos.X - 15, textPos.Y + 40, System.Drawing.Color.White, "[Recommended Stacks = 3/5]");
                        }
                        Drawing.DrawText(heropos.X - 15, heropos.Y + 40, System.Drawing.Color.White, "AP COMBO");
                    }
                    if (MenuConfig.ComboMode.SelectedValue == "Triple Q")
                    {
                        if (MenuConfig.DrawHelp)
                        {
                            Drawing.DrawText(textPos.X - 15, textPos.Y + 40, System.Drawing.Color.White, "[Recommended Stacks = 4/5]");
                            Drawing.DrawText(textPos.X - 15, textPos.Y + 60, System.Drawing.Color.White, "Will Try Q 3 Times");
                            Drawing.DrawText(textPos.X - 15, textPos.Y + 80, System.Drawing.Color.White, "Jungle: Warrior => Lucidity => PD => Inf => Sell boots for Trinity");
                        }
                        Drawing.DrawText(heropos.X - 15, heropos.Y + 40, System.Drawing.Color.White, "TRIPLE Q");
                    }
                    if (MenuConfig.ComboMode.SelectedValue == "Gank")
                    {
                        if (MenuConfig.DrawHelp)
                        {
                            Drawing.DrawText(textPos.X - 15, textPos.Y + 40, System.Drawing.Color.White, "[Recommended Stacks = 5/5]");
                            Drawing.DrawText(textPos.X - 15, textPos.Y + 60, System.Drawing.Color.White, "Will Try To Stun Target");
                        }
                        Drawing.DrawText(heropos.X - 15, heropos.Y + 40, System.Drawing.Color.White, "GANK");
                    }
                    if (MenuConfig.ComboMode.SelectedValue == "OneShot")
                    {
                        if (MenuConfig.DrawHelp)
                        {
                            Drawing.DrawText(textPos.X - 15, textPos.Y + 40, System.Drawing.Color.White, "[Recommended Stacks = 5/5]");
                            Drawing.DrawText(textPos.X - 15, textPos.Y + 60, System.Drawing.Color.White, "50%~+ Crit Recommended");
                        }
                        Drawing.DrawText(heropos.X - 15, heropos.Y + 40, System.Drawing.Color.White, "ONESHOT");
                    }
                }
                else if (MenuConfig.DrawCombo && MenuConfig.DrawAnim)
                {
                    if (MenuConfig.ComboMode.SelectedValue == "Ap Combo")
                    {
                        if (MenuConfig.DrawHelp)
                        {
                            Drawing.DrawText(textPos.X - 15, textPos.Y + 40, System.Drawing.Color.White, "[Recommended Stacks = 3/5]");
                            Drawing.DrawText(textPos.X - 15, textPos.Y + 60, System.Drawing.Color.White, "Get ProtoBelt & Ludens");
                        }
                        Drawing.DrawText(heropos.X - 15, heropos.Y + 40, System.Drawing.Color.White, "[AP COMBO]");
                        Drawing.DrawText(heropos.X - 15, heropos.Y + 60, System.Drawing.Color.Gray, "2.ONESHOT");
                        Drawing.DrawText(heropos.X - 15, heropos.Y + 80, System.Drawing.Color.DimGray, "3.GANK");
                    }
                    if (MenuConfig.ComboMode.SelectedValue == "OneShot")
                    {
                        if (MenuConfig.DrawHelp)
                        {
                            Drawing.DrawText(textPos.X - 15, textPos.Y + 40, System.Drawing.Color.White, "[Recommended Stacks = 5/5]");
                            Drawing.DrawText(textPos.X - 15, textPos.Y + 60, System.Drawing.Color.White, "50%~+ Crit Recommended");
                            Drawing.DrawText(textPos.X - 15, textPos.Y + 80, System.Drawing.Color.White, "Jungle: Sabre => Youmuu => PD => Inf => Trinity");
                        }
                            Drawing.DrawText(heropos.X - 15, heropos.Y + 40, System.Drawing.Color.White, "[ONESHOT]");
                            Drawing.DrawText(heropos.X - 15, heropos.Y + 60, System.Drawing.Color.Gray, "2.GANK");
                            Drawing.DrawText(heropos.X - 15, heropos.Y + 80, System.Drawing.Color.DimGray, "3.TRIPLE Q");  
                    }
                    if (MenuConfig.ComboMode.SelectedValue == "Gank")
                    {
                        if (MenuConfig.DrawHelp)
                        {
                            Drawing.DrawText(textPos.X - 15, textPos.Y + 40, System.Drawing.Color.White, "[Recommended Stacks = 5/5]");
                            Drawing.DrawText(textPos.X - 15, textPos.Y + 60, System.Drawing.Color.White, "Will Try To Stun Target");
                        }
                        Drawing.DrawText(heropos.X - 15, heropos.Y + 40, System.Drawing.Color.White, "[GANK]");
                        Drawing.DrawText(heropos.X - 15, heropos.Y + 60, System.Drawing.Color.Gray, "2.TRIPLE Q");
                        Drawing.DrawText(heropos.X - 15, heropos.Y + 80, System.Drawing.Color.DimGray, "3.AP COMBO");
                    }
                    if (MenuConfig.ComboMode.SelectedValue == "Triple Q")
                    {
                        if (MenuConfig.DrawHelp)
                        {
                            Drawing.DrawText(textPos.X - 15, textPos.Y + 40, System.Drawing.Color.White, "[Recommended Stacks = 4/5]");
                            Drawing.DrawText(textPos.X - 15, textPos.Y + 60, System.Drawing.Color.White, "Will Try Q 3 Times");
                            Drawing.DrawText(textPos.X - 15, textPos.Y + 80, System.Drawing.Color.White, "Jungle: Warrior => Lucidity => PD => Inf => Sell boots for Trinity");
                        }
                        Drawing.DrawText(heropos.X - 15, heropos.Y + 40, System.Drawing.Color.White, "[TRIPLE Q]");
                        Drawing.DrawText(heropos.X - 15, heropos.Y + 60, System.Drawing.Color.Gray, "2.AP COMBO");
                        Drawing.DrawText(heropos.X - 15, heropos.Y + 80, System.Drawing.Color.DimGray, "3.ONESHOT");
                    }

                }
            }
        }
        public static void OnDraw(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }
            var heropos = Drawing.WorldToScreen(ObjectManager.Player.Position);

            DrawAnimation();

            if (MenuConfig.Passive.Active)
            {
                Drawing.DrawText(heropos.X - 15, heropos.Y + 20, System.Drawing.Color.Cyan, "Stacking  (     )");
                Drawing.DrawText(heropos.X + 62, heropos.Y + 20, MenuConfig.Passive.Active ? System.Drawing.Color.White : System.Drawing.Color.Red, MenuConfig.Passive.Active ? "On" : "Off");
            }
           
            if (MenuConfig.EngageDraw)
            {
                Render.Circle.DrawCircle(Player.Position, Spells.E.Range,
                   Spells.E.IsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed);
            }
        }
    }
}
