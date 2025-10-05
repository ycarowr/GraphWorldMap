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
        public enum EDeletionReason
        {
            None = 0,
            Overlap = 1,
            OutOfBounds = 2,

            All = int.MaxValue,
        }

        // Orientation is used to sort and display the graph.
        public enum EOrientationGraph
        {
            None = 0,
            LeftRight = 1, // the direction is Left to Right
            BottomTop = 2, // the direction is Bottom to Top
        }

        // Sorting method is used to order the nodes in the graph.
        public enum ESortMethod
        {
            None = 0,
            Axis = 1, // the order of the nodes is created based on the X or Y position.
            Distance = 2, // the order of the nodes is created based on the distance to the end point.
        }

        [SerializeField] private Region[] RegionParameters;

        [SerializeField]
        private Vector2 minRegionSize = Vector2.one;
        [SerializeField]
        private Vector2 maxRegionSize = Vector2.one;
        
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
        private bool isRandomSeed = true;

        [SerializeField] private EOrientationGraph orientation = EOrientationGraph.LeftRight;

        [SerializeField] [Tooltip("Amount of starting nodes.")]
        private int amountStart = 1;

        [SerializeField] [Tooltip("Amount of ending nodes.")]
        private int amountEnd = 1;

        [SerializeField] [Tooltip("Whether is using parallelism to generate the nodes.")]
        private bool useAsync = true;

        [SerializeField] [Tooltip("The number of connections.")]
        private int amountOfRegionConnections = 1;

        [Tooltip("Runtime debug data.")] public DebugData DebugValues;

        [SerializeField] [Tooltip("The direction in which the path is created.")]
        private ESortMethod sortMethod = ESortMethod.Distance;

        [SerializeField] private TMP_Text debugDistanceText;

        [SerializeField] private bool isAutoRegion = true;
        
        [SerializeField] private bool isAnimation = true;

        public bool IsAnimation
        {
            get => isAnimation;
            set => isAnimation = value;
        }

        public bool IsAutoRegion => isAutoRegion;

        public Region[] Regions
        {
            get => RegionParameters;
            set => RegionParameters = value;
        }

        public int Amount
        {
            get => amount;
            set => amount = value;
        }

        public Vector2 NodeWorldSize
        {
            get => nodeWorldSize;
            set => nodeWorldSize = value;
        }

        public Vector2 TotalWorldSize
        {
            get => totalWorldSize;
            set => totalWorldSize = value;
        }

        public int Iterations => iterations;

        public int ParallelIterations => parallelIterations;

        public int Timeout => timeout;

        public int Seed
        {
            get => seed;
            set => seed = value;
        }

        public bool IsRandomSeed
        {
            get => isRandomSeed;
            set => isRandomSeed = value;
        }

        public EOrientationGraph Orientation
        {
            get => orientation;
            set => orientation = value;
        }

        public int AmountStart
        {
            get => amountStart;
            set => amountStart = value;
        }

        public int AmountEnd
        {
            get => amountEnd;
            set => amountEnd = value;
        }

        public bool UseAsync
        {
            get
            {
#if UNITY_WEBGL_API && !UNITY_EDITOR
                // I didn't manage to make it work for WebGL builds.
                return false;
#endif
                return useAsync;
            }
        }

        public int AmountOfRegionConnections
        {
            get => amountOfRegionConnections;
            set => amountOfRegionConnections = value;
        }

        public ESortMethod SortingMethod
        {
            get => sortMethod;
            set => sortMethod = value;
        }
        
        public float FontSize
        {
            get
            {
                const float defaultFontSize = 16f;
                const float default100x100area = 100;
                const float defaultFactorArea = defaultFontSize / default100x100area;
                var area = TotalWorldSize.x * TotalWorldSize.y;
                return Mathf.Sqrt(area) * defaultFactorArea;
            }
        }

        public float LineSize
        {
            get
            {
                const float defaultLineSize = 0.15f;
                const float default100x100area = 100;
                const float defaultFactorArea = defaultLineSize / default100x100area;
                var area = TotalWorldSize.x * TotalWorldSize.y;
                return Mathf.Sqrt(area) * defaultFactorArea;
            }
        }

        public TMP_Text DebugDistanceText => debugDistanceText;

        public WorldMapStaticData CreateData()
        {
            var center = totalWorldSize / 2;
            var bounds = new Rect(center, totalWorldSize); // + WorldMapStaticData.SMALL_VECTOR);
            bounds.center = center;
            return new WorldMapStaticData(this, bounds);
        }


        [Button]
        public void Create()
        {
            FindFirstObjectByType<BaseWorldMapController>().Create();
        }

        [Button]
        private void RefreshColors()
        {
            WorldMapGraphGizmos.Colors.Clear();
        }

        [Serializable]
        public class Region
        {
            public Rect Bounds;
        }

        [Serializable]
        public class DebugData
        {
            public enum EDrawMode
            {
                None = 0,
                Nodes = 1,
                Graph = 2,
                Distances = 3,
                All = int.MaxValue,
            }

            public EDrawMode Mode = EDrawMode.All;
            public EDeletionReason DeletionReason = EDeletionReason.All;
        }
    }
}