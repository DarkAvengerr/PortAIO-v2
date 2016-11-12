using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using HikiCarry.Core.Plugins;
using HikiCarry.Core.Predictions;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Utilities = HikiCarry.Core.Utilities.Utilities;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry.Champions
{
    internal class Vayne
    {
        internal static Spell Q;
        internal static Spell W;
        internal static Spell E;
        internal static Spell R;
        public static long LastCheck;
        public static List<Vector2> Points = new List<Vector2>();

        public Vayne()
        {

            Q = new Spell(SpellSlot.Q, 300f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 550f);
            E.SetTargetted(0.25f, 1600f);
            R = new Spell(SpellSlot.R);

            var comboMenu = new Menu("Combo Settings", "Combo Settings");
            {
                comboMenu.AddItem(new MenuItem("q.combo", "Use (Q)", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("e.combo", "Use (E)", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("r.combo", "Use (R)", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("combo.r.count", "R on x Enemy", true).SetValue(new Slider(4, 1, 5)));
                Initializer.Config.AddSubMenu(comboMenu);
            }
            
            var harassMenu = new Menu("Harass Settings", "Harass Settings");
            {
                harassMenu.AddItem(new MenuItem("q.harass", "Use (Q)", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("e.harass", "Use (E)", true).SetValue(false));
                harassMenu.AddItem(new MenuItem("harass.mana", "Harass Mana Percent", true).SetValue(new Slider(30, 1, 99)));
                Initializer.Config.AddSubMenu(harassMenu);
            }

            var junglemenu = new Menu("Jungle Settings", "Jungle Settings");
            {
                junglemenu.AddItem(new MenuItem("q.jungle", "Use (Q)", true).SetValue(true));
                junglemenu.AddItem(new MenuItem("e.jungle", "Use (Q)", true).SetValue(true));
                junglemenu.AddItem(new MenuItem("jungle.mana", "Jungle Mana Percent", true).SetValue(new Slider(30, 1, 99)));
                Initializer.Config.AddSubMenu(junglemenu);
            }

            var condemnmenu = new Menu("» Condemn Settings «", "Condemn Settings");
            {
                condemnmenu.AddItem(new MenuItem("condemn.distance", "» Condemn Push Distance",true).SetValue(new Slider(410, 350, 420)));
                condemnmenu.AddItem(new MenuItem("info.vayne.1", "                       Condemn Whitelist", true)).SetFontStyle(FontStyle.Bold, SharpDX.Color.Yellow);
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
                {
                    condemnmenu.AddItem(new MenuItem("condemnset." + enemy.CharData.BaseSkinName,
                        $"Condemn: {enemy.CharData.BaseSkinName}",true).SetValue(true));
                }
                Initializer.Config.AddSubMenu(condemnmenu);
            }

            Initializer.Config.AddItem(new MenuItem("masterracec0mb0", "                      Masterrace Settings"));
            Initializer.Config.AddItem(new MenuItem("condemn.style", "Condemn Method",true).SetValue(new StringList(new[] { "Shine", "Asuna", "360" })));
            Initializer.Config.AddItem(new MenuItem("condemn.x1", "Condemn Style",true).SetValue(new StringList(new[] { "Only Combo" })));
            Initializer.Config.AddItem(new MenuItem("q.type", "Q Type",true).SetValue(new StringList(new[] { "Cursor Position", "Safe Position" }, 1)));
            Initializer.Config.AddItem(new MenuItem("combo.type", "Combo Type",true).SetValue(new StringList(new[] { "Burst", "Normal" }, 1)));
            Initializer.Config.AddItem(new MenuItem("harass.type", "Harass Type",true).SetValue(new StringList(new[] { "2 Silver Stack + Q", "2 Silver Stack + E" })));
            Initializer.Config.AddItem(new MenuItem("q.stealth", "(Q) Stealth (ms) )",true).SetValue(new Slider(1000, 0, 1000)));

            Game.OnUpdate += VayneOnUpdate;
            Orbwalking.BeforeAttack += VayneBeforeAttack;
            Obj_AI_Base.OnSpellCast += VayneOnSpellCast;
            AntiGapcloser.OnEnemyGapcloser += VayneAntiGapcloser;

        }

        private void VayneAntiGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.End.Distance(ObjectManager.Player.ServerPosition) <= 300 && Utilities.Enabled("anti.gap"))
            {
                E.Cast(gapcloser.End.Extend(ObjectManager.Player.ServerPosition, ObjectManager.Player.Distance(gapcloser.End) + E.Range));
            }
        }

        private void VayneBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Unit.IsMe && args.Target.IsEnemy && args.Unit.HasBuff("vaynetumblefade"))
            {
                var stealthtime = Utilities.Slider("q.stealth");
                var stealthbuff = args.Unit.GetBuff("vaynetumblefade");
                if (stealthbuff.EndTime - Game.Time > stealthbuff.EndTime - stealthbuff.StartTime - stealthtime / 1000)
                {
                    args.Process = false;
                }
            }
            else
            {
                args.Process = true;
            }
        }

        private void VayneOnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && Orbwalking.IsAutoAttack(args.SData.Name) &&
                sender.Type == GameObjectType.AIHeroClient)
            {
                if (Utilities.Enabled("combo.q") && Q.IsReady() && !args.Target.IsDead &&
                    ((AIHeroClient)args.Target).IsValidTarget(ObjectManager.Player.AttackRange) &&
                    Initializer.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    QCast(((AIHeroClient)args.Target));
                }
            }
        }

        public static bool AsunasAllyFountain(Vector3 position)
        {
            float fountainRange = 750;
            var map = LeagueSharp.Common.Utility.Map.GetMap();
            if (map != null && map.Type == LeagueSharp.Common.Utility.Map.MapType.SummonersRift)
            {
                fountainRange = 1050;
            }
            return
                ObjectManager.Get<GameObject>().Where(spawnPoint => spawnPoint is Obj_SpawnPoint && spawnPoint.IsAlly).Any(spawnPoint => Vector2.Distance(position.To2D(), spawnPoint.Position.To2D()) < fountainRange);
        }

        public static void SelectedCondemn()
        {
            switch (Initializer.Config.Item("condemn.style",true).GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    foreach (var target in HeroManager.Enemies.Where(h => h.IsValidTarget(E.Range)))
                    {
                        if (Utilities.Enabled("condemnset." + target.CharData.BaseSkinName))
                        {
                            var pushDistance = Utilities.Slider("condemn.distance");
                            var targetPosition = E.GetPrediction(target).UnitPosition;
                            var pushDirection = (targetPosition - ObjectManager.Player.ServerPosition).Normalized();
                            float checkDistance = pushDistance / 40f;
                            for (int i = 0; i < 40; i++)
                            {
                                Vector3 finalPosition = targetPosition + (pushDirection * checkDistance * i);
                                var collFlags = NavMesh.GetCollisionFlags(finalPosition);
                                if (collFlags.HasFlag(CollisionFlags.Wall) || collFlags.HasFlag(CollisionFlags.Building)) //not sure about building, I think its turrets, nexus etc
                                {
                                    E.Cast(target);
                                }
                            }
                        }

                    }
                    break;
                case 1:
                    foreach (var En in HeroManager.Enemies.Where(hero => hero.IsValidTarget(E.Range) && !hero.HasBuffOfType(BuffType.SpellShield) && !hero.HasBuffOfType(BuffType.SpellImmunity)))
                    {
                        if (Utilities.Enabled("condemnset." + En.CharData.BaseSkinName))
                        {
                            var EPred = E.GetPrediction(En);
                            int pushDist = Utilities.Slider("condemn.distance");
                            var FinalPosition = EPred.UnitPosition.To2D().Extend(ObjectManager.Player.ServerPosition.To2D(), -pushDist).To3D();

                            for (int i = 1; i < pushDist; i += (int)En.BoundingRadius)
                            {
                                Vector3 loc3 = EPred.UnitPosition.To2D().Extend(ObjectManager.Player.ServerPosition.To2D(), -i).To3D();

                                if (loc3.IsWall() || AsunasAllyFountain(FinalPosition))
                                    E.Cast(En);
                            }
                        }
                    }
                    break;
                case 2:
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range) && !x.HasBuffOfType(BuffType.SpellShield) && !x.HasBuffOfType(BuffType.SpellImmunity) &&
                        IsCondemable(x)))
                    {
                        if (Utilities.Enabled("condemnset." + enemy.CharData.BaseSkinName))
                        {
                            E.Cast(enemy);
                        }
                    }
                    break;
            }
        }
        public static void ComboUltimateLogic()
        {
            if (ObjectManager.Player.CountEnemiesInRange(1000) >= Utilities.Slider("combo.r.count"))
            {
                R.Cast();
            }
        }
        public void SilverStackE()
        {
            if (Initializer.Config.Item("harass.type",true).GetValue<StringList>().SelectedIndex == 1)
            {
                foreach (AIHeroClient qTarget in HeroManager.Enemies.Where(x => x.IsValidTarget(550)))
                {
                    if (qTarget.Buffs.Any(buff => buff.Name == "vaynesilvereddebuff" && buff.Count == 2))
                    {
                        E.Cast(qTarget);
                    }
                }
            }

        }
        public void SilverStackQ()
        {
            if (Initializer.Config.Item("harass.type",true).GetValue<StringList>().SelectedIndex == 0)
            {
                foreach (AIHeroClient qTarget in HeroManager.Enemies.Where(x => x.IsValidTarget(550)))
                {
                    if (qTarget.Buffs.Any(buff => buff.Name == "vaynesilvereddebuff" && buff.Count == 2))
                    {
                        Q.Cast(Game.CursorPos);
                    }
                }
            }
        }

        public static void QCast(AIHeroClient enemy)
        {
            switch (Initializer.Config.Item("q.type",true).GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    Q.Cast(Game.CursorPos);
                    break;
                case 1:
                    Utilities.ECast(enemy,Q);
                    break;
            }
        }

        public static void QComboMethod()
        {
            switch (Initializer.Config.Item("combo.type",true).GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    foreach (AIHeroClient qTarget in HeroManager.Enemies.Where(x => x.IsValidTarget(550)))
                    {
                        if (qTarget.Buffs.Any(buff => buff.Name == "vaynesilvereddebuff" && buff.Count == 2))
                        {
                            QCast(qTarget);
                        }
                    }
                    break;
                case 1:
                    foreach (AIHeroClient qTarget in HeroManager.Enemies.Where(x => x.IsValidTarget(550)))
                    {
                        QCast(qTarget);
                    }
                    break;
            }
        }
        public static void CondemnJungleMobs()
        {
            foreach (var jungleMobs in ObjectManager.Get<Obj_AI_Minion>().Where(o => o.IsValidTarget(E.Range) && o.Team == GameObjectTeam.Neutral && o.IsHPBarRendered && !o.IsDead))
            {
                if (jungleMobs.CharData.BaseSkinName == "SRU_Razorbeak" || jungleMobs.CharData.BaseSkinName == "SRU_Red" ||
                    jungleMobs.CharData.BaseSkinName == "SRU_Blue" || jungleMobs.CharData.BaseSkinName == "SRU_Dragon" ||
                    jungleMobs.CharData.BaseSkinName == "SRU_Krug" || jungleMobs.CharData.BaseSkinName == "SRU_Gromp" ||
                    jungleMobs.CharData.BaseSkinName == "Sru_Crab")
                {
                    var pushDistance = Utilities.Slider("condemn.distance");
                    var targetPosition = E.GetPrediction(jungleMobs).UnitPosition;
                    var pushDirection = (targetPosition - ObjectManager.Player.ServerPosition).Normalized();
                    float checkDistance = pushDistance / 40f;
                    for (int i = 0; i < 40; i++)
                    {
                        Vector3 finalPosition = targetPosition + (pushDirection * checkDistance * i);
                        var collFlags = NavMesh.GetCollisionFlags(finalPosition);
                        if (collFlags.HasFlag(CollisionFlags.Wall) || collFlags.HasFlag(CollisionFlags.Building)) //not sure about building, I think its turrets, nexus etc
                        {
                            E.Cast(jungleMobs);
                        }
                    }
                }
            }
        }
        public static void JungleMobsQ()
        {
            var mob = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (mob == null || (mob.Count == 0))
                return;
            Q.Cast(Game.CursorPos);
        }
        public static bool IsCondemable(AIHeroClient unit, Vector2 pos = new Vector2())
        {
            if (unit.HasBuffOfType(BuffType.SpellImmunity) || unit.HasBuffOfType(BuffType.SpellShield) || LastCheck + 50 > Environment.TickCount || ObjectManager.Player.IsDashing()) return false;
            var prediction = E.GetPrediction(unit);
            var predictionsList = pos.IsValid() ? new List<Vector3>() { pos.To3D() } : new List<Vector3>
                        {
                            unit.ServerPosition,
                            unit.Position,
                            prediction.CastPosition,
                            prediction.UnitPosition
                        };

            var wallsFound = 0;
            Points = new List<Vector2>();
            foreach (var position in predictionsList)
            {
                for (var i = 0; i < Utilities.Slider("condemn.distance"); i += (int)unit.BoundingRadius) // 420 = push distance
                {
                    var cPos = ObjectManager.Player.Position.Extend(position, ObjectManager.Player.Distance(position) + i).To2D();
                    Points.Add(cPos);
                    if (NavMesh.GetCollisionFlags(cPos.To3D()).HasFlag(CollisionFlags.Wall) || NavMesh.GetCollisionFlags(cPos.To3D()).HasFlag(CollisionFlags.Building))
                    {
                        wallsFound++;
                        break;
                    }
                }
            }
            if ((wallsFound / predictionsList.Count) >= 33 / 100f)
            {
                return true;
            }

            return false;
        }
        private void VayneOnUpdate(EventArgs args)
        {
            switch (Initializer.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    OnCombo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    OnJungle();
                    break;
            }
        }

        private static void OnCombo()
        {
            if (Utilities.Enabled("q.combo") && Q.IsReady())
            {
                QComboMethod();
            }
            if (Utilities.Enabled("e.combo") && E.IsReady())
            {
                SelectedCondemn();
            }
            if (Utilities.Enabled("r.combo") && R.IsReady())
            {
                ComboUltimateLogic();
            }

        }

        private static void OnJungle()
        {
            if (ObjectManager.Player.ManaPercent < Utilities.Slider("jungle.mana"))
                return;

            if (Utilities.Enabled("q.jungle") && Q.IsReady())
            {
                JungleMobsQ();
            }
            if (Utilities.Enabled("e.jungle") && E.IsReady())
            {
                CondemnJungleMobs();
            }
        }
    }
}

