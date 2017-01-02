#region

using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;


#endregion

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Nechrito_Twitch // Namespace, if we'd put this class in a folder it'd be "Nechrito_Twitch.FOLDER_NAME
{
    internal class Program // No other assemblies can access this class
    {
        private static readonly AIHeroClient Player = ObjectManager.Player; // We're only going to read off of the given API "Player"
         // Loads in HpBarIndicator.cs into Program.cs, which makes us able to make use of it
        private static readonly Dmg dmg = new Dmg();

        /// <summary>
        ///  String with all jungle monsters we're going to use in Jungleclear
        /// </summary>
        private static readonly string[] Monsters =
        {
            "SRU_Red", "SRU_Gromp", "SRU_Krug", "SRU_Razorbeak", "SRU_Murkwolf"
        };

        /// <summary>
        /// Strings with all dragon names, Baron & RiftHerald
        /// </summary>
        private static readonly string[] Dragons =
        {
            "SRU_Dragon_Air", "SRU_Dragon_Fire", "SRU_Dragon_Water", "SRU_Dragon_Earth", "SRU_Dragon_Elder", "SRU_Baron",
            "SRU_RiftHerald"
        };

        // Initializes the loading process
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Twitch") return; // If our champion name isn't Twitch, we will return and we will not load the script

            // Printing chat with our message when starting the loading process
            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Nechrito Twitch</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> Version: 6.18</font></b>");

            MenuConfig.LoadMenu(); // Loads the Menu
            Spells.Initialise();   // Loads the Spells
            Recall(); // Loads our Recall void

            Drawing.OnDraw += OnDraw; // Subscribers
            Drawing.OnEndScene += Drawing_OnEndScene; // With this we can draw health bar and damages

            Game.OnUpdate += Game_OnUpdate; // Initialises our update method
        }

        /// <summary>
        /// Our Update Method
        /// </summary>
        /// <param name="args"></param>
        private static void Game_OnUpdate(EventArgs args)
        {
            AutoE(); // Updates our "AutoE" void 
            SkinChanger(); // Updates skinchanger void
            EDeath();  // Updates EDeath void
            Trinket(); // Updates Trinket void

            switch (MenuConfig.Orbwalker.ActiveMode) // Switch for our current pressed keybind / Mode
            {
                case Orbwalking.OrbwalkingMode.Combo: // If we press the combo keybind
                    Combo(); // Update our combo method
                    break; // Breaks our method when we release the keybind
                case Orbwalking.OrbwalkingMode.Mixed: // If we press our harass keybind
                    Harass(); // Update our harass method
                    break; // Breaks our method when we release the keybind
                case Orbwalking.OrbwalkingMode.LaneClear: // If we press the laneclear keybind
                    LaneClear(); // Update our laneclear method AND Jungleclear method
                    JungleClear();
                    break; // Breaks our method when we release the keybind
            }
        }
       
        // Combo
        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Spells.W.Range, TargetSelector.DamageType.Physical); // Gets a target from Twitch's W spell Range
            if (target == null || !target.IsValidTarget() || target.IsInvulnerable) return; // If the target isn't defined, not a valid target within our range or just Invulnerable, return

            if(target.HealthPercent <= 80)
            {
                Usables.Botrk();
                Usables.CastYoumoo();
            }

            if (MenuConfig.DisableW && (Player.HasBuff("TwitchUlt") || Player.HasBuff("TwitchFullAutomatic")))
            {
                return;
            }

            if (!MenuConfig.UseW  || target.Health < Player.GetAutoAttackDamage(target, true)*2)
            {
                return; // If our Combo W Menu is "Off" or targets health is more than 2 AA, return. 
            }

            var wPred = Spells.W.GetPrediction(target).CastPosition; // Twitch's W prediction for our Combo

            if (Spells.W.IsReady()) // if Twitch's W spell is ready
            {
                Spells.W.Cast(wPred); // Cast to the given prediction
            }
        }

        // Harass
        private static void Harass()
        {
            var target = TargetSelector.GetTarget(Spells.E.Range, TargetSelector.DamageType.Physical); // Searches for targets within Twitch's E spell range
            if (target == null || !target.IsValidTarget(Spells.E.Range)) return; // If the target isn't defined, not a valid target within our range or just Invulnerable, return

            // If our target isn't in AA range and the target has the amount of E stacks we gave it in our Menu & Twitch's mana is above 49% & Twitch's E is ready
            if (!Orbwalking.InAutoAttackRange(target) && target.GetBuffCount("twitchdeadlyvenom") >= MenuConfig.ESlider && Player.ManaPercent >= 50 && Spells.E.IsReady())
            {
                Spells.E.Cast(); // Cast Twitch's E spell
            }

            if (!MenuConfig.HarassW || !Spells.W.IsReady()) return; // If Harass => Use W is "Off", return

            if (target.IsValidTarget(Spells.W.Range)) // if Twitch's target is valid within W range & Twitch's W spell is ready
            {
                Spells.W.Cast(target.Position); // Cast Twitch's W spell to target's position
            }
        }

        // Laneclear
        private static void LaneClear()
        {
            var minions = MinionManager.GetMinions(Player.ServerPosition, Spells.W.Range); // searches for minions within 800 units

            if (minions == null) return; // If there aren't any | they aren't defined, return

            var wPrediction = Spells.W.GetCircularFarmLocation(minions); // Twitch's W spell prediction for minions

            if (!MenuConfig.LaneW) return; // If our Lane => Use W is "Off" return

            if (!Spells.W.IsReady()) return; // if Twitch's W spell isn't ready, return

            if (wPrediction.MinionsHit >= 4) // If the prediction can hit 4 or more minions
            {
                Spells.W.Cast(wPrediction.Position); // Cast Twitch's W spell to the given prediction
            }
        }

        // Jungle
        private static void JungleClear()
        {
            if (Player.Level == 1) return; // If our player level is 1, return. This is so we can prevent stealing mobs from our jungler

            // Gets jungle mobs within Twitch's W spell range, prioritizes the max health mob. 
            var mobs = MinionManager.GetMinions(Player.Position, Spells.W.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            var wPrediction = Spells.W.GetCircularFarmLocation(mobs); // Twitch's prediction for casting W in jungle

            if (mobs.Count == 0) return; // If there aren't any mobs, return (This isn't really neccessary) 

            if (MenuConfig.JungleW && Spells.W.IsReady()) // If Jungle => Use W is "On" do the following code within the brackets
            {
                if (wPrediction.MinionsHit >= 3 && Player.ManaPercent >= 10) // If prediction can hit 3 or more mobs & Twitch's mana is above 19%
                {
                    Spells.W.Cast(wPrediction.Position); // Cast Twitch's W spell to the given prediction
                }
            }

            if(!MenuConfig.JungleE || !Spells.E.IsReady()) return; // If Jungle => Use E is "Off", return

            foreach (var m in ObjectManager.Get<Obj_AI_Base>().Where(x => Monsters.Contains(x.CharData.BaseSkinName))) // Looks for mobs that isn't dead
            {
                if (m.Health < Spells.E.GetDamage(m)) // if the mobs health is less than Twitch's E spell
                {
                    Spells.E.Cast(); // Execute mob with Twitch's E spell
                }
            }
        }

        // Recall
        private static void Recall()
        {
            Spellbook.OnCastSpell += (sender, eventArgs) => // Subscribes to the event(?)
            {
                if (!Spells.Q.IsReady()
                || !SpellSlot.Recall.IsReady()
                || !MenuConfig.QRecall
                || eventArgs.Slot != SpellSlot.Recall) return; // If Twitch's Q isn't ready or Recall isn't ready etc. return

                Spells.Q.Cast(); // Cast Twitch's Q spell

                // Delays the action with Q Delay + 0.3 seconds, then cast Recall
                LeagueSharp.Common.Utility.DelayAction.Add((int) Spells.Q.Delay + 300 + Game.Ping/2, () => ObjectManager.Player.Spellbook.CastSpell(SpellSlot.Recall));
                eventArgs.Process = false; // It's as return or bool, the code has been executed 
            };
        }

        // Auto E 
        private static void AutoE()
        {
            if (!Spells.E.IsReady())
            {
                return;
            }

            if (MenuConfig.KsE) // If Menu => Combo => Killsecure E is "On", do the following code
            {
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsValidTarget(Spells.E.Range) && enemy.Health <= dmg.GetDamage(enemy)))
                {
                    Spells.E.Cast(enemy); // Executes enemy with Twitch's E spell
                }
            }

            // Looks for mobs within Twitch's E spell range, prioritizes the mob with most health
            var mob = MinionManager.GetMinions(Spells.E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            // If Menu => Steal => Steal Dragon & Baron is "On" execute the following code within the brackets
            if (MenuConfig.StealEpic)
            {
                // Searches through our list with "Dragons"
                foreach (var m in ObjectManager.Get<Obj_AI_Base>().Where(x => Dragons.Contains(x.CharData.BaseSkinName)))
                {
                    if (m.Health <= dmg.GetDamage(m)) // If monster is found and targets health is less than Twitch's E spell
                    {
                        Spells.E.Cast(m); // Execute with Twitch's E spell
                    }
                }
            }

            // If Menu => Steal => Steal Redbuff, do the following code within the brackets
            if (!MenuConfig.StealBuff) return;

            foreach (var m in mob)
            {
                // If base skin name is SRU_Red (Redbuff)
                if (m.CharData.BaseSkinName.Contains("SRU_Red")) continue;

                if (m.Health <= dmg.GetDamage(m)) // If Redbuff is killable
                {
                    Spells.E.Cast(); // Kill Redbuff with Twitch's E spell
                }
            }
        }
       
        private static void SkinChanger()
        {
            //Player.SetSkin(Player.CharData.BaseSkinName, MenuConfig.UseSkin ? MenuConfig.Skin.SelectedIndex : Player.SkinId);
        }

        private static void EDeath()
        {
            if (!MenuConfig.EOnDeath || !Spells.E.IsReady() || Player.HealthPercent > 8) return;

            var target = ObjectManager.Get<AIHeroClient>().FirstOrDefault(x => x.IsValidTarget(Spells.E.Range) && x.HasBuff("twitchdeadlyvenom"));

            if (target == null)
            {
                return;
            }

            Spells.E.Cast(); // Executes enemy with Twitch's E spell    
        }

        public static void Trinket()
        {
            if (Player.Level < 9
                || !Player.InShop()
                || !MenuConfig.BuyTrinket
                || Items.HasItem(3363)
                || Items.HasItem(3364))
            {
                return;
            }
            
            switch (MenuConfig.TrinketList.SelectedIndex)
            {
                case 0:
                    Shop.BuyItem(ItemId.Oracle_Alteration);
                    break;
                case 1:
                    Shop.BuyItem(ItemId.Farsight_Alteration);
                    break;
            }
        }

        public static void OnDraw(EventArgs args)
        {
            // This variable checks wether or not we are in stealth by Twitch's Q
            var hasPassive = Player.HasBuff("TwitchHideInShadows");

            if (!hasPassive || !MenuConfig.DrawQ) return; // If we don't have the passive, go back.

            var passiveTime = Math.Max(0, Player.GetBuff("TwitchHideInShadows").EndTime) - Game.Time; // Checks for Q Passive time - Game Time

            Render.Circle.DrawCircle(Player.Position, passiveTime * Player.MoveSpeed, System.Drawing.Color.Gray); // Renders a circle on Player's Position times Movement speed in a gray circle
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
        }
    }
}