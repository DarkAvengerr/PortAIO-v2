//using EloBuddy; 
 //using LeagueSharp.Common; 
 //namespace YasuoMedia.CommonEx.Algorithm.Media
//{
//    #region Using Directives

//    using System;
//    using System.Collections.Generic;
//    using System.Linq;

//    using Djikstra;

//    using Djikstra.ConnectionTypes;

//    using LeagueSharp;
//    using LeagueSharp.Common;

//    using SharpDX;

//    using Dash = Objects.Dash;
//    using Point = Djikstra.Point;

//    #endregion

//    public class GridGeneratorV2 : IGridGenerator<Djikstra.Point, ConnectionBase<Djikstra.Point>>
//    {
//        #region Fields

//        /// <summary>
//        ///     The dict
//        /// </summary>
//        private Dictionary<int, Dictionary<, int>> dictionary;
        
//        /// <summary>
//        ///     The points
//        /// </summary>
//        private Dictionary<Point, int> points;

//        /// <summary>
//        ///     The grid
//        /// </summary>
//        internal Grid Grid;

//        /// <summary>
//        ///     The units
//        /// </summary>
//        internal List<Obj_AI_Base> Units;

//        /// <summary>
//        /// The deepness
//        /// </summary>
//        internal int deepness = 10;

//        /// <summary>
//        ///     The depth
//        /// </summary>
//        private int depth;

//        #endregion

//        #region Constructors and Destructors

//        // Vector3 from, Vector3 to, int deepness = 10, List<Obj_AI_Base> units = null

//        #endregion

//        #region Public Properties

//        /// <summary>
//        ///     Gets or sets start.
//        /// </summary>
//        /// <value>
//        ///     start.
//        /// </value>
//        public Vector3 From { get; set; }

//        /// <summary>
//        ///     Gets or sets end.
//        /// </summary>
//        /// <value>
//        ///     end.
//        /// </value>
//        public Vector3 To { get; set; }

//        /// <summary>
//        /// The deepness
//        /// </summary>
//        internal int Deepness
//        {
//            get
//            {
//                return this.deepness;
//            }
//            set
//            {
//                if (value > 0)
//                {
//                    this.deepness = value;
//                }
//            }
//        }

//        #endregion

//        #region Public Methods and Operators

//        /// <summary>
//        ///     Generates the grid used for pathfinding.
//        /// </summary>
//        public Grid Generate()
//        {
//            try
//            {
//                this.Initialize();

//                if (this.Units == null || !this.Units.Any())
//                { 
//                    if (GlobalVariables.Debug)
//                    {
//                        Console.WriteLine(@"Returned null");
//                    }

//                    return this.Grid;
//                }

//                var walkingpath = new Dictionary<YasuoConnection<>, int>();

//                var vectorarray = GlobalVariables.Player.GetPath(this.From, this.To);

//                for (var i = 0; i < this.deepness; i++)
//                {
//                    this.depth = i;

//                    if (GlobalVariables.Debug)
//                    {
//                        Console.WriteLine(@"this.depth = " + this.depth);
//                    }

//                    // walking
//                    this.BuildWalkingPath(vectorarray);

//                    // dashes
//                    foreach (var keyvaluepair in this.points.ToList().Where(x => x.Value >= this.depth))
//                    {
//                        this.BuildDashesAroundPoint(keyvaluepair.Key);

//                        //this.BuildWalkingPathAroundPoint(point);
//                    }

//                    this.AddOrUpdate(walkingpath, this.depth);
//                }

//                this.Grid = this.GenerateGrid();

//                return this.Grid;
//            }
//            catch (Exception ex)
//            {
//                if (GlobalVariables.Debug)
//                {
//                    Console.WriteLine(ex);
//                }

//                this.Reset();
//            }

//            return this.Grid;
//        }

//        /// <summary>
//        ///     Updates the specified settings.
//        /// </summary>
//        /// <param name="units">The units.</param>
//        /// <param name="from">start.</param>
//        /// <param name="to">end.</param>
//        public void Update(List<Obj_AI_Base> units, Vector3 @from, Vector3 to)
//        {
//            this.Units = units;
//            this.From = from;
//            this.To = to;
//        }

//        /// <summary>
//        ///     Resets this instance.
//        /// </summary>
//        public void Reset()
//        {
//            this.dictionary = new Dictionary<int, Dictionary<YasuoConnection<>, int>>();

//            this.points = new Dictionary<Point, int>() { { new Point(this.From), 0 } }; 

//            this.Units = new List<Obj_AI_Base>();

//            this.depth = 0;

//            this.Grid = null;
//        }

//        public void Initialize()
//        {
//            this.dictionary = new Dictionary<int, Dictionary<YasuoConnection<>, int>>();

//            this.points = new Dictionary<Point, int>() { { new Point(this.From), 0 } };

//            this.depth = 0;

//            this.Grid = null;
//        }

//        #endregion

//        #region Methods

//        /// <summary>
//        ///     Adds the dict or updates it.
//        /// </summary>
//        /// <param name="dict">The dict.</param>
//        /// <param name="level">The level.</param>
//        private void AddOrUpdate(Dictionary<YasuoConnection<>, int> dict, int level)
//        {
//            if (GlobalVariables.Debug)
//            {
//                Console.WriteLine($"Adding new level to grid: {level}, containing {dict.Count} entries");
//            }

//            foreach (var keyvaluepair in dict)
//            {
//                this.points.Add(keyvaluepair.Key.End, level);
//            }

//            Dictionary<YasuoConnection<>, int> entry;

//            this.dictionary.TryGetValue(level, out entry);

//            if (entry != null && entry.Any())
//            {
//                foreach (var newEntry in dict)
//                {
//                    entry.Add(newEntry.Key, newEntry.Value);
//                }

//                this.dictionary[level] = entry;
//            }
//            else
//            {
//                this.dictionary[level] = dict;
//            }
//        }

//        /// <summary>
//        ///     Backtraces until connection containing that unit was found
//        /// </summary>
//        /// <returns></returns>
//        private List<Obj_AI_Base> Backtrace(Point from)
//        {         
//            var result = new List<Obj_AI_Base>();

//            var prevPoint = from;

//            for (int i = this.depth - 1; i-- > 0;)
//            {
//                Console.WriteLine(@"for loop level: " + i);

//                if (!this.dictionary.ContainsKey(i))
//                {
//                    continue;
//                }

//                foreach (var keyValuePair in this.dictionary[i])
//                {
//                    if (keyValuePair.IsDefault())
//                    {
//                        continue;
//                    }

//                    Console.WriteLine($"level contains: {this.dictionary[i].Count}");

//                    if (keyValuePair.Key.End == prevPoint)
//                    {
//                        result.Add(keyValuePair.Key.Unit);
//                    }
//                }
//            }

//            Console.WriteLine($"Blacklist contains {result.Count} results");

//            return result;
//        }

//        /// <summary>
//        ///     Builds the dashes around some point.
//        /// </summary>
//        /// <param name="point">The point.</param>
//        private void BuildDashesAroundPoint(Point point)
//        {
//            if (GlobalVariables.Debug)
//            {
//                Console.WriteLine(@"Building dashes around some point");
//            }

//            var localDic = new Dictionary<YasuoConnection<>, int>();

//            var validUnits = new List<Obj_AI_Base>();

//            foreach (var unit in
//                    this.Units.Where(
//                        x => x.ServerPosition.Distance(point.Position) <= GlobalVariables.Spells[SpellSlot.E].Range).ToList()
//                )
//            {
//                var blacklist = this.Backtrace(point);

//                if (blacklist.Contains(unit)) continue;

//                validUnits.Add(unit);
//            }

//            foreach (var unit in validUnits.ToList())
//            {
//                Console.WriteLine($"Valid Units: {validUnits.Count}");
//                var dash = new Dash(point.Position, unit);

//                var id = this.depth++;

//                localDic.Add(
//                    new YasuoConnection<>(point, new Point(dash.EndPosition)),
//                    id);
//            }

//            if (GlobalVariables.Debug)
//            {
//                Console.WriteLine($@"Building dashes around point: {localDic.Count} dashes found");
//            }

//            this.AddOrUpdate(localDic, this.depth);
//        }

//        /// <summary>
//        ///     Builds the walking PathBase.
//        /// </summary>
//        private void BuildWalkingPath(Vector3[] path)
//        {
//            var localDic = new Dictionary<YasuoConnection<>, int>();

//            // walking
//            if (this.depth < path.Length - 1)
//            {
//                var point1 = new Point(path[this.depth]);
//                var point2 = new Point(path[this.depth + 1]);

//                localDic.Add(new YasuoConnection<>(point1, point2), this.depth);
//            }

//            this.AddOrUpdate(localDic, this.depth);
//        }

//        // TODO
//        /// <summary>
//        ///     Builds the walking PathBase around some point (unit).
//        ///     Used for walking behind a minion in order end dash around it.
//        /// </summary>
//        /// <param name="point">The point.</param>
//        private void BuildWalkingDashPathAroundPoint(Point point)
//        {
//        }

//        private Grid GenerateGrid()
//        {
//            var list = new List<YasuoConnection<>>();
//            foreach (var entry in this.dictionary)
//            {
//                foreach (var entry2 in entry.Value.Keys)
//                {
//                    list.Add(entry2);
//                }
//            }

//            var grid = new Grid(
//                list,
//                new Point(this.From),
//                new Point(this.To));

//            return grid;
//        }

//        private void OnDraw(EventArgs args)
//        {
//            this.Grid?.Draw(1);
//        }

//        Grid<Point, ConnectionBase<Point>> IGridGenerator<Point, ConnectionBase<Point>>.Generate()
//        {
//            throw new NotImplementedException();
//        }

//        #endregion
//    }

//    public static class ExtensionsGridGenerator
//    {
//        public static bool IsDefault<T>(this T value) where T : struct
//        {
//            bool isDefault = value.Equals(default(T));

//            return isDefault;
//        }
//    }
//}