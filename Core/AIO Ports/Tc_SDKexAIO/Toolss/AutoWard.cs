using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Tc_SDKexAIO.Toolss
{

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;
    using LeagueSharp.SDK.Enumerations;
    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Linq;


    internal class AutoWard
    {
        private Menu Menu;
        public static List<ChampionInfo> ChampionInfoList = new List<ChampionInfo>();
        private Vector3 EnemySpawn = ObjectManager.Get<Obj_SpawnPoint>().FirstOrDefault(x => x.IsEnemy).Position;
        private AIHeroClient Player, Rengar = null, Vayne = null;
        public static List<HiddenObj> HiddenObjList = new List<HiddenObj>();

        private Items.Item VisionWard = new Items.Item(ItemId.Oracle_Alteration, 550f);
        private Items.Item OracleLens = new Items.Item(ItemId.Control_Ward, 550f);
        private Items.Item WardN = new Items.Item(ItemId.Warding_Totem_Trinket, 600f);
        private Items.Item TrinketN = new Items.Item(ItemId.Warding_Totem_Trinket, 600f);
        private Items.Item SightStone = new Items.Item(ItemId.Sightstone, 600f);
        private Items.Item FarsightOrb = new Items.Item(ItemId.Farsight_Alteration, 4000f);
        private Items.Item ScryingOrb = new Items.Item(ItemId.Farsight_Alteration, 3500f);

        private Items.Item Watchers = new Items.Item(2301, 600f);
        private Items.Item Oasis = new Items.Item(2302, 600f);
        private Items.Item Equinox = new Items.Item(2303, 600f);


        public AutoWard(Menu mainMenu)
        {
            this.Menu = mainMenu;

            Player = GameObjects.Player;

            foreach (var hero in ObjectManager.Get<AIHeroClient>())
            {
                if (hero.IsEnemy)
                {
                    ChampionInfoList.Add(new ChampionInfo() { NetworkId = hero.NetworkId, LastVisablePos = hero.Position });
                    if (hero.ChampionName == "Rengar")
                        Rengar = hero;
                    if (hero.ChampionName == "Vayne")
                        Vayne = hero;
                }
            }

            Menu AutoWard = new Menu("AutoWard.Menu", "AutoWard");
            AutoWard.Add(new MenuBool("Enable", "Enable"));
            AutoWard.Add(new MenuBool("BuyBlue", "BuyBlue"));
            AutoWard.Add(new MenuBool("AutoWardCombo", "AutoWardCombo", true));
            AutoWard.Add(new MenuKeyBind("ComboKey", "ComboKey!", System.Windows.Forms.Keys.Space, KeyBindType.Press));

            Menu.Add(AutoWard);

            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
        }

        private void OnCreate(GameObject sender, EventArgs args)
        {
            if (!sender.IsEnemy || sender.IsAlly)
                return;

            var missile = sender as MissileClient;

            if (missile != null)
            {
                if (!missile.SpellCaster.IsVisible)
                    if ((missile.SData.Name == "BantamTrapShort" || missile.SData.Name == "BantamTrapBounceSpell") && !HiddenObjList.Exists(x => missile.EndPosition == x.pos))
                        AddWard("teemorcast", missile.EndPosition);
            }

            var minion = sender as Obj_AI_Minion;

            if (minion != null)
            {
                if ((sender.Name.ToLower() == "visionward" || sender.Name.ToLower() == "sightward") && !HiddenObjList.Exists(x => x.pos.Distance(sender.Position) < 100))
                {
                    foreach (var obj in HiddenObjList)
                    {
                        if (obj.pos.Distance(sender.Position) < 400)
                            if (obj.type == 0)
                            {
                                HiddenObjList.Remove(obj);
                                return;
                            }
                    }

                    var dupa = (Obj_AI_Minion)sender;

                    if (dupa.Mana == 0)
                        HiddenObjList.Add(new HiddenObj() { type = 2, pos = sender.Position, endTime = float.MaxValue });
                    else
                        HiddenObjList.Add(new HiddenObj() { type = 1, pos = sender.Position, endTime = Game.Time + dupa.Mana });
                }
            }
            else if (Rengar != null && sender.Position.Distance(Player.Position) < 800)
            {
                switch (sender.Name)
                {
                    case "Rengar_LeapSound.troy":
                        CastVisionWards(sender.Position);
                        break;
                    case "Rengar_Base_R_Alert":
                        CastVisionWards(sender.Position);
                        break;
                }
            }
        }

        private void OnDelete(GameObject sender, EventArgs args)
        {
            var minion = sender as Obj_AI_Minion;

            if (minion != null && minion.Health < 100)
            {
                foreach (var obj in HiddenObjList)
                {
                    if (obj.pos == sender.Position)
                    {
                        HiddenObjList.Remove(obj);
                        return;
                    }
                    else if (obj.type == 3 && obj.pos.Distance(sender.Position) < 100)
                    {
                        HiddenObjList.Remove(obj);
                        return;
                    }
                    else if (obj.pos.Distance(sender.Position) < 400)
                    {
                        if (obj.type == 2 && sender.Name.ToLower() == "visionward")
                        {
                            HiddenObjList.Remove(obj);
                            return;
                        }
                        else if ((obj.type == 0 || obj.type == 1) && sender.Name.ToLower() == "sightward")
                        {
                            HiddenObjList.Remove(obj);
                            return;
                        }
                    }
                }
            }
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender is AIHeroClient && sender.IsEnemy)
            {
                if (args.Target == null)
                    AddWard(args.SData.Name.ToLower(), args.End);

                if ((OracleLens.IsReady || VisionWard.IsReady) && sender.Distance(Player.Position) < 1200)
                {
                    switch (args.SData.Name.ToLower())
                    {
                        case "akalismokebomb":
                            CastVisionWards(sender.ServerPosition);
                            break;
                        case "deceive":
                            CastVisionWards(sender.ServerPosition);
                            break;
                        case "khazixr":
                            CastVisionWards(sender.ServerPosition);
                            break;
                        case "khazixrlong":
                            CastVisionWards(sender.ServerPosition);
                            break;
                        case "talonshadowassault":
                            CastVisionWards(sender.ServerPosition);
                            break;
                        case "monkeykingdecoy":
                            CastVisionWards(sender.ServerPosition);
                            break;
                        case "rengarr":
                            CastVisionWards(sender.ServerPosition);
                            break;
                        case "twitchhideinshadows":
                            CastVisionWards(sender.ServerPosition);
                            break;
                    }
                }
            }
        }

        private void OnUpdate(EventArgs args)
        {
            foreach (var enemy in GameObjects.EnemyHeroes.Where(enemy => enemy.IsValid))
            {
                var ChampionInfoOne = ChampionInfoList.Find(x => x.NetworkId == enemy.NetworkId);
                if (enemy.IsDead)
                {
                    if (ChampionInfoOne != null)
                    {
                        ChampionInfoOne.NetworkId = enemy.NetworkId;
                        ChampionInfoOne.LastVisablePos = EnemySpawn;
                        ChampionInfoOne.LastVisableTime = Game.Time;
                        ChampionInfoOne.PredictedPos = EnemySpawn;
                    }
                }
                else if (enemy.IsVisible)
                {
                    Vector3 prepos = enemy.Position;

                    if (enemy.IsMoving)
                        prepos = prepos.Extend(enemy.GetWaypoints().Last().ToVector3(), 125);

                    if (ChampionInfoOne == null)
                    {
                        ChampionInfoList.Add(new ChampionInfo() { NetworkId = enemy.NetworkId, LastVisablePos = enemy.Position, LastVisableTime = Game.Time, PredictedPos = prepos });
                    }
                    else
                    {
                        ChampionInfoOne.NetworkId = enemy.NetworkId;
                        ChampionInfoOne.LastVisablePos = enemy.Position;
                        ChampionInfoOne.LastVisableTime = Game.Time;
                        ChampionInfoOne.PredictedPos = prepos;
                    }
                }
            }

            if (!Menu["AutoWard.Menu"]["Enable"].GetValue<MenuBool>())
                return;

            if (Menu["AutoWard.Menu"]["BuyBlue"].GetValue<MenuBool>())
            {
                if (Player.InFountain() && !ScryingOrb.IsOwned() && Player.Level >= 9)
                    Shop.BuyItem(ItemId.Farsight_Alteration);
            }

            if (Rengar != null && Player.HasBuff("rengarralertsound"))
                CastVisionWards(Player.ServerPosition);

            if (Vayne != null && Vayne.IsValidTarget(1000) && Vayne.HasBuff("vaynetumblefade"))
                CastVisionWards(Vayne.ServerPosition);

            foreach (var enemy in GameObjects.EnemyHeroes.Where(enemy => enemy.IsValid && !enemy.IsVisible && !enemy.IsDead))
            {
                var need = ChampionInfoList.Find(x => x.NetworkId == enemy.NetworkId);

                if (need == null || need.PredictedPos == null)
                    continue;

                var PPDistance = need.PredictedPos.Distance(Player.Position);

                if (PPDistance > 1400)
                    continue;

                var timer = Game.Time - need.LastVisableTime;

                if (timer < 4)
                {
                    if (Menu["AutoWard.Menu"]["AutoWardCombo"].GetValue<MenuBool>() && !Menu["AutoWard.Menu"]["ComboKey"].GetValue<MenuKeyBind>().Active)
                        return;

                    if (NavMesh.IsWallOfGrass(need.PredictedPos, 0))
                    {
                        if (PPDistance < 600)
                        {
                            if (TrinketN.IsReady)
                            {
                                TrinketN.Cast(need.PredictedPos);
                                need.LastVisableTime = Game.Time - 5;
                            }
                            else if (SightStone.IsReady)
                            {
                                SightStone.Cast(need.PredictedPos);
                                need.LastVisableTime = Game.Time - 5;
                            }
                            else if (WardN.IsReady)
                            {
                                WardN.Cast(need.PredictedPos);
                                need.LastVisableTime = Game.Time - 5;
                            }
                            else if (Oasis.IsReady)
                            {
                                Oasis.Cast(need.PredictedPos);
                                need.LastVisableTime = Game.Time - 5;
                            }
                            else if (Equinox.IsReady)
                            {
                                Equinox.Cast(need.PredictedPos);
                                need.LastVisableTime = Game.Time - 5;
                            }
                            else if (Watchers.IsReady)
                            {
                                Watchers.Cast(need.PredictedPos);
                                need.LastVisableTime = Game.Time - 5;
                            }
                        }

                        if (FarsightOrb.IsReady)
                        {
                            FarsightOrb.Cast(need.PredictedPos);
                            need.LastVisableTime = Game.Time - 5;
                        }
                        else if (ScryingOrb.IsReady)
                        {
                            ScryingOrb.Cast(need.PredictedPos);
                            need.LastVisableTime = Game.Time - 5;
                        }
                    }
                }
            }
        }

        private void AddWard(string name, Vector3 posCast)
        {
            switch (name)
            {
                case "visionward":
                    HiddenObjList.Add(new HiddenObj() { type = 2, pos = posCast, endTime = float.MaxValue });
                    break;
                case "trinkettotemlvl3B":
                    HiddenObjList.Add(new HiddenObj() { type = 1, pos = posCast, endTime = Game.Time + 180 });
                    break;
                case "itemghostward":
                    HiddenObjList.Add(new HiddenObj() { type = 1, pos = posCast, endTime = Game.Time + 180 });
                    break;
                case "wrigglelantern":
                    HiddenObjList.Add(new HiddenObj() { type = 1, pos = posCast, endTime = Game.Time + 180 });
                    break;
                case "sightward":
                    HiddenObjList.Add(new HiddenObj() { type = 1, pos = posCast, endTime = Game.Time + 180 });
                    break;
                case "itemferalflare":
                    HiddenObjList.Add(new HiddenObj() { type = 1, pos = posCast, endTime = Game.Time + 180 });
                    break;
                case "trinkettotemlvl1":
                    HiddenObjList.Add(new HiddenObj() { type = 1, pos = posCast, endTime = Game.Time + 60 });
                    break;
                case "trinkettotemlvl2":
                    HiddenObjList.Add(new HiddenObj() { type = 1, pos = posCast, endTime = Game.Time + 120 });
                    break;
                case "trinkettotemlvl3":
                    HiddenObjList.Add(new HiddenObj() { type = 1, pos = posCast, endTime = Game.Time + 180 });
                    break;
                case "teemorcast":
                    HiddenObjList.Add(new HiddenObj() { type = 3, pos = posCast, endTime = Game.Time + 300 });
                    break;
                case "noxious trap":
                    HiddenObjList.Add(new HiddenObj() { type = 3, pos = posCast, endTime = Game.Time + 300 });
                    break;
                case "JackInTheBox":
                    HiddenObjList.Add(new HiddenObj() { type = 3, pos = posCast, endTime = Game.Time + 100 });
                    break;
                case "Jack In The Box":
                    HiddenObjList.Add(new HiddenObj() { type = 3, pos = posCast, endTime = Game.Time + 100 });
                    break;
            }
        }

        private void CastVisionWards(Vector3 position)
        {
            if (OracleLens.IsReady)
                OracleLens.Cast(Player.Position.Extend(position, OracleLens.Range));
            else if (VisionWard.IsReady)
                VisionWard.Cast(Player.Position.Extend(position, VisionWard.Range));
        }
    }

    internal class ChampionInfo
    {

        public int NetworkId { get; set; }
        public Vector3 LastVisablePos { get; set; }
        public float LastVisableTime { get; set; }
        public Vector3 PredictedPos { get; set; }
        public float StartRecallTime { get; set; }
        public float AbortRecallTime { get; set; }
        public float FinishRecallTime { get; set; }


        public ChampionInfo()
        {
            LastVisableTime = Game.Time;
            StartRecallTime = 0;
            AbortRecallTime = 0;
            FinishRecallTime = 0;
        }
    }

    internal class HiddenObj
    {
        public int type;
        public float endTime { get; set; }
        public Vector3 pos { get; set; }
    }
}