using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_Fiora.Evade
{
    using System;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using System.Linq;

    internal class WDodgeSpell
    {
        private static Menu Menu;
        private static int RivenQTime;
        private static float RivenQRange;
        private static Vector2 RivenDashPos;

        internal static void Init(Menu mainMenu)
        {
            Menu = mainMenu;

            foreach (
                var hero in
                ObjectManager.Get<AIHeroClient>()
                    .Where(
                        x =>
                            x.IsEnemy &&
                            WSpellDataBase.Spells.Any(
                                a =>
                                    string.Equals(x.ChampionName, a.ChampionName,
                                        StringComparison.CurrentCultureIgnoreCase))))
            {
                Menu.AddSubMenu(new Menu(hero.ChampionName, hero.ChampionName.ToLower()));
            }

            foreach (
                var spell in
                WSpellDataBase.Spells.Where(
                    x =>
                        ObjectManager.Get<AIHeroClient>().Any(
                            a => a.IsEnemy &&
                                 string.Equals(a.ChampionName, x.ChampionName,
                                     StringComparison.CurrentCultureIgnoreCase))))
            {
                Menu.SubMenu(spell.ChampionName.ToLower())
                    .AddItem(new MenuItem(spell.ChampionName.ToLower() + spell.SpellSlot,
                        spell.ChampionName + " " + spell.SpellSlot, true).SetValue(true));
            }

            Menu.AddItem(new MenuItem("EnabledWDodge", "Enabled W Dodge", true).SetValue(true));

            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Obj_AI_Base.OnPlayAnimation += OnPlayAnimation;
            CustomEvents.Unit.OnDash += OnDash;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || !ObjectManager.Player.GetSpell(SpellSlot.W).IsReady() ||
                !Menu.Item("EnabledWDodge", true).GetValue<bool>())
            {
                return;
            }

            var buffs = ObjectManager.Player.Buffs;

            foreach (var buff in buffs)
            {
                var time = buff.EndTime;

                switch (buff.Name.ToLower())
                {
                    case "karthusfallenonetarget":
                        if ((time - Game.Time)*1000 <= 300)
                        {
                            CastW();
                        }
                        break;
                    case "nautilusgrandlinetarget":
                        if ((time - Game.Time)*1000 <= 300)
                        {
                            CastW();
                        }
                        break;
                    case "nocturneparanoiadash":
                        if (HeroManager.Enemies.FirstOrDefault(
                                x =>
                                    !x.IsDead && x.ChampionName.ToLower() == "nocturne" &&
                                    x.Distance(ObjectManager.Player) < 500) != null)
                        {
                            CastW();
                        }
                        break;
                    case "soulshackles":
                        if ((time - Game.Time)*1000 <= 300)
                        {
                            CastW();
                        }
                        break;
                    case "vladimirhemoplaguedebuff":
                        if ((time - Game.Time)*1000 <= 300)
                        {
                            CastW();
                        }
                        break;
                    case "zedrdeathmark":
                        if ((time - Game.Time)*1000 <= 300)
                        {
                            CastW();
                        }
                        break;
                }
            }

            foreach (var target in HeroManager.Enemies.Where(x => !x.IsDead && x.IsValidTarget()))
            {
                switch (target.ChampionName)
                {
                    case "Jax":
                        if (Menu.Item("jaxE", true).GetValue<bool>())
                        {
                            if (target.HasBuff("jaxcounterstrike"))
                            {
                                var buff = target.GetBuff("jaxcounterstrike");

                                if ((buff.EndTime - Game.Time)*1000 <= 650 &&
                                    ObjectManager.Player.Distance(target) <= 350f)
                                {
                                    CastW();
                                }
                            }
                        }
                        break;
                    case "Riven":
                        if (Menu.Item("rivenQ", true).GetValue<bool>())
                        {
                            if (Utils.GameTimeTickCount - RivenQTime <= 100 && RivenDashPos.IsValid() &&
                                ObjectManager.Player.Distance(target) <= RivenQRange)
                            {
                                CastW();
                            }
                        }
                        break;
                }
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!Menu.Item("EnabledWDodge", true).GetValue<bool>())
            {
                return;
            }

            var target = sender as AIHeroClient;

            if (target == null || target.Team == ObjectManager.Player.Team || !target.IsValid)
            {
                return;
            }

            var spells =
                WSpellDataBase.Spells.Where(
                    x =>
                        string.Equals(x.ChampionName, target.ChampionName, StringComparison.CurrentCultureIgnoreCase) &&
                        Menu.Item(x.ChampionName.ToLower() + x.SpellSlot, true) != null &&
                        Menu.Item(x.ChampionName.ToLower() + x.SpellSlot, true).GetValue<bool>());

            if (spells.Any())
            {
                foreach (var x in spells)
                {
                    switch (x.ChampionName)
                    {
                        case "Alistar":
                            if (x.SpellSlot == SpellSlot.Q)
                            {
                                if (target.Distance(ObjectManager.Player) <= 350)
                                {
                                    CastW("Alistar", x.SpellSlot);
                                }
                            }

                            if (x.SpellSlot == SpellSlot.W)
                            {
                                if (Args.Target.IsMe)
                                {
                                    CastW("Alistar", x.SpellSlot);
                                }
                            }
                            break;
                        case "Blitzcrank":
                            if (x.SpellSlot == SpellSlot.E)
                            {
                                if (Args.Target.IsMe && Args.SData.Name == "PowerFistAttack")
                                {
                                    CastW("Blitzcrank", x.SpellSlot);
                                }
                            }
                            break;
                        case "Chogath":
                            if (x.SpellSlot == SpellSlot.R)
                            {
                                if (Args.Target.IsMe)
                                {
                                    CastW("Chogath", x.SpellSlot);
                                }
                            }
                            break;
                        case "Darius":
                            if (x.SpellSlot == SpellSlot.R)
                            {
                                if (Args.Target.IsMe)
                                {
                                    CastW("Darius", x.SpellSlot);
                                }
                            }
                            break;
                        case "Elise":
                            if (x.SpellSlot == SpellSlot.Q && Args.SData.Name == "EliseHumanQ")
                            {
                                if (Args.Target.IsMe)
                                {
                                    CastW("Elise", x.SpellSlot);
                                }
                            }
                            break;
                        case "Fiddlesticks":
                            if (x.SpellSlot == SpellSlot.Q)
                            {
                                if (Args.Target.IsMe)
                                {
                                    CastW("Fiddlesticks", x.SpellSlot);
                                }
                            }
                            break;
                        case "Gangplank":
                            if (x.SpellSlot == SpellSlot.Q && Args.SData.Name == "Parley")
                            {
                                if (Args.Target.IsMe)
                                {
                                    CastW("Gangplank", x.SpellSlot);
                                }
                            }
                            break;
                        case "Garen":
                            if (x.SpellSlot == SpellSlot.R)
                            {
                                if (Args.Target.IsMe)
                                {
                                    CastW("Garen", x.SpellSlot);
                                }
                            }
                            break;
                        case "Hecarim":
                            if (x.SpellSlot == SpellSlot.E)
                            {
                                if (Args.Target.IsMe && Args.SData.Name == "HecarimRampAttack")
                                {
                                    CastW("Hecarim", x.SpellSlot);
                                }
                            }
                            break;
                        case "Irelia":
                            if (x.SpellSlot == SpellSlot.E)
                            {
                                if (Args.Target.IsMe)
                                {
                                    CastW("Irelia", x.SpellSlot);
                                }
                            }
                            break;
                        case "Jarvan":
                            if (x.SpellSlot == SpellSlot.R)
                            {
                                if (Args.Target.IsMe)
                                {
                                    CastW("Jarvan", x.SpellSlot);
                                }
                            }
                            break;
                        case "Kalista":
                            if (x.SpellSlot == SpellSlot.E)
                            {
                                if (ObjectManager.Player.HasBuff("kalistaexpungemarker") &&
                                    ObjectManager.Player.Distance(target) <= 950f)
                                {
                                    CastW("Kalista", x.SpellSlot);
                                }
                            }
                            break;
                        case "Kayle":
                            if (x.SpellSlot == SpellSlot.Q)
                            {
                                if (Args.Target.IsMe)
                                {
                                    CastW("Kayle", x.SpellSlot);
                                }
                            }
                            break;
                        case "Leesin":
                            if (x.SpellSlot == SpellSlot.R)
                            {
                                if (Args.Target.IsMe)
                                {
                                    CastW("Leesin", x.SpellSlot);
                                }
                            }
                            break;
                        case "Lissandra":
                            if (x.SpellSlot == SpellSlot.R)
                            {
                                if (Args.Target.IsMe)
                                {
                                    CastW("Lissandra", x.SpellSlot);
                                }
                            }
                            break;
                        case "Malzahar":
                            if (x.SpellSlot == SpellSlot.R)
                            {
                                if (Args.Target.IsMe)
                                {
                                    CastW("Mordekaiser", x.SpellSlot);
                                }
                            }
                            break;
                        case "Mordekaiser":
                            if (x.SpellSlot == SpellSlot.Q)
                            {
                                if (Args.Target.IsMe && Args.SData.Name == "MordekaiserQAttack2")
                                {
                                    CastW("Mordekaiser", x.SpellSlot);
                                }
                            }

                            if (x.SpellSlot == SpellSlot.R)
                            {
                                if (Args.Target.IsMe)
                                {
                                    CastW("Mordekaiser", x.SpellSlot);
                                }
                            }
                            break;
                        case "Nasus":
                            if (x.SpellSlot == SpellSlot.W)
                            {
                                if (Args.Target.IsMe)
                                {
                                    CastW("Nasus", x.SpellSlot);
                                }
                            }
                            break;
                        case "Olaf":
                            if (x.SpellSlot == SpellSlot.E)
                            {
                                if (Args.Target.IsMe)
                                {
                                    CastW("Olaf", x.SpellSlot);
                                }
                            }
                            break;
                        case "Pantheon":
                            if (x.SpellSlot == SpellSlot.Q)
                            {
                                if (Args.Target.IsMe)
                                {
                                    CastW("Pantheon", x.SpellSlot);
                                }
                            }

                            if (x.SpellSlot == SpellSlot.W)
                            {
                                if (Args.Target.IsMe)
                                {
                                    CastW("Pantheon", x.SpellSlot);
                                }
                            }
                            break;
                        case "Renekton":
                            if (x.SpellSlot == SpellSlot.W)
                            {
                                if (Args.Target.IsMe)
                                {
                                    CastW("Renekton", x.SpellSlot);
                                }
                            }
                            break;
                        case "Rengar":
                            if (x.SpellSlot == SpellSlot.Q)
                            {
                                if (ObjectManager.Player.Distance(target) <= 300 && Args.Target.IsMe)
                                {
                                    CastW("Rengar", x.SpellSlot);
                                }
                            }
                            break;
                        case "Riven":
                            if (x.SpellSlot == SpellSlot.W)
                            {
                                if (ObjectManager.Player.Position.Distance(target.Position) <=
                                    125f + ObjectManager.Player.BoundingRadius + target.BoundingRadius)
                                {
                                    CastW("Riven", x.SpellSlot);
                                }
                            }
                            break;
                        case "Ryze":
                            if (x.SpellSlot == SpellSlot.W)
                            {
                                if (Args.Target.IsMe)
                                {
                                    CastW("Ryze", x.SpellSlot);
                                }
                            }
                            break;
                        case "Singed":
                            if (x.SpellSlot == SpellSlot.E)
                            {
                                if (Args.Target.IsMe)
                                {
                                    CastW("Singed", x.SpellSlot);
                                }
                            }
                            break;
                        case "Syndra":
                            if (x.SpellSlot == SpellSlot.R)
                            {
                                if (Args.Target.IsMe && Args.SData.Name == "SyndraR")
                                {
                                    CastW("Syndra", x.SpellSlot);
                                }
                            }
                            break;
                        case "TahmKench":
                            if (x.SpellSlot == SpellSlot.W)
                            {
                                if (Args.Target.IsMe)
                                {
                                    CastW("TahmKench", x.SpellSlot);
                                }
                            }
                            break;
                        case "Tristana":
                            if (x.SpellSlot == SpellSlot.R)
                            {
                                if (Args.Target.IsMe && Args.SData.Name == "TristanaR")
                                {
                                    CastW("Tristana", x.SpellSlot);
                                }
                            }
                            break;
                        case "Trundle":
                            if (x.SpellSlot == SpellSlot.R)
                            {
                                if (Args.Target.IsMe)
                                {
                                    CastW("Trundle", x.SpellSlot);
                                }
                            }
                            break;
                        case "TwistedFate":
                            if (Args.SData.Name.Contains("attack") && Args.Target.IsMe &&
                                target.Buffs.Any(
                                    buff =>
                                        buff.Name == "BlueCardAttack" || buff.Name == "GoldCardAttack" ||
                                        buff.Name == "RedCardAttack"))
                            {
                                CastW("TwistedFate", x.SpellSlot);
                            }
                            break;
                        case "Veigar":
                            if (x.SpellSlot == SpellSlot.R)
                            {
                                if (Args.Target.IsMe && Args.SData.Name == "VeigarPrimordialBurst")
                                {
                                    CastW("Veigar", x.SpellSlot);
                                }
                            }
                            break;
                        case "Vi":
                            if (x.SpellSlot == SpellSlot.R)
                            {
                                if (Args.Target.IsMe)
                                {
                                    CastW("Vi", x.SpellSlot);
                                }
                            }
                            break;
                        case "Volibear":
                            if (x.SpellSlot == SpellSlot.Q)
                            {
                                if (Args.Target.IsMe)
                                {
                                    CastW("Volibear", x.SpellSlot);
                                }
                            }

                            if (x.SpellSlot == SpellSlot.W)
                            {
                                if (Args.Target.IsMe)
                                {
                                    CastW("Volibear", x.SpellSlot);
                                }
                            }
                            break;
                        case "Warwick":
                            if (x.SpellSlot == SpellSlot.R)
                            {
                                if (Args.Target.IsMe)
                                {
                                    CastW("Warwick", x.SpellSlot);
                                }
                            }
                            break;
                        case "XinZhao":
                            if (x.SpellSlot == SpellSlot.Q)
                            {
                                if (Args.Target.IsMe && Args.SData.Name == "XenZhaoThrust3")
                                {
                                    CastW("XinZhao", x.SpellSlot);
                                }
                            }
                            break;
                    }

                }
            }
        }

        private static void OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs Args)
        {
            var riven = sender as AIHeroClient;

            if (riven == null || riven.Team == ObjectManager.Player.Team || riven.ChampionName != "Riven" || !riven.IsValid)
            {
                return;
            }

            if (Menu.Item(riven.ChampionName.ToLower() + SpellSlot.Q, true).GetValue<bool>() &&
                Args.Animation.ToLower() == "spell1c")
            {
                RivenQTime = Utils.GameTimeTickCount;
                RivenQRange = riven.HasBuff("RivenFengShuiEngine") ? 225f : 150f;
            }
        }

        private static void OnDash(Obj_AI_Base sender, Dash.DashItem Args)
        {
            var riven = sender as AIHeroClient;

            if (riven == null || riven.Team == ObjectManager.Player.Team || riven.ChampionName != "Riven" || !riven.IsValid)
            {
                return;
            }

            if (Menu.Item(riven.ChampionName.ToLower() + SpellSlot.Q, true).GetValue<bool>())
            {
                RivenDashPos = Args.EndPos;
            }
        }

        private static void CastW()
        {
            if (ObjectManager.Player.IsDead || !ObjectManager.Player.GetSpell(SpellSlot.W).IsReady())
            {
                return;
            }

            var target =
                HeroManager.Enemies.Where(x => !x.IsDead && !x.IsZombie && x.Distance(ObjectManager.Player) <= 750f)
                    .OrderBy(x => x.Distance(ObjectManager.Player))
                    .FirstOrDefault();

            ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W, target?.Position ?? Game.CursorPos);
        }

        private static void CastW(string name, SpellSlot spellslot)
        {
            if (ObjectManager.Player.IsDead || !ObjectManager.Player.GetSpell(SpellSlot.W).IsReady())
            {
                return;
            }

            if (Menu.Item(name.ToLower() + spellslot, true).GetValue<bool>())
            {
                var target =
                    HeroManager.Enemies.Where(x => !x.IsDead && !x.IsZombie && x.Distance(ObjectManager.Player) <= 750f)
                        .OrderBy(x => x.Distance(ObjectManager.Player))
                        .FirstOrDefault();

                ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W, target?.Position ?? Game.CursorPos);
            }
        }
    }
}
