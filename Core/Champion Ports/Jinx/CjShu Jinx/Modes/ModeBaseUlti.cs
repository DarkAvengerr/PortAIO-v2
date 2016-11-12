using EloBuddy; 
 using LeagueSharp.Common; 
 namespace CjShuJinx.Modes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using SharpDX.Direct3D9;
    using Collision = LeagueSharp.Common.Collision;

    internal class ModeBaseUlti
    {
        static Menu MenuLocal;
        static Menu TeamUlt;
        static Menu DisabledChampions;

        static Spell Ultimate;
        static int LastUltCastT;

        static  LeagueSharp.Common.Utility.Map.MapType Map;

        static List<AIHeroClient> Heroes;
        static List<AIHeroClient> Enemies;
        static List<AIHeroClient> Allies;

        public static List<EnemyInfo> EnemyInfo = new List<EnemyInfo>();

        public static Dictionary<int, int> RecallT = new Dictionary<int, int>();

        static Vector3 EnemySpawnPos;

        static  Font Text;

        static System.Drawing.Color NotificationColor = System.Drawing.Color.FromArgb(136, 207, 240);

        static float BarX = Drawing.Width*0.425f;
        static float BarY = Drawing.Height*0.80f;
        static int BarWidth = (int) (Drawing.Width - 2*BarX);
        static int BarHeight = 6;
        static int SeperatorHeight = 5;
        static float Scale = (float) BarWidth/8000;

        public static void Init(Menu ParentMenu)
        {
            MenuLocal = new Menu("Base Ulti", "BaseUlt3");
            MenuLocal.AddItem(new MenuItem("showRecalls", "Show Recalls").SetValue(true));
            MenuLocal.AddItem(new MenuItem("baseUlt", "Base Ult").SetValue(true));
            MenuLocal.AddItem(new MenuItem("checkCollision", "Check Collision").SetValue(true));
            MenuLocal.AddItem(new MenuItem("panicKey", "No Ult while SBTW").SetValue(new KeyBind(32, KeyBindType.Press)));
                // 32 == space
            MenuLocal.AddItem(
                new MenuItem("regardlessKey", "No timelimit (hold)").SetValue(new KeyBind(17, KeyBindType.Press)));
                // 17 == ctrl
            ParentMenu.AddSubMenu(MenuLocal);
            Heroes = ObjectManager.Get<AIHeroClient>().ToList();
            Enemies = Heroes.Where(x => x.IsEnemy).ToList();
            Allies = Heroes.Where(x => x.IsAlly).ToList();

            EnemyInfo = Enemies.Select(x => new EnemyInfo(x)).ToList();

            bool compatibleChamp = IsCompatibleChamp(ObjectManager.Player.ChampionName);

            TeamUlt = MenuLocal.AddSubMenu(new Menu("Team Baseult Friends", "TeamUlt"));
            DisabledChampions = MenuLocal.AddSubMenu(new Menu("Disabled Champion targets", "DisabledChampions"));

            if (compatibleChamp)
            {
                foreach (AIHeroClient champ in Allies.Where(x => !x.IsMe && IsCompatibleChamp(x.ChampionName)))
                    TeamUlt.AddItem(
                        new MenuItem(champ.ChampionName, "Ally with baseult: " + champ.ChampionName).SetValue(false)
                            .DontSave());

                foreach (AIHeroClient champ in Enemies)
                    DisabledChampions.AddItem(
                        new MenuItem(champ.ChampionName, "Don't shoot: " + champ.ChampionName).SetValue(false)
                            .DontSave());
            }

            var NotificationsMenu = MenuLocal.AddSubMenu(new Menu("Notifications", "Notifications"));

            NotificationsMenu.AddItem(new MenuItem("notifRecFinished", "Recall finished").SetValue(true));
            NotificationsMenu.AddItem(new MenuItem("notifRecAborted", "Recall aborted").SetValue(true));

            var objSpawnPoint = ObjectManager.Get<Obj_SpawnPoint>().FirstOrDefault(x => x.IsEnemy);
            if (objSpawnPoint != null)
            {
                EnemySpawnPos = objSpawnPoint.Position;
            }
            // ObjectManager.Get<GameObject>().FirstOrDefault(x => x.Type == GameObjectType.obj_SpawnPoint && x.IsEnemy).Position;

            Map = LeagueSharp.Common.Utility.Map.GetMap().Type;

            Ultimate = new Spell(SpellSlot.R);

            Text = new Font(Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Calibri",
                    Height = 13,
                    Width = 6,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.Default
                });

            Obj_AI_Base.OnTeleport += Obj_AI_Base_OnTeleport;
            Drawing.OnPreReset += Drawing_OnPreReset;
            Drawing.OnPostReset += Drawing_OnPostReset;
            Drawing.OnDraw += Drawing_OnDraw;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_DomainUnload;

            if (compatibleChamp)
            {
                Game.OnUpdate += Game_OnUpdate;
            }
        }

        static void ShowNotification(string message, System.Drawing.Color color, int duration = -1, bool dispose = true)
        {
            Notifications.AddNotification(new Notification(message, duration, dispose).SetTextColor(color));
        }

        static bool IsCompatibleChamp(String championName)
        {
            return UltSpellData.Keys.Any(x => x == championName);
        }

        static void Game_OnUpdate(EventArgs args)
        {
            int time = Utils.TickCount;

            foreach (EnemyInfo enemyInfo in EnemyInfo.Where(x => x.Player.IsVisible))
                enemyInfo.LastSeen = time;

            if (!MenuLocal.Item("baseUlt").GetValue<bool>())
                return;

            foreach (EnemyInfo enemyInfo in EnemyInfo.Where(x =>
                x.Player.IsValid<AIHeroClient>() &&
                !x.Player.IsDead &&
                !DisabledChampions.Item(x.Player.ChampionName).GetValue<bool>() &&
                #pragma warning disable 618
                x.RecallInfo.Recall.Status == Packet.S2C.Teleport.Status.Start &&
                #pragma warning restore 618
                #pragma warning disable 618
                x.RecallInfo.Recall.Type == Packet.S2C.Teleport.Type.Recall)
                #pragma warning restore 618
                .OrderBy(x => x.RecallInfo.GetRecallCountdown()))
            {
                if (Utils.TickCount - LastUltCastT > 15000)
                    HandleUltTarget(enemyInfo);
            }
        }

        struct UltSpellDataS
        {
            public int SpellStage;
            public float DamageMultiplicator;
            public float Width;
            public float Delay;
            public float Speed;
            public bool Collision;
        }

        static Dictionary<String, UltSpellDataS> UltSpellData = new Dictionary<string, UltSpellDataS>
        {
            {
                "Jinx",
                new UltSpellDataS
                {
                    SpellStage = 1,
                    DamageMultiplicator = 1.0f,
                    Width = 140f,
                    Delay = 0600f/1000f,
                    Speed = 1700f,
                    Collision = true
                }
            },
            {
                "Ashe",
                new UltSpellDataS
                {
                    SpellStage = 0,
                    DamageMultiplicator = 1.0f,
                    Width = 130f,
                    Delay = 0250f/1000f,
                    Speed = 1600f,
                    Collision = true
                }
            },
            {
                "Draven",
                new UltSpellDataS
                {
                    SpellStage = 0,
                    DamageMultiplicator = 0.7f,
                    Width = 160f,
                    Delay = 0400f/1000f,
                    Speed = 2000f,
                    Collision = true
                }
            },
            {
                "Ezreal",
                new UltSpellDataS
                {
                    SpellStage = 0,
                    DamageMultiplicator = 0.7f,
                    Width = 160f,
                    Delay = 1000f/1000f,
                    Speed = 2000f,
                    Collision = false
                }
            },
            {
                "Karthus",
                new UltSpellDataS
                {
                    SpellStage = 0,
                    DamageMultiplicator = 1.0f,
                    Width = 000f,
                    Delay = 3125f/1000f,
                    Speed = 0000f,
                    Collision = false
                }
            }
        };

        static bool CanUseUlt(AIHeroClient hero) // use for allies when fixed: champ.Spellbook.GetSpell(SpellSlot.R) = Ready
        {
            return hero.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready ||
                   (hero.Spellbook.GetSpell(SpellSlot.R).Level > 0 &&
                    hero.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Surpressed &&
                    hero.Mana >= hero.Spellbook.GetSpell(SpellSlot.R).SData.Mana);
        }

        static void HandleUltTarget(EnemyInfo enemyInfo)
        {
            bool ultNow = false;
            bool me = false;

            foreach (
                AIHeroClient champ in
                    Allies.Where(x => // gathering the damage from allies should probably be done once only with timers
                        x.IsValid<AIHeroClient>() &&
                        !x.IsDead &&
                        ((x.IsMe && !x.IsStunned) ||
                         TeamUlt.Items.Any(item => item.GetValue<bool>() && item.Name == x.ChampionName)) &&
                        CanUseUlt(x)))
            {
                if (MenuLocal.Item("checkCollision").GetValue<bool>() && UltSpellData[champ.ChampionName].Collision &&
                    IsCollidingWithChamps(champ, EnemySpawnPos, UltSpellData[champ.ChampionName].Width))
                {
                    enemyInfo.RecallInfo.IncomingDamage[champ.NetworkId] = 0;
                    continue;
                }

                // increase timeneeded if it should arrive earlier, decrease if later
                var timeneeded =
                    GetUltTravelTime(champ, UltSpellData[champ.ChampionName].Speed,
                        UltSpellData[champ.ChampionName].Delay, EnemySpawnPos) - 65;

                if (enemyInfo.RecallInfo.GetRecallCountdown() >= timeneeded)
                    enemyInfo.RecallInfo.IncomingDamage[champ.NetworkId] =
                        (float)
                            champ.GetSpellDamage(enemyInfo.Player, SpellSlot.R,
                                UltSpellData[champ.ChampionName].SpellStage)*
                        UltSpellData[champ.ChampionName].DamageMultiplicator;
                else if (enemyInfo.RecallInfo.GetRecallCountdown() < timeneeded - (champ.IsMe ? 0 : 125))
                    // some buffer for allies so their damage isnt getting reset
                {
                    enemyInfo.RecallInfo.IncomingDamage[champ.NetworkId] = 0;
                    continue;
                }

                if (champ.IsMe)
                {
                    me = true;

                    enemyInfo.RecallInfo.EstimatedShootT = timeneeded;

                    if (enemyInfo.RecallInfo.GetRecallCountdown() - timeneeded < 60)
                        ultNow = true;
                }
            }

            if (me)
            {
                if (!IsTargetKillable(enemyInfo))
                {
                    enemyInfo.RecallInfo.LockedTarget = false;
                    return;
                }

                enemyInfo.RecallInfo.LockedTarget = true;

                if (!ultNow || MenuLocal.Item("panicKey").GetValue<KeyBind>().Active)
                    return;

                Ultimate.Cast(EnemySpawnPos, true);
                LastUltCastT = Utils.TickCount;
            }
            else
            {
                enemyInfo.RecallInfo.LockedTarget = false;
                enemyInfo.RecallInfo.EstimatedShootT = 0;
            }
        }

        static bool IsTargetKillable(EnemyInfo enemyInfo)
        {
            float totalUltDamage = enemyInfo.RecallInfo.IncomingDamage.Values.Sum();

            float targetHealth = GetTargetHealth(enemyInfo, enemyInfo.RecallInfo.GetRecallCountdown());

            if (Utils.TickCount - enemyInfo.LastSeen > 20000 &&
                !MenuLocal.Item("regardlessKey").GetValue<KeyBind>().Active)
            {
                if (totalUltDamage < enemyInfo.Player.MaxHealth)
                    return false;
            }
            else if (totalUltDamage < targetHealth)
                return false;

            return true;
        }

        static float GetTargetHealth(EnemyInfo enemyInfo, int additionalTime)
        {
            if (enemyInfo.Player.IsVisible)
                return enemyInfo.Player.Health;

            float predictedHealth = enemyInfo.Player.Health +
                                    enemyInfo.Player.HPRegenRate*
                                    ((Utils.TickCount - enemyInfo.LastSeen + additionalTime)/1000f);

            return predictedHealth > enemyInfo.Player.MaxHealth ? enemyInfo.Player.MaxHealth : predictedHealth;
        }

        static float GetUltTravelTime(AIHeroClient source, float speed, float delay, Vector3 targetpos)
        {
            if (source.ChampionName == "Karthus")
                return delay*1000;

            float distance = Vector3.Distance(source.ServerPosition, targetpos);

            float missilespeed = speed;

            if (source.ChampionName == "Jinx" && distance > 1350)
            {
                const float accelerationrate = 0.3f; // = (1500f - 1350f) / (2200 - speed), 1 unit = 0.3units/second

                var acceldifference = distance - 1350f;

                if (acceldifference > 150f) // it only accelerates 150 units
                    acceldifference = 150f;

                var difference = distance - 1500f;

                missilespeed = (1350f*speed + acceldifference*(speed + accelerationrate*acceldifference) +
                                difference*2200f)/distance;
            }

            return (distance/missilespeed + delay)*1000;
        }

        static bool IsCollidingWithChamps(AIHeroClient source, Vector3 targetpos, float width)
        {
            var input = new PredictionInput
            {
                Radius = width,
                Unit = source,
            };

            input.CollisionObjects[0] = CollisionableObjects.Heroes;

            return Collision.GetCollision(new List<Vector3> {targetpos}, input).Any();
                // x => x.NetworkId != targetnetid, hard to realize with teamult
        }

        static void Obj_AI_Base_OnTeleport(GameObject sender, GameObjectTeleportEventArgs args)
        {
            var unit = sender as AIHeroClient;

            if (unit == null || !unit.IsValid || unit.IsAlly)
            {
                return;
            }

            #pragma warning disable 618
            var recall = Packet.S2C.Teleport.Decoded(unit, args);
            #pragma warning restore 618
            var enemyInfo =
                EnemyInfo.Find(x => x.Player.NetworkId == recall.UnitNetworkId).RecallInfo.UpdateRecall(recall);

            #pragma warning disable 618
            if (recall.Type == Packet.S2C.Teleport.Type.Recall)
            #pragma warning restore 618
            {
                switch (recall.Status)
                {
            #pragma warning disable 618
                    case Packet.S2C.Teleport.Status.Abort:
            #pragma warning restore 618
                        if (MenuLocal.Item("notifRecAborted").GetValue<bool>())
                        {
                            ShowNotification(enemyInfo.Player.ChampionName + ": Recall ABORTED",
                                System.Drawing.Color.Orange, 4000);
                        }

                        break;
            #pragma warning disable 618
                    case Packet.S2C.Teleport.Status.Finish:
            #pragma warning restore 618
                        if (MenuLocal.Item("notifRecFinished").GetValue<bool>())
                        {
                            ShowNotification(enemyInfo.Player.ChampionName + ": Recall FINISHED", NotificationColor,
                                4000);
                        }

                        break;
                }
            }
        }

        static void Drawing_OnPostReset(EventArgs args)
        {
            Text.OnResetDevice();
        }

        static void Drawing_OnPreReset(EventArgs args)
        {
            Text.OnLostDevice();
        }

        static void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            Text.Dispose();
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (!MenuLocal.Item("showRecalls").GetValue<bool>() || Drawing.Direct3DDevice == null ||
                Drawing.Direct3DDevice.IsDisposed)
                return;

            bool indicated = false;

            float fadeout = 1f;
            int count = 0;

            foreach (EnemyInfo enemyInfo in EnemyInfo.Where(x =>
                x.Player.IsValid<AIHeroClient>() &&
                x.RecallInfo.ShouldDraw() &&
                !x.Player.IsDead && // maybe redundant
                x.RecallInfo.GetRecallCountdown() > 0).OrderBy(x => x.RecallInfo.GetRecallCountdown()))
            {
                if (!enemyInfo.RecallInfo.LockedTarget)
                {
                    fadeout = 1f;
                    System.Drawing.Color color = System.Drawing.Color.White;

                    if (enemyInfo.RecallInfo.WasAborted())
                    {
                        fadeout = (float) enemyInfo.RecallInfo.GetDrawTime()/(float) enemyInfo.RecallInfo.FADEOUT_TIME;
                        color = System.Drawing.Color.Yellow;
                    }

                    DrawRect(BarX, BarY, (int) (Scale*(float) enemyInfo.RecallInfo.GetRecallCountdown()), BarHeight, 1,
                        System.Drawing.Color.FromArgb((int) (100f*fadeout), System.Drawing.Color.White));
                    DrawRect(BarX + Scale*(float) enemyInfo.RecallInfo.GetRecallCountdown() - 1, BarY - SeperatorHeight,
                        0, SeperatorHeight + 1, 1, System.Drawing.Color.FromArgb((int) (255f*fadeout), color));

                    Text.DrawText(null, enemyInfo.Player.ChampionName,
                        (int) BarX +
                        (int)
                            (Scale*(float) enemyInfo.RecallInfo.GetRecallCountdown() -
                             (float) (enemyInfo.Player.ChampionName.Length*Text.Description.Width)/2),
                        (int) BarY - SeperatorHeight - Text.Description.Height - 1,
                        new ColorBGRA(color.R, color.G, color.B, (byte) ((float) color.A*fadeout)));
                }
                else
                {
                    if (!indicated && enemyInfo.RecallInfo.EstimatedShootT != 0)
                    {
                        indicated = true;
                        DrawRect(BarX + Scale*enemyInfo.RecallInfo.EstimatedShootT,
                            BarY + SeperatorHeight + BarHeight - 3, 0, SeperatorHeight*2, 2, System.Drawing.Color.Orange);
                    }

                    DrawRect(BarX, BarY, (int) (Scale*(float) enemyInfo.RecallInfo.GetRecallCountdown()), BarHeight, 1,
                        System.Drawing.Color.FromArgb(255, System.Drawing.Color.Red));
                    DrawRect(BarX + Scale*(float) enemyInfo.RecallInfo.GetRecallCountdown() - 1,
                        BarY + SeperatorHeight + BarHeight - 3, 0, SeperatorHeight + 1, 1,
                        System.Drawing.Color.IndianRed);

                    Text.DrawText(null, enemyInfo.Player.ChampionName,
                        (int) BarX +
                        (int)
                            (Scale*(float) enemyInfo.RecallInfo.GetRecallCountdown() -
                             (float) (enemyInfo.Player.ChampionName.Length*Text.Description.Width)/2),
                        (int) BarY + SeperatorHeight + Text.Description.Height/2, new ColorBGRA(255, 92, 92, 255));
                }

                count++;
            }

            /*
             * Show in a red rectangle right next to the normal bar the names of champs which can be killed (when they are not recalling yet)
             * Requires calculating the damages (make more functions!)
             * 
             * var BaseUltableEnemies = EnemyInfo.Where(x =>
                x.Player.IsValid<AIHeroClient>() &&
                !x.RecallInfo.ShouldDraw() &&
                !x.Player.IsDead && //maybe redundant
                x.RecallInfo.GetRecallCountdown() > 0 && x.RecallInfo.LockedTarget).OrderBy(x => x.RecallInfo.GetRecallCountdown());*/

            if (count > 0)
            {
                if (count != 1) // make the whole bar fadeout when its only 1
                    fadeout = 1f;

                DrawRect(BarX, BarY, BarWidth, BarHeight, 1,
                    System.Drawing.Color.FromArgb((int) (40f*fadeout), System.Drawing.Color.White));

                DrawRect(BarX - 1, BarY + 1, 0, BarHeight, 1,
                    System.Drawing.Color.FromArgb((int) (255f*fadeout), System.Drawing.Color.White));
                DrawRect(BarX - 1, BarY - 1, BarWidth + 2, 1, 1,
                    System.Drawing.Color.FromArgb((int) (255f*fadeout), System.Drawing.Color.White));
                DrawRect(BarX - 1, BarY + BarHeight, BarWidth + 2, 1, 1,
                    System.Drawing.Color.FromArgb((int) (255f*fadeout), System.Drawing.Color.White));
                DrawRect(BarX + 1 + BarWidth, BarY + 1, 0, BarHeight, 1,
                    System.Drawing.Color.FromArgb((int) (255f*fadeout), System.Drawing.Color.White));
            }
        }

        static public void DrawRect(float x, float y, int width, int height, float thickness, System.Drawing.Color color)
        {
            for (int i = 0; i < height; i++)
                Drawing.DrawLine(x, y + i, x + width, y + i, thickness, color);
        }
    }

    class EnemyInfo
    {
        public AIHeroClient Player;
        public int LastSeen;

        public RecallInfo RecallInfo;

        public EnemyInfo(AIHeroClient player)
        {
            Player = player;
            RecallInfo = new RecallInfo(this);
        }
    }

    class RecallInfo
    {
        public EnemyInfo EnemyInfo;
        public Dictionary<int, float> IncomingDamage; //from, damage
        #pragma warning disable 618
        public Packet.S2C.Teleport.Struct Recall;
        public Packet.S2C.Teleport.Struct AbortedRecall;
        #pragma warning restore 618
        public bool LockedTarget;
        public float EstimatedShootT;
        public int AbortedT;
        public int FADEOUT_TIME = 3000;

        public RecallInfo(EnemyInfo enemyInfo)
        {
            EnemyInfo = enemyInfo;
            #pragma warning disable 618
            Recall = new Packet.S2C.Teleport.Struct(EnemyInfo.Player.NetworkId, Packet.S2C.Teleport.Status.Unknown, Packet.S2C.Teleport.Type.Unknown, 0);
            #pragma warning restore 618
            IncomingDamage = new Dictionary<int, float>();
        }

        public bool ShouldDraw()
        {
            return IsPorting() || (WasAborted() && GetDrawTime() > 0);
        }

        public bool IsPorting()
        {
        #pragma warning disable 618
            return Recall.Type == Packet.S2C.Teleport.Type.Recall && Recall.Status == Packet.S2C.Teleport.Status.Start;
        #pragma warning restore 618
        }

        public bool WasAborted()
        {
        #pragma warning disable 618
            return Recall.Type == Packet.S2C.Teleport.Type.Recall && Recall.Status == Packet.S2C.Teleport.Status.Abort;
        #pragma warning restore 618
        }

        #pragma warning disable 618
        public EnemyInfo UpdateRecall(Packet.S2C.Teleport.Struct newRecall)
        #pragma warning restore 618
        {
            IncomingDamage.Clear();
            LockedTarget = false;
            EstimatedShootT = 0;

        #pragma warning disable 618
            if (newRecall.Type == Packet.S2C.Teleport.Type.Recall && newRecall.Status == Packet.S2C.Teleport.Status.Abort)
        #pragma warning restore 618
            {
                AbortedRecall = Recall;
                AbortedT = Utils.TickCount;
            }
            else
                AbortedT = 0;

            Recall = newRecall;
            return EnemyInfo;
        }

        public int GetDrawTime()
        {
            int drawtime = 0;

            if (WasAborted())
                drawtime = FADEOUT_TIME - (Utils.TickCount - AbortedT);
            else
                drawtime = GetRecallCountdown();

            return drawtime < 0 ? 0 : drawtime;
        }

        public int GetRecallCountdown()
        {
            int time = Utils.TickCount;
            int countdown = 0;

            if (time - AbortedT < FADEOUT_TIME)
                countdown = AbortedRecall.Duration - (AbortedT - AbortedRecall.Start);
            else if (AbortedT > 0)
                countdown = 0; //AbortedT = 0
            else
                countdown = Recall.Start + Recall.Duration - time;

            return countdown < 0 ? 0 : countdown;
        }

        public override string ToString()
        {
            String drawtext = EnemyInfo.Player.ChampionName + ": " + Recall.Status;
                // change to better string and colored

            float countdown = GetRecallCountdown()/1000f;

            if (countdown > 0)
                drawtext += " (" + countdown.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + "s)";

            return drawtext;
        }
    }
}