using System;
using System.Linq;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.UI;
using LeagueSharp.SDK.Utils;
using LeagueSharp.SDK.Enumerations;
using SharpDX;

using Menu = LeagueSharp.SDK.UI.Menu;
using SkillshotType = LeagueSharp.SDK.Enumerations.SkillshotType;
using Spell = LeagueSharp.SDK.Spell;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace EvictRyze
{
    class Program
    {
        public static Spell Q, W, E, QShort;
        public static Menu s_Menu;

        public static void Main()
        {
            Bootstrap.Init();
            OnLoad();
        }

        private static void OnLoad()
        {
            if (ObjectManager.Player.ChampionName != "Ryze")
                return;

            Q = new Spell(SpellSlot.Q, 900f);
            Q.SetSkillshot(0.25f, 70f, Q.Instance.SData.MissileSpeed, true, SkillshotType.SkillshotLine);
            QShort = new Spell(SpellSlot.Q, 600f);
            QShort.SetSkillshot(0.25f, 90f, Q.Instance.SData.MissileSpeed, true, SkillshotType.SkillshotLine);
            W = new Spell(SpellSlot.W, 600f);
            E = new Spell(SpellSlot.E, 600f);

            s_Menu = new Menu("ryze.root", "Ryze", true);
            Menu laneclear = new Menu("ryze.laneclear", "LaneClear");
            laneclear.Add(new MenuKeyBind("enable", "Enable LaneClear", System.Windows.Forms.Keys.L, KeyBindType.Toggle, ObjectManager.Player.ChampionName));

            Menu lasthit = new Menu("ryze.lasthit", "Last Hit");
            lasthit.Add(new MenuBool("useq", "Use Q", true, ObjectManager.Player.ChampionName));

            s_Menu.Add(new MenuSeparator("combo.info", "Combo Notation > (Q)>E>Q>W>E>Q", ObjectManager.Player.ChampionName));
            s_Menu.Add(laneclear);
            s_Menu.Add(lasthit);
            s_Menu.Attach();
            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.Slot == SpellSlot.Q)
                {
                    if (Items.HasItem(3030))
                        Items.UseItem(3030, args.End);
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            switch(Variables.Orbwalker.ActiveMode)
            {
                case OrbwalkingMode.Combo:
                    Combo();
                    break;
                case OrbwalkingMode.LaneClear:
                    LaneClear();
                    break;
                case OrbwalkingMode.LastHit:
                    LastHit();
                    break;
                default:
                    break;
            }
        }

        private static void Combo()
        {
            bool qcoll = false;
            var target = Variables.TargetSelector.GetTargetNoCollision(QShort);

            if (target == null)
            {
                target = Variables.TargetSelector.GetTarget(E.Range, DamageType.Magical);
                qcoll = true;
            }
            
            if (target != null)
            {
                if (Q.IsReady() && (!qcoll || ObjectManager.Player.HasBuff("ryzeqiconfullcharge") || (ObjectManager.Player.HasBuff("ryzeqiconfullcharge") && (ObjectManager.Player.GetBuff("ryzeqiconfullcharge").EndTime - Game.Time) * 1000f < 500)))
                {
                    if (W.IsReady())
                    {
                        if ((target.IsMelee && ObjectManager.Player.Distance(target) < 400) || (ObjectManager.Player.HasBuff("ryzeqiconhalfcharge") && ObjectManager.Player.HealthPercent < 20))
                            W.Cast(target);
                    }
                    if (ObjectManager.Player.HasBuff("ryzeqiconfullcharge"))
                        Q.Cast(target.ServerPosition);
                    else
                        Q.Cast(target);
                    return;
                }
                if (W.IsReady())
                {
                    if (target.IsValidTarget(W.Range))
                    {
                        W.Cast(target);
                        return;
                    }
                }
                if (E.IsReady())
                {
                    if (target.IsValidTarget(E.Range))
                    {
                        E.Cast(target);
                        return;
                    }
                }
            }
        }
        
        private static void LaneClear()
        {
            if (!s_Menu["ryze.laneclear"]["enable"].GetValue<MenuBool>().Value)
                return;

            var minion = ObjectManager.Get<Obj_AI_Minion>()
                .Where(p => !p.IsDead && !p.IsAlly && p.Distance(ObjectManager.Player.ServerPosition) < E.Range)
                .OrderByDescending(q => q.MaxHealth)
                .FirstOrDefault();

            if (minion != null)
            {
                if (minion.Health < EDmg(minion) + WDmg(minion) && ObjectManager.Get<Obj_AI_Minion>().Where(p => !p.IsDead && !p.IsAlly && p.Distance(minion) < 450 && p.NetworkId != minion.NetworkId).Count() > 2)
                {
                    if (minion.Health > EDmg(minion))
                        W.Cast(minion);
                    E.Cast(minion);
                }
            }
            else
            {
                minion = ObjectManager.Get<Obj_AI_Minion>()
                    .Where(p => !p.IsDead && !p.IsAlly && p.Distance(ObjectManager.Player.ServerPosition) < Q.Range && p.Health < QDmg(p))
                    .FirstOrDefault();

                if (minion != null && Q.GetCollision(ObjectManager.Player.ServerPosition.ToVector2(), new List<Vector2> { minion.ServerPosition.ToVector2() }).Count < 2)
                {
                    Q.Cast(minion.ServerPosition);
                }
            }
        }

        private static void LastHit()
        {
            if (!s_Menu["ryze.lasthit"]["useq"].GetValue<MenuBool>().Value)
                return;
            var minion = ObjectManager.Get<Obj_AI_Minion>()
                    .Where(p => !p.IsDead && !p.IsAlly && p.Distance(ObjectManager.Player.ServerPosition) < Q.Range && p.Health < QDmg(p))
                    .FirstOrDefault();

            if (minion != null && Q.GetCollision(ObjectManager.Player.ServerPosition.ToVector2(), new List<Vector2> { minion.ServerPosition.ToVector2() }).Count < 2)
            {
                Q.Cast(minion.ServerPosition);
            }
        }

        private static double QDmg(Obj_AI_Base target)
        {
            if (!Q.IsReady())
                return 0;
            return ObjectManager.Player.CalculateDamage(target, DamageType.Magical, new int[] { 60, 85, 110, 135, 160, 185 }[Q.Level - 1] + 0.45f * ObjectManager.Player.TotalMagicalDamage + 0.03 * ObjectManager.Player.Mana);
        }

        private static double WDmg(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return 0;
            return ObjectManager.Player.CalculateDamage(target, DamageType.Magical, new int[] { 50, 60, 70, 80, 90 }[W.Level - 1] + 0.2f * ObjectManager.Player.TotalMagicalDamage + 0.01 * ObjectManager.Player.Mana);
        }

        private static double EDmg(Obj_AI_Base target)
        {
            if (!E.IsReady())
                return 0;
            return ObjectManager.Player.CalculateDamage(target, DamageType.Magical, new int[] { 50, 75, 100, 125, 150 }[E.Level - 1] + 0.3f * ObjectManager.Player.TotalMagicalDamage + 0.02 * ObjectManager.Player.Mana);
        }
    }
}
