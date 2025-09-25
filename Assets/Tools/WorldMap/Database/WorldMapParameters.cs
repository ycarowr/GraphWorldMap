using System;
using Game;
using TMPro;
using Tools.Attributes;
using Tools.WorldMapCore.Runtime;
using UnityEngine;

namespace Tools.WorldMapCore.Database
{
    [CreateAssetMenu(menuName = "Database/WorldMap/Parameters")]
    public class WorldMapParameters : ScriptableObject
    {
        public enum OrientationGraph
        {
            None = 0,
            LeftRight = 1,
            BottomTop = 2,
        }

        public enum SortMethod
        {
            None = 0,
            Axis = 1,
            Distance = 2,
        }

        public const float SMALL_NUMBER = 0.0001f;

        [SerializeField] [Tooltip("Total amount of nodes that will be created.")]
        private int amount = 32;

        [SerializeField] [Tooltip("Node size in world units from each node.")]
        private Vector2 nodeWorldSize = Vector2.one;

        [SerializeField] [Tooltip("Total world map size in world units.")]
        private Vector2 totalWorldSize = new(90, 60);

        [SerializeField] [Tooltip("Maximum amount of attempts to generate a map with the provided parameters.")]
        private int iterations = 65535;

        [SerializeField] [Tooltip("Maximum amount of attempts to generate a map with the provided parameters.")]
        private int parallelIterations = 100;

        [SerializeField] [Tooltip("Timeout for parallel iterations in Seconds.")]
        private int timeout = 60;

        [SerializeField] [Tooltip("Seed value used for generation of the map.")]
        private int seed;

        [SerializeField] [Tooltip("Will the seed be used for generation of the map.")]
        private bool hasRandomSeed = true;

        [SerializeField] private OrientationGraph orientation = OrientationGraph.LeftRight;

        [SerializeField] [Tooltip("Amount of starting nodes.")]
        private int amountStart = 1;

        [SerializeField] [Tooltip("Amount of ending nodes.")]
        private int amountEnd = 1;

        [SerializeField] private bool isPerfectSegmentLane;

        [SerializeField] private bool useAsync = true;

        [SerializeField] private int amountOfLaneConnections = 1;

        [SerializeField] private bool hasConnections = true;

        [Tooltip("Runtime debug data.")] public DebugData DebugValues;

        [SerializeField] private SortMethod sortMethod = SortMethod.Distance;

        [SerializeField] private TMP_Text debugDistanceText;

        public int Amount
        {
            get => amount;
            set => amount = value;
        }

        public Vector2 NodeWorldSize => nodeWorldSize;

        public Vector2 TotalWorldSize => totalWorldSize;

        public int Iterations => iterations;

        public int ParallelIterations => parallelIterations;

        public int Timeout => timeout;

        public int Seed
        {
            get => seed;
            set => seed = value;
        }

        public bool HasRandomSeed => hasRandomSeed;

        public OrientationGraph Orientation => orientation;

        public int AmountStart => amountStart;

        public int AmountEnd => amountEnd;

        public bool IsPerfectSegmentLane => isPerfectSegmentLane;

        public bool UseAsync => useAsync;

        public int AmountOfLaneConnections => amountOfLaneConnections;

        public SortMethod SortingMethod => sortMethod;

        public bool HasConnections => hasConnections;

        public TMP_Text DebugDistanceText => debugDistanceText;

        public WorldMapStaticData CreateData()
        {
            var center = totalWorldSize / 2;
            var bounds = new Rect(center, totalWorldSize + new Vector2(SMALL_NUMBER, SMALL_NUMBER));
            bounds.center = center;
            return new WorldMapStaticData(this, bounds);
        }


        [Button]
        public void Refresh()
        {
            FindFirstObjectByType<BaseWorldMapController>().Create();
            FindFirstObjectByType<MainCamera>().OnCreateWorldMap();
        }

        [Button]
        private void RefreshColors()
        {
            WorldMapGraphGizmos.colors.Clear();
        }

        [Serializable]
        public class DebugData
        {
            public enum DrawMode
            {
                None = 0,
                Nodes = 1,
                Graph = 2,
                Distances = 3,
                All = int.MaxValue,
            }

            public DrawMode Mode = DrawMode.All;
            public WorldMap.EDeletionReason DeletionReason = WorldMap.EDeletionReason.All;
        }
    }
}