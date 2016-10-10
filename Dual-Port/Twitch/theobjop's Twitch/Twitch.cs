using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using Color = System.Drawing.Color;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy;
using LeagueSharp.Common;
namespace Twiitch
{
    class Twitch
    {

        private static string ChampionName = "Twitch";

        public static Orbwalking.Orbwalker _orbwalker;

        private static Spell _q;
        private static Spell _w;
        private static Spell _e;
        private static Spell _r;

        private static float lastHpPercent = 100;
        private static int lastKills = 0;
        private static Vector3 buffPosition;

        public static AIHeroClient Player { get { return ObjectManager.Player; } }

        // Thankyou, Tr33s
        private static readonly string[] FocusMinions =
        {
            "SRU_Red", "SRU_Blue", "SRU_Dragon", "SRU_Baron",
            "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak", "SRU_Krug", "Sru_Crab", "TT_Spiderboss", "TTNGolem", "TTNWolf",
            "TTNWraith"
        };


        private static float GetDamage(AIHeroClient target)
        {
            return _e.GetDamage(target);
        }

        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            // Verify champion
            if (Player.ChampionName != ChampionName)
                return;

            Notifications.AddNotification("Twitch by TheOBJop", 2);

            // Spells
            _q = new Spell(SpellSlot.Q, 1500);
            _w = new Spell(SpellSlot.W, 950);
            _w.SetSkillshot(0.25f, 120f, 1400f, false, SkillshotType.SkillshotCircle);
            _e = new Spell(SpellSlot.E, 1200);
            _r = new Spell(SpellSlot.R, 850);

            lastKills = Player.ChampionsKilled;

            // Menu
            TwitchMenu.Init();

            CustomDamageIndicator.Initialize(GetDamage);

            // Listen to Events
            Game.OnUpdate += Game_OnUpdate;
            CustomEvents.Unit.OnDash += Unit_OnDash;
            Game.OnNotify += Game_OnNotify;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        // Anti-Gapclose
        private static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            if (!sender.IsEnemy)
                return;

            if (!args.IsBlink && TwitchMenu.Item("WAntigap").GetValue<bool>() && _w.IsReady() && Player.Distance(sender) <= (_w.Range / 1.5) && Player.Distance(args.StartPos) > Player.Distance(args.EndPos))
                _w.Cast(args.EndPos);
        }

        // Auto Q after kill if Enemies
        private static void Game_OnNotify(GameNotifyEventArgs args)
        {
            switch (args.EventId)
            {
                case GameEventId.OnChampionKill:
                    if (WasYourKill() && TwitchMenu.Item("QKill").GetValue<bool>() && Player.GetEnemiesInRange(_q.Range).Count >= TwitchMenu.Item("QFleeCount").GetValue<Slider>().Value && _q.IsReady())
                        _q.Cast();
                    break;
                case GameEventId.OnDamageTaken:
                    if (TwitchMenu.Item("EBeforeDeath").GetValue<bool>() && (Player.HealthPercent <= 15 || (lastHpPercent - Player.HealthPercent) > 25) && _e.IsReady())
                        _e.Cast();
                    lastHpPercent = Player.HealthPercent;
                    break;
                case GameEventId.OnBuff:
                    if (Player.HasBuff("TwitchHideInShadows"))
                        buffPosition = Player.Position;
                    break;
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            CustomDamageIndicator.DrawingColor = TwitchMenu.Item("EDamage").GetValue<Circle>().Color;
            CustomDamageIndicator.Enabled = TwitchMenu.Item("EDamage").GetValue<Circle>().Active;

            if (TwitchMenu.Item("DrawQRange").GetValue<bool>())
                Drawing.DrawCircle(buffPosition, _q.Range, Color.Green);
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            // Update Q range based on Movespeed and drawing position
            _q.Range = Player.MoveSpeed * (_q.Level + 3.0f);
            if (!Player.HasBuff("TwitchHideInShadows"))
                buffPosition = Player.Position;

            // Killsteal with E
            if (_e.IsReady())
                if (TwitchMenu.Item("EKillsteal").GetValue<bool>())
                    foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsValidTarget(_e.Range) && _e.IsKillable(enemy)))
                        _e.Cast();

            // E before death
            if (TwitchMenu.Item("EBeforeDeath").GetValue<bool>() && Player.HealthPercent <= 10 && _e.IsReady())
                _e.Cast();

            // Always KS monsters
            KsMonsters();

            // R to KS
            RToKs();

            // Orbwalker Mode
            switch (_orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(_w.Range, TargetSelector.DamageType.Physical);

            // If target not found, don't do anything.
            if (target == null)
                return;

            // Use W
            if (TwitchMenu.Item("UseWCombo").GetValue<bool>())
            {
                if (target.IsValidTarget(_w.Range) && _w.CanCast(target) && _w.GetPrediction(target).Hitchance >= HitChance.High)
                    _w.Cast(target);
            }

            // Use BoRK
            if (target.Type == Player.Type && target.ServerPosition.Distance(Player.ServerPosition) < 450)
            {
                var hasCutlass = Items.HasItem(3144);
                var hasBork = Items.HasItem(3153);

                if (hasBork || hasCutlass)
                {
                    var itemId = hasCutlass ? 3144 : 3153;
                    var damage = Player.GetItemDamage(target, Damage.DamageItems.Botrk);
                    if (hasCutlass || Player.Health + damage < Player.MaxHealth)
                        Items.UseItem(itemId, target);
                }
            }

            // Use Youmuu's
            if (target.Type == Player.Type && Orbwalking.InAutoAttackRange(target))
                Items.UseItem(3142);
        }

        private static void KsMonsters()
        {
            // If not KS enabled, don't KS
            if (!TwitchMenu.Item("KSMonsters").GetValue<bool>())
                return;

            // Find Minions
            var minions = MinionManager.GetMinions(_e.Range, MinionTypes.All, MinionTeam.NotAlly);
            foreach (var m in minions)
            {
                if ((m.CharData.BaseSkinName.Contains("MinionSiege") || m.CharData.BaseSkinName.Contains("Dragon") || m.CharData.BaseSkinName.Contains("Baron")) && _e.IsKillable(m))
                    _e.Cast();
            }
        }

        private static void RToKs()
        {
            AIHeroClient enemy = Player.GetEnemiesInRange(_r.Range).FirstOrDefault(nmy => nmy.HealthPercent < 10);

            if (enemy == null)
                return;

            // Make sure to only KS the enemy (Kinda like auto-activating R to secure kill)
            if (TwitchMenu.Item("RToKS").GetValue<bool>() && _r.IsReady() && Player.Distance(enemy) > 500)
            {
                _r.Cast();
                _orbwalker.ForceTarget(enemy);
            }
        }

        private static bool WasYourKill()
        {
            if (Player.ChampionsKilled > lastKills)
            {
                lastKills = Player.ChampionsKilled;
                return true;
            }

            return false;
        }

        private static Obj_AI_Base GetCenterMinion(List<Obj_AI_Base> objects)
        {
            Obj_AI_Base highest = objects.FirstOrDefault();
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i].GetAlliesInRange(_w.Width).Count > highest.GetAlliesInRange(_w.Width).Count)
                    highest = objects[i];
            }

            return highest;
        }

        private static void JungleClear()
        {
            var minions = MinionManager.GetMinions(_e.Range, MinionTypes.All, MinionTeam.Neutral);

            if (minions == null)
                return;

            var highestHealthMinion = minions.FirstOrDefault(buff => buff.IsValidTarget() && FocusMinions.Contains(buff.CharData.BaseSkinName));
            var centerMinion = _w.GetCircularFarmLocation(minions);
            var useE = TwitchMenu.Item("EJGClear").GetValue<bool>();
            var useW = TwitchMenu.Item("WJGClear").GetValue<bool>();
            var minMobsForW = TwitchMenu.Item("WJGMinionCount").GetValue<Slider>().Value;
            var minMana = TwitchMenu.Item("JGMana").GetValue<Slider>().Value;

            if (useW && _w.IsReady() && (minions.Count(m => Player.Distance(m, false) < _w.Range) >= minMobsForW) && Player.ManaPercent > minMana)
                _w.Cast(centerMinion.Position);

            if (highestHealthMinion == null)
                return;

            if (useE && _e.IsReady() && _e.IsKillable(highestHealthMinion) && Player.ManaPercent > minMana)
                _e.Cast(highestHealthMinion);
        }

        private static void LaneClear()
        {
            var minions = MinionManager.GetMinions(_e.Range, MinionTypes.All, MinionTeam.Enemy);

            if (minions == null)
                return;

            var centerMinion = _w.GetCircularFarmLocation(minions);
            var useE = TwitchMenu.Item("ELaneClear").GetValue<bool>();
            var minMobsForE = TwitchMenu.Item("ELaneMinionCount").GetValue<Slider>().Value;
            var useW = TwitchMenu.Item("WLaneClear").GetValue<bool>();
            var minMobsForW = TwitchMenu.Item("WLaneMinionCount").GetValue<Slider>().Value;
            var minMana = TwitchMenu.Item("LaneMana").GetValue<Slider>().Value;

            if (useE && _e.IsReady() && minions.Count(m => _e.CanCast(m) && m.Health <= _e.GetDamage(m)) >= minMobsForE && Player.ManaPercent > minMana)
                _e.Cast();

            if (useW && _w.IsReady() && (minions.Count(m => Player.Distance(m, false) < _w.Range) >= minMobsForW) && Player.ManaPercent > minMana)
                _w.Cast(centerMinion.Position);
        }
    }
}
