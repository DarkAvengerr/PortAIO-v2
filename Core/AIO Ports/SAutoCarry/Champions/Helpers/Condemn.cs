using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SCommon.PluginBase;
using SPrediction;
using SharpDX;
using EloBuddy;

namespace SAutoCarry.Champions.Helpers
{
    public static class Condemn
    {
        private static SCommon.PluginBase.Champion s_Champion;
        private static SpellSlot s_Flash;
        public static void Initialize(SCommon.PluginBase.Champion champ)
        {
            s_Champion = champ;
            s_Flash = ObjectManager.Player.GetSpellSlot("summonerflash");
            Menu condemn = new Menu("Condemn Settings", "SAutoCarry.Helpers.Condemn.Root");
            condemn.AddItem(new MenuItem("SAutoCarry.Helpers.Condemn.Root.AntiGapCloser", "Use Condemn to Gapclosers").SetValue(false));
            condemn.AddItem(new MenuItem("SAutoCarry.Helpers.Condemn.Root.Interrupter", "Use Condemn to Interrupt spells").SetValue(false));
            condemn.AddItem(new MenuItem("SAutoCarry.Helpers.Condemn.Root.Jungle", "Condemn Jungle Minions").SetValue(true));
            condemn.AddItem(new MenuItem("SAutoCarry.Helpers.Condemn.Root.TumbleCondemn", "Q->E when possible").SetTooltip("if this option enabled, assembly will try to do tumble and condemn (this option may drop fps while E is up)").SetValue(true)).ValueChanged += 
                (s, ar) =>
                {
                    condemn.Item("SAutoCarry.Helpers.Condemn.Root.TumbleCondemnSafe").Show(ar.GetNewValue<bool>());
                    condemn.Item("SAutoCarry.Helpers.Condemn.Root.TumbleCondemnCount").Show(ar.GetNewValue<bool>());
                };
            condemn.AddItem(new MenuItem("SAutoCarry.Helpers.Condemn.Root.TumbleCondemnCount", "Q->E Position Check Count").SetValue(new Slider(12, 2, 12)).SetTooltip("the bigger count is more fps drop")).Show(condemn.Item("SAutoCarry.Helpers.Condemn.Root.TumbleCondemn").GetValue<bool>());
            condemn.AddItem(new MenuItem("SAutoCarry.Helpers.Condemn.Root.TumbleCondemnSafe", "Only Q->E when tumble position is safe").SetValue(false)).Show(condemn.Item("SAutoCarry.Helpers.Condemn.Root.TumbleCondemn").GetValue<bool>());
            condemn.AddItem(new MenuItem("SAutoCarry.Helpers.Condemn.Root.FlashCondemn", "Condemn->Flash selected target").SetValue(new KeyBind('T', KeyBindType.Press)));
            condemn.AddItem(new MenuItem("SAutoCarry.Helpers.Condemn.Root.DontCondemnTurret", "Dont Condemn Under Turret").SetTooltip("if this option enabled, enemy wont condemned under his allied tower").SetValue(true));
            condemn.AddItem(new MenuItem("SAutoCarry.Helpers.Condemn.Root.PushDistance", "Push Distance").SetValue(new Slider(400, 300, 470)));
            condemn.AddItem(new MenuItem("SAutoCarry.Helpers.Condemn.Root.Accuracy", "Accuracy").SetValue(new Slider(12, 2, 12))).SetTooltip("the bigger count is more fps drop");
            condemn.AddItem(new MenuItem("SAutoCarry.Helpers.Condemn.Root.Draw", "Draw").SetValue(true));
            Menu whitelist = new Menu("Whitelist", "SAutoCarry.Helpers.Condemn.WhiteList");
            foreach(var enemy in HeroManager.Enemies)
                whitelist.AddItem(new MenuItem("SAutoCarry.Helpers.Condemn.WhiteList" + enemy.ChampionName, "Condemn " + enemy.ChampionName).SetValue(true));
            condemn.AddSubMenu(whitelist);
            s_Champion.ConfigMenu.AddSubMenu(condemn);
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (s_Champion.ConfigMenu.Item("SAutoCarry.Helpers.Condemn.Root.Draw").GetValue<bool>() && s_Champion.Spells[2].IsReady())
            {
                foreach (var enemy in HeroManager.Enemies)
                {
                    if (enemy.IsValidTarget(2000))
                    {
                        var targetPosition = SPrediction.Geometry.PositionAfter(enemy.Path.ToList().To2D(), 300, (int)enemy.MoveSpeed);
                        var pushDirection = (targetPosition - ObjectManager.Player.ServerPosition.To2D()).Normalized();
                        for (int i = 0; i < PushDistance - 20; i += 20)
                        {
                            var lastPost = targetPosition + (pushDirection * i);
                            var colFlags = NavMesh.GetCollisionFlags(lastPost.X, lastPost.Y);
                            if (colFlags.HasFlag(CollisionFlags.Wall) || colFlags.HasFlag(CollisionFlags.Building))
                            {
                                var sideA = lastPost + pushDirection * 20f + (pushDirection.Perpendicular() * enemy.BoundingRadius);
                                var sideB = lastPost + pushDirection * 20f - (pushDirection.Perpendicular() * enemy.BoundingRadius);

                                var flagsA = NavMesh.GetCollisionFlags(sideA.X, sideA.Y);
                                var flagsB = NavMesh.GetCollisionFlags(sideB.X, sideB.Y);

                                bool condemn = (flagsA.HasFlag(CollisionFlags.Wall) || flagsA.HasFlag(CollisionFlags.Building)) && (flagsB.HasFlag(CollisionFlags.Wall) || flagsB.HasFlag(CollisionFlags.Building));
                                if (condemn)
                                {
                                    Drawing.DrawLine(Drawing.WorldToScreen(targetPosition.To3D()), Drawing.WorldToScreen((targetPosition + pushDirection * PushDistance + (pushDirection.Perpendicular() * enemy.BoundingRadius)).To3D()), 3, System.Drawing.Color.Green);
                                    Drawing.DrawLine(Drawing.WorldToScreen(targetPosition.To3D()), Drawing.WorldToScreen((targetPosition + pushDirection * PushDistance - (pushDirection.Perpendicular() * enemy.BoundingRadius)).To3D()), 3, System.Drawing.Color.Green);
                                    Render.Circle.DrawCircle(sideA.To3D(), 10, System.Drawing.Color.Green, 7);
                                    Render.Circle.DrawCircle(sideB.To3D(), 10, System.Drawing.Color.Green, 7);
                                    return;
                                }
                            }
                        }
                        var lastPos = targetPosition + pushDirection * PushDistance - 20;
                        var sideAa = lastPos + pushDirection * 20f + pushDirection.Perpendicular() * enemy.BoundingRadius;
                        var sideBb = lastPos + pushDirection * 20f - pushDirection.Perpendicular() * enemy.BoundingRadius;
                        Drawing.DrawLine(Drawing.WorldToScreen(targetPosition.To3D()), Drawing.WorldToScreen(sideAa.To3D()), 3, System.Drawing.Color.Red);
                        Drawing.DrawLine(Drawing.WorldToScreen(targetPosition.To3D()), Drawing.WorldToScreen(sideBb.To3D()), 3, System.Drawing.Color.Red);
                        Render.Circle.DrawCircle(sideAa.To3D(), 10, System.Drawing.Color.Red, 7);
                        Render.Circle.DrawCircle(sideBb.To3D(), 10, System.Drawing.Color.Red, 7);
                    }
                }
            }
        }

        public static bool IsValidTarget(AIHeroClient target)
        {
            if (!IsWhitelisted(target))
                return false;

            var targetPosition = SPrediction.Geometry.PositionAfter(target.Path.ToList().To2D(), 300, (int)target.MoveSpeed);

            if (target.Distance(ObjectManager.Player.ServerPosition) < 650f && IsCondemnable(ObjectManager.Player.ServerPosition.To2D(), targetPosition, target.BoundingRadius))
            {
                if (target.Path.Length == 0)
                {
                    var outRadius = (0.3f * target.MoveSpeed) / (float)Math.Cos(2 * Math.PI / Accuracy);
                    int count = 0;
                    for (int i = 1; i <= Accuracy; i++)
                    {
                        if (count + (Accuracy - i) < Accuracy / 3)
                            return false;

                        var angle = i * 2 * Math.PI / Accuracy;
                        float x = target.Position.X + outRadius * (float)Math.Cos(angle);
                        float y = target.Position.Y + outRadius * (float)Math.Sin(angle);
                        if (IsCondemnable(ObjectManager.Player.ServerPosition.To2D(), new Vector2(x, y), target.BoundingRadius))
                            count++;
                    }
                    return count >= Accuracy / 3;
                }
                else
                    return true;
            }
            else
            {
                if (TumbleCondemn && s_Champion.Spells[SCommon.PluginBase.Champion.Q].IsReady())
                {
                    var outRadius = 300 / (float)Math.Cos(2 * Math.PI / TumbleCondemnCount);

                    for (int i = 1; i <= TumbleCondemnCount; i++)
                    {
                        var angle = i * 2 * Math.PI / TumbleCondemnCount;
                        float x = ObjectManager.Player.Position.X + outRadius * (float)Math.Cos(angle);
                        float y = ObjectManager.Player.Position.Y + outRadius * (float)Math.Sin(angle);
                        targetPosition = SPrediction.Geometry.PositionAfter(target.Path.ToList().To2D(), 300, (int)target.MoveSpeed);
                        var vec = new Vector2(x, y);
                        if (targetPosition.Distance(vec) < 550f && IsCondemnable(vec, targetPosition, target.BoundingRadius, 300f))
                        {
                            if (!TumbleCondemnSafe || Tumble.IsSafe(target, vec.To3D2(), false).IsValid())
                            {
                                s_Champion.Spells[SCommon.PluginBase.Champion.Q].Cast(vec);
                                break;
                            }
                        }
                    }

                    return false;
                }
            }

            return false;
        }

        private static bool IsCondemnable(Vector2 from, Vector2 targetPosition, float boundingRadius, float pushRange = -1)
        {
            if (pushRange == -1)
                pushRange = PushDistance - 20f;

            var pushDirection = (targetPosition - from).Normalized();
            for (int i = 0; i < pushRange; i += 20)
            {
                var lastPost = targetPosition + (pushDirection * i);
                if (!lastPost.To3D2().UnderTurret(true) || !DontCondemnTurret)
                {
                    var colFlags = NavMesh.GetCollisionFlags(lastPost.X, lastPost.Y);
                    if (colFlags.HasFlag(CollisionFlags.Wall) || colFlags.HasFlag(CollisionFlags.Building))
                    {
                        var sideA = lastPost + pushDirection * 20f + (pushDirection.Perpendicular() * boundingRadius);
                        var sideB = lastPost + pushDirection * 20f - (pushDirection.Perpendicular() * boundingRadius);

                        var flagsA = NavMesh.GetCollisionFlags(sideA.X, sideA.Y);
                        var flagsB = NavMesh.GetCollisionFlags(sideB.X, sideB.Y);

                        if ((flagsA.HasFlag(CollisionFlags.Wall) || flagsA.HasFlag(CollisionFlags.Building)) && (flagsB.HasFlag(CollisionFlags.Wall) || flagsB.HasFlag(CollisionFlags.Building)))
                            return true;
                    }
                }
            }
            return false;
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (AntiGapCloser && gapcloser.End.Distance(ObjectManager.Player.ServerPosition) < 200)
                s_Champion.Spells[SCommon.PluginBase.Champion.E].CastOnUnit(gapcloser.Sender);
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Interrupter && sender.IsValidTarget(s_Champion.Spells[SCommon.PluginBase.Champion.E].Range))
                s_Champion.Spells[SCommon.PluginBase.Champion.E].CastOnUnit(sender);
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (args == null || ObjectManager.Player.IsDead)
                return;

            if(FlashCondemn)
            {
                //asuna's condemn flash
                if(TargetSelector.SelectedTarget != null && TargetSelector.SelectedTarget.IsValidTarget(s_Champion.Spells[SCommon.PluginBase.Champion.E].Range))
                {
                    const int currentStep = 30;
                    var direction = ObjectManager.Player.Direction.To2D().Perpendicular();

                    for (var i = -90; i <= 90; i += currentStep)
                    {
                        var angleRad = LeagueSharp.Common.Geometry.DegreeToRadian(i);
                        var rotatedPosition = ObjectManager.Player.Position.To2D() + (425f * direction.Rotated(angleRad));
                        if(IsCondemnable(rotatedPosition, SPrediction.Geometry.PositionAfter(TargetSelector.SelectedTarget.Path.ToList().To2D(), (int)300, (int)TargetSelector.SelectedTarget.MoveSpeed), TargetSelector.SelectedTarget.BoundingRadius))
                        {
                            s_Champion.Spells[SCommon.PluginBase.Champion.E].CastOnUnit(TargetSelector.SelectedTarget);
                            LeagueSharp.Common.Utility.DelayAction.Add((int)(250 + Game.Ping / 2f + 25), () =>
                            {
                                ObjectManager.Player.Spellbook.CastSpell(s_Flash, rotatedPosition.To3D());
                            });

                        }
                    }

                }
            }
        }

        public static bool IsWhitelisted(AIHeroClient enemy)
        {
            return s_Champion.ConfigMenu.Item("SAutoCarry.Helpers.Condemn.WhiteList" + enemy.ChampionName).GetValue<bool>();
        }

        public static bool AntiGapCloser
        {
            get { return s_Champion.ConfigMenu.Item("SAutoCarry.Helpers.Condemn.Root.AntiGapCloser").GetValue<bool>(); }
        }

        public static bool Interrupter
        {
            get { return s_Champion.ConfigMenu.Item("SAutoCarry.Helpers.Condemn.Root.Interrupter").GetValue<bool>(); }
        }

        public static bool CondemnJungleMinions
        {
            get { return s_Champion.ConfigMenu.Item("SAutoCarry.Helpers.Condemn.Root.Jungle").GetValue<bool>(); }
        }

        public static bool TumbleCondemn
        {
            get { return s_Champion.ConfigMenu.Item("SAutoCarry.Helpers.Condemn.Root.TumbleCondemn").GetValue<bool>(); }
        }

        public static bool TumbleCondemnSafe
        {
            get { return s_Champion.ConfigMenu.Item("SAutoCarry.Helpers.Condemn.Root.TumbleCondemnSafe").GetValue<bool>(); }
        }

        public static int TumbleCondemnCount
        {
            get { return s_Champion.ConfigMenu.Item("SAutoCarry.Helpers.Condemn.Root.TumbleCondemnCount").GetValue<Slider>().Value; }
        }

        public static bool FlashCondemn
        {
            get { return s_Champion.ConfigMenu.Item("SAutoCarry.Helpers.Condemn.Root.FlashCondemn").GetValue<KeyBind>().Active; }
        }

        public static bool DontCondemnTurret
        {
            get { return s_Champion.ConfigMenu.Item("SAutoCarry.Helpers.Condemn.Root.DontCondemnTurret").GetValue<bool>(); }
        }

        public static int PushDistance
        {
            get { return s_Champion.ConfigMenu.Item("SAutoCarry.Helpers.Condemn.Root.PushDistance").GetValue<Slider>().Value; }
        }

        public static int Accuracy
        {
            get { return s_Champion.ConfigMenu.Item("SAutoCarry.Helpers.Condemn.Root.Accuracy").GetValue<Slider>().Value; }
        }
    }
}
