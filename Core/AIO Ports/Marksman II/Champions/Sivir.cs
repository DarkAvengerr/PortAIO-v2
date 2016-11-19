#region
using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using Marksman.Utils;
using Orbwalking = LeagueSharp.Common.Orbwalking;

#endregion

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Marksman.Champions
{
    using System.Collections.Generic;
    using SharpDX;
    using Color = System.Drawing.Color;
    using Marksman.Utils;

    internal class DangerousSpells
    {
        public string SpellName { get; private set; }
        public string ChampionName { get; private set; }
        public SpellSlot SpellSlot { get; private set; }

        public DangerousSpells(string spellName, string championName, SpellSlot spellSlot)
        {
            SpellName = spellName;
            ChampionName = championName;
            SpellSlot = spellSlot;
        }
    }

    

    internal class Sivir : Champion
    {
        public static Spell Q;
        public Spell E;
        public Spell W;
        

        public static List<DangerousSpells> DangerousTargetedSpells = new List<DangerousSpells>();

        public Sivir()
        {
            Q = new Spell(SpellSlot.Q, 1220);
            Q.SetSkillshot(0.25f, 90f, 1350f, false, SkillshotType.SkillshotLine);

            W = new Spell(SpellSlot.W, 593);

            E = new Spell(SpellSlot.E);

            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;

            DangerousTargetedSpells.Add(new DangerousSpells("dariusR", "darius", SpellSlot.R));
            DangerousTargetedSpells.Add(new DangerousSpells("viR", "vi", SpellSlot.R));
            DangerousTargetedSpells.Add(new DangerousSpells("veigarR", "veigar", SpellSlot.R));
            DangerousTargetedSpells.Add(new DangerousSpells("fiddlesticksQ", "fiddlesticks", SpellSlot.Q));
            DangerousTargetedSpells.Add(new DangerousSpells("chogathR", "chogath", SpellSlot.R));
            DangerousTargetedSpells.Add(new DangerousSpells("garenR", "garen", SpellSlot.R));
            DangerousTargetedSpells.Add(new DangerousSpells("leesinR", "leesin", SpellSlot.R));
            DangerousTargetedSpells.Add(new DangerousSpells("nautiliusR", "nautilius", SpellSlot.R));
            DangerousTargetedSpells.Add(new DangerousSpells("skarnerR", "skarner", SpellSlot.R));
            DangerousTargetedSpells.Add(new DangerousSpells("syndraR", "syndra", SpellSlot.R));
            DangerousTargetedSpells.Add(new DangerousSpells("warwickR", "warwick", SpellSlot.R));
            DangerousTargetedSpells.Add(new DangerousSpells("zedR", "zed", SpellSlot.R));
            DangerousTargetedSpells.Add(new DangerousSpells("tristanaR", "tristana", SpellSlot.R));
            DangerousTargetedSpells.Add(new DangerousSpells("malzaharR", "malzahar", SpellSlot.R));
            DangerousTargetedSpells.Add(new DangerousSpells("urgotR", "urgot", SpellSlot.R));
            DangerousTargetedSpells.Add(new DangerousSpells("trundleR", "trundle", SpellSlot.R));

            DangerousTargetedSpells.Add(new DangerousSpells("maokaiW", "maokai", SpellSlot.W));
            DangerousTargetedSpells.Add(new DangerousSpells("alistarQ", "alistar", SpellSlot.W));
            DangerousTargetedSpells.Add(new DangerousSpells("nasusW", "nasus", SpellSlot.W));
            DangerousTargetedSpells.Add(new DangerousSpells("pantheonW", "pantheon", SpellSlot.W));
            DangerousTargetedSpells.Add(new DangerousSpells("ryzeW", "ryze", SpellSlot.W));
            DangerousTargetedSpells.Add(new DangerousSpells("rammusE", "rammus", SpellSlot.E));
            DangerousTargetedSpells.Add(new DangerousSpells("singedW", "singed", SpellSlot.W));
            DangerousTargetedSpells.Add(new DangerousSpells("kayleQ", "kayle", SpellSlot.Q));

            Utils.PrintMessage("Sivir");
        }

        public override void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (!W.IsReady())
            {
                return;
            }

            if (GetValue<bool>("Misc.UseW.Inhibitor") && args.Target is Obj_BarracksDampener)
            {
                W.Cast();
            }

            if (GetValue<bool>("Misc.UseW.Nexus") && args.Target is Obj_HQ)
            {
                W.Cast();
            }

            if (GetValue<bool>("Misc.UseW.Turret") && args.Target is Obj_AI_Turret)
            {
                W.Cast();
            }
        }

        public void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.Type != GameObjectType.AIHeroClient)
            {
                return;
            }

            if (!sender.IsValid || sender.Team == ObjectManager.Player.Team)
            {
                return;
            }

            if (W.IsReady())
            {
                if (sender.IsEnemy && sender is AIHeroClient && args.Target.IsMe)
                {
                    foreach (
                        var c in
                            DangerousTargetedSpells.Where(c => ((AIHeroClient)sender).ChampionName.ToLower() == c.ChampionName)
                                .Where(c => args.Slot == c.SpellSlot))
                    //.Where(c => args.SData.Name == ((AIHeroClient)sender).GetSpell(c.SpellSlot).Name))
                    {
                        W.Cast();
                    }
                }

                var enemy = (AIHeroClient)sender;
                if ((enemy.CharData.BaseSkinName.ToLower() == "vayne" || enemy.CharData.BaseSkinName.ToLower() == "poppy") && args.Slot == SpellSlot.E &&
                    args.Target.IsMe)
                {
                    for (var i = 1; i < 8; i++)
                    {
                        var myBehind = ObjectManager.Player.Position +
                                       Vector3.Normalize(enemy.ServerPosition -
                                                         ObjectManager.Player.Position) * (-i * 50);
                        if (myBehind.IsWall())
                        {
                            W.Cast();
                        }
                    }
                }

                if (enemy.CharData.BaseSkinName.ToLower() == "riven" && args.Slot == SpellSlot.W && enemy.Position.Distance(ObjectManager.Player.Position) < Orbwalking.GetRealAutoAttackRange(null) + 150)
                {
                    W.Cast();
                    DodgeMessage("Riven's W");
                }


                if (enemy.CharData.BaseSkinName.ToLower() == "diana" && args.Slot == SpellSlot.E && enemy.Position.Distance(ObjectManager.Player.Position) <= 350)
                {
                    W.Cast();
                    DodgeMessage("Diana E");
                }

                if (enemy.CharData.BaseSkinName.ToLower() == "MasterYi" && args.Slot == SpellSlot.E && enemy.Health < ObjectManager.Player.Health && args.Target.IsMe)
                {
                    W.Cast();
                    DodgeMessage("MasterYi E");
                }

                if (enemy.CharData.BaseSkinName.ToLower() == "darius" && args.Slot == SpellSlot.E && enemy.Position.Distance(ObjectManager.Player.Position) <= 550 && enemy.Level <= 5)
                {
                    W.Cast();
                    DodgeMessage("Darius E");
                }

                if (enemy.CharData.BaseSkinName.ToLower() == "blitzcrank" && args.Slot == SpellSlot.R && enemy.Position.Distance(ObjectManager.Player.Position) < Orbwalking.GetRealAutoAttackRange(null) + 300)
                {
                    W.Cast();
                    DodgeMessage("Blitzcrank's R");
                }
                if (enemy.CharData.BaseSkinName.ToLower() == "lissandra" && args.Slot == SpellSlot.R && enemy.Position.Distance(ObjectManager.Player.Position) < 500)
                {
                    W.Cast();
                    DodgeMessage("Lissandra's R");
                }
            }
        }

        private static void DodgeMessage(string text)
        {
            Chat.Print("<font color='#ff3232'>Dodged: </font><font color='#d4d4d4'><font color='#FFFFFF'>" + text + "</font>");
        }


        public override void GameOnUpdate(EventArgs args)
        {
            if (GetValue<bool>("AutoQ"))
            {
                var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                if (Q.IsReady() && t.IsValidTarget())
                {
                    if ((t.HasBuffOfType(BuffType.Slow) || t.HasBuffOfType(BuffType.Stun) ||
                         t.HasBuffOfType(BuffType.Snare) || t.HasBuffOfType(BuffType.Fear) ||
                         t.HasBuffOfType(BuffType.Taunt)))
                    {
                        Q.CastIfHitchanceGreaterOrEqual(t);
                        //CastQ();
                    }
                }
            }

            if (ComboActive || HarassActive)
            {
                var useQ = GetValue<bool>("UseQ" + (ComboActive ? "C" : "H"));

                if (Q.IsReady() && useQ)
                {
                    var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                    if (t != null)
                    {
                        Q.CastIfHitchanceGreaterOrEqual(t);
                        //CastQ();
                    }
                }
            }
        }

        public override void ExecuteJungle()
        {
            var jungleMobs = Utils.GetMobs(Q.Range, Marksman.Utils.Utils.MobTypes.All);

            if (jungleMobs != null)
            {
                if (Q.IsReady())
                {
                    switch (Program.Config.Item("UseQ.Jungle").GetValue<StringList>().SelectedIndex)
                    {
                        case 1:
                        {
                            Q.Cast(jungleMobs);
                            break;
                        }
                        case 2:
                        {
                            jungleMobs = Utils.GetMobs(Q.Range, Utils.MobTypes.BigBoys);
                            if (jungleMobs != null)
                            {
                                Q.Cast(jungleMobs);
                            }
                            break;
                        }
                    }
                }

                if (W.IsReady())
                {
                    var jW = Program.Config.Item("UseW.Jungle").GetValue<StringList>().SelectedIndex;
                    if (jW != 0)
                    {
                        if (jW == 1)
                        {
                            jungleMobs = Utils.GetMobs(Orbwalking.GetRealAutoAttackRange(null) + 65,
                                Utils.MobTypes.BigBoys);
                            if (jungleMobs != null)
                            {
                                Q.Cast();
                            }
                        }
                        else
                        {
                            var totalAa =
                                ObjectManager.Get<Obj_AI_Minion>()
                                    .Where(
                                        m =>
                                            m.Team == GameObjectTeam.Neutral &&
                                            m.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 165))
                                    .Sum(mob => (int) mob.Health);
                            totalAa = (int) (totalAa/ObjectManager.Player.TotalAttackDamage);
                            if (totalAa > jW)
                            {
                                W.Cast();
                            }
                        }
                    }
                }
            }
        }

        public override void ExecuteLane()
        {
            var qJ = Program.Config.Item("UseQ.Lane").GetValue<StringList>().SelectedIndex;
            if (qJ != 0)
            {
                var minionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All);
                if (minionsQ != null)
                {
                    if (Q.IsReady())
                    {
                        var locQ = Q.GetLineFarmLocation(minionsQ);
                        if (minionsQ.Count == minionsQ.Count(m => ObjectManager.Player.Distance(m) < Q.Range) &&
                            locQ.MinionsHit > qJ && locQ.Position.IsValid())
                        {
                            Q.Cast(locQ.Position);
                        }
                    }
                }
            }
            var wJ = Program.Config.Item("UseW.Lane").GetValue<StringList>().SelectedIndex;
            if (wJ != 0)
            {
                var minionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition,
                    Orbwalking.GetRealAutoAttackRange(null) + 165, MinionTypes.All);
                if (minionsW != null && minionsW.Count >= wJ)
                {
                    if (W.IsReady())
                    {
                        W.Cast();
                    }
                }
            }
        }

        public override void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            var t = target as AIHeroClient;
            if (t != null && (ComboActive || HarassActive) && unit.IsMe)
            {
                var useW = GetValue<bool>("UseWC");

                if (W.IsReady() && useW)
                {
                    W.Cast();
                }
            }
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            Spell[] spellList = {Q};
            foreach (var spell in spellList)
            {
                var menuItem = GetValue<Circle>("Draw" + spell.Slot);
                if (menuItem.Active)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, menuItem.Color);
            }
        }

        public override bool ComboMenu(Menu config)
        {
            config.AddItem(new MenuItem("UseQC" + Id, "Use Q").SetValue(true));
            config.AddItem(new MenuItem("UseWC" + Id, "Use W").SetValue(true));
            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.AddItem(new MenuItem("UseQH" + Id, "Use Q").SetValue(false));
            return true;
        }

        public override bool MiscMenu(Menu config)
        {
            config.AddItem(new MenuItem("AutoQ" + Id, "Auto Q on Stun/Slow/Fear/Taunt/Snare").SetValue(true));
            config.AddItem(new MenuItem("Misc.UseW.Turret" + Id, "Use W for Turret").SetValue(false));
            config.AddItem(new MenuItem("Misc.UseW.Inhibitor" + Id, "Use W for Inhibitor").SetValue(false));
            config.AddItem(new MenuItem("Misc.UseW.Nexus" + Id, "Use W for Nexus").SetValue(false));
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.AddItem(
                new MenuItem("DrawQ" + Id, "Q range").SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            return true;
        }

        public override bool LaneClearMenu(Menu menuLane)
        {
            string[] strQ = new string[5];
            strQ[0] = "Off";

            for (var i = 1; i < 5; i++)
            {
                strQ[i] = "Minion Count >= " + i;
            }

            menuLane.AddItem(new MenuItem("UseQ.Lane", Utils.Tab + "Use Q:").SetValue(new StringList(strQ, 0)));
            menuLane.AddItem(new MenuItem("UseQR.Lane", Utils.Tab + "Use Q for out of AA Range").SetValue(true));


            string[] strW = new string[5];
            strW[0] = "Off";

            for (var i = 1; i < 5; i++)
            {
                strW[i] = "Minion Count >= " + i;
            }

            menuLane.AddItem(new MenuItem("UseW.Lane", Utils.Tab + "Use W:").SetValue(new StringList(strW, 0)));

            return true;
        }

        public override bool JungleClearMenu(Menu menuJungle)
        {
            menuJungle.AddItem(
                new MenuItem("UseQ.Jungle", "Use Q").SetValue(
                    new StringList(new[] {"Off", "On", "Just for big Monsters"}, 1)));

            string[] strW = new string[8];
            strW[0] = "Off";
            strW[1] = "Just for big Monsters";

            for (var i = 2; i < 8; i++)
            {
                strW[i] = "If need to AA more than >= " + i;
            }

            menuJungle.AddItem(new MenuItem("UseW.Jungle", "Use W").SetValue(new StringList(strW, 4)));

            return true;
        }
    }
}
