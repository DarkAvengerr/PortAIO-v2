using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SPrediction;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Nechrito_Gragas
{
    class Program
    {
        public static readonly int[] BlueSmite = { 3706, 1400, 1401, 1402, 1403 };

        public static readonly int[] RedSmite = { 3715, 1415, 1414, 1413, 1412 };

        public static GameObject GragasQ;

        private static Orbwalking.Orbwalker _orbwalker;

        public static AIHeroClient Player => ObjectManager.Player;

        private static readonly HpBarIndicator Indicator = new HpBarIndicator();

        private static void Main() => CustomEvents.Game.OnGameLoad += OnGameLoad;

        private static void OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Gragas") return;

            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Nechrito Gragas</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> Version: 7 (Date: 27/6-16)</font></b>");

            MenuConfig.LoadMenu();
            Spells.Initialise();

            Game.OnUpdate += Mode.Game_OnUpdate;
            Game.OnUpdate += OnTick;

            Obj_AI_Base.OnSpellCast += OnSpellCast;

            GameObject.OnCreate += OnCreateObject;
            GameObject.OnDelete += GameObject_OnDelete;

            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }
        private static void OnTick(EventArgs args)
        {
            SmiteJungle();
            SmiteCombo();
            Killsteal();
            

            switch (MenuConfig._orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Mode.ComboLogic();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Mode.JungleLogic();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Mode.HarassLogic();
                    break;
            }
        }

        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
            
            if (sender.Name == "Gragas_Base_Q_Ally.troy")
            {
                GragasQ = sender;
            }
        }

        static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Gragas_Base_Q_Ally.troy")
            {
                GragasQ = null;
            }
        }

        private static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Player.Spellbook.IsAutoAttacking) return;

            if (args.Target is Obj_AI_Minion)
            {
                if (MenuConfig._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    var minions = MinionManager.GetMinions(Player.ServerPosition, 600);
                    {
                        if (minions == null)
                            return;

                        foreach (var m in minions)
                        {
                            if (Spells.E.IsReady() && MenuConfig.LaneE)
                            {
                                if (m.Health < Spells.E.GetDamage(m))
                                {
                                    Spells.E.Cast(GetCenterMinion());
                                }
                            }

                            if (Spells.Q.IsReady() && MenuConfig.LaneQ)
                            {
                                if (GragasQ == null)
                                {
                                    Spells.Q.Cast(GetCenterMinion(), true);
                                }
                                if (GragasQ != null && m.Distance(GragasQ.Position) <= 250 && m.Health < Spells.Q.GetDamage(m))
                                {
                                    Spells.Q.Cast(true);
                                }
                            }
                            if(m.Distance(Player) <= 250f)
                            {
                                if (Spells.W.IsReady() && MenuConfig.LaneW)
                                {
                                    Spells.W.Cast();
                                }
                            }
                        }
                    }
                }
            }
        }
    
        public static Obj_AI_Base GetCenterMinion()
        {
            var minionposition = MinionManager.GetMinions(300 + Spells.Q.Range).Select(x => x.Position.To2D()).ToList();
            var center = MinionManager.GetBestCircularFarmLocation(minionposition, 250, 300 + Spells.Q.Range);

            return center.MinionsHit >= 4
                ? MinionManager.GetMinions(1000).OrderBy(x => x.Distance(center.Position)).FirstOrDefault()
                : null;
        }

        private static void Killsteal()
        {
            if (Spells.Q.IsReady() && Spells.R.IsReady())
            {
                var targets = HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.Q.Range) && !x.IsZombie);
                foreach (var target in targets)
                {
                    if (target.Health < Spells.Q.GetDamage(target) + Spells.Q.GetDamage(target))
                    {
                        var pos = Spells.R.GetSPrediction(target).CastPosition + 60;

                        Spells.Q.Cast(Mode.rpred(target));
                        Spells.R.Cast(Mode.rpred(target));
                    }
                }
            }

            if(Spells.Q.IsReady())
            {
                var targets = HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.Q.Range) && !x.IsZombie);
                foreach (var target in targets)
                {
                    if (!(target.Health < Spells.Q.GetDamage(target))) return;
                    if (GragasQ == null)
                    {
                        var pos = Spells.Q.GetSPrediction(target).CastPosition;
                        Spells.Q.Cast(pos, true);
                    }
                    if(GragasQ != null && target.Distance(GragasQ.Position) <= 250)
                    {
                        Spells.Q.Cast();
                    }
                }
            }

            if (Spells.E.IsReady())
            {
                var targets = HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.E.Range) && !x.IsZombie);
                foreach (var target in targets)
                {
                    if (target.Health < Spells.E.GetDamage(target))
                    {
                        var pos = Spells.E.GetSPrediction(target).CastPosition;
                        Spells.E.Cast(pos);
                    }
                }
            }
        }
        
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead || !MenuConfig.prediction) return;

            var Target = TargetSelector.GetSelectedTarget();

            if(Target != null && !Target.IsDead)
            {
                Render.Circle.DrawCircle(Mode.rpred(Target), 100, System.Drawing.Color.GhostWhite);
                Render.Circle.DrawCircle(Mode.qpred(Target), 100, System.Drawing.Color.Blue);
            }
            Render.Circle.DrawCircle(Player.Position, Spells.R.Range, Spells.R.IsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed);
        }
        private static void Drawing_OnEndScene(EventArgs args)
        {
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.IsValidTarget() && !ene.IsZombie))
            {
                if (MenuConfig.dind)
                {
                    var lethal = Spells.R.IsReady() && Dmg.IsLethal(enemy)
                        ? new ColorBGRA(0, 255, 0, 120)
                        : new ColorBGRA(255, 255, 0, 120);

                    Indicator.unit = enemy;
                    Indicator.drawDmg(Dmg.ComboDmg(enemy), lethal);
                }
            }
        }
        protected static void SmiteCombo()
        {
            if (BlueSmite.Any(id => Items.HasItem(id)))
            {
                Spells.Smite = Player.GetSpellSlot("s5_summonersmiteplayerganker");
                return;
            }

            if (RedSmite.Any(id => Items.HasItem(id)))
            {
                Spells.Smite = Player.GetSpellSlot("s5_summonersmiteduel");
                return;
            }

            Spells.Smite = Player.GetSpellSlot("summonersmite");
        }

        protected static void SmiteJungle()
        {
            foreach (var minion in MinionManager.GetMinions(900f, MinionTypes.All, MinionTeam.Neutral))
            {
                var damage = Player.Spellbook.GetSpell(Spells.Smite).State == SpellState.Ready
                    ? (float)Player.GetSummonerSpellDamage(minion, Damage.SummonerSpell.Smite)
                    : 0;
                if (minion.Distance(Player.ServerPosition) <= 550)
                {
                    if ((minion.CharData.BaseSkinName.Contains("Dragon") || minion.CharData.BaseSkinName.Contains("Baron")))
                    {
                        if (damage >= minion.Health)
                            Player.Spellbook.CastSpell(Spells.Smite, minion);
                    }
                }

            }

        }
    }
}
