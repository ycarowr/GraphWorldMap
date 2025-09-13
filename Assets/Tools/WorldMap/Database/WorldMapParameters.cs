using System;
using Tools.Attributes;
using Tools.WorldMapCore.Runtime;
using UnityEngine;

namespace Tools.WorldMapCore.Database
{
    [CreateAssetMenu(menuName = "Database/WorldMap/Parameters")]
    public class WorldMapParameters : ScriptableObject
    {
        public enum Orientation
        {
            None = 0,
            LeftRight = 1,
            BottomTop = 2,
        }

        public const float SMALL_NUMBER = 0.0001f;

        [SerializeField] [Tooltip("Total amount of nodes that will be created.")]
        private int amount = 32;

        [SerializeField]
        [Tooltip(
            "Minimum distance necessary between two nodes in order to keep them alive. Use a negative value to ignore.")]
        private float isolationDistance = 10;

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

        [SerializeField] private Orientation orientation = Orientation.LeftRight;

        [SerializeField] [Tooltip("Amount of starting nodes.")]
        private int amountStart = 1;

        [SerializeField] [Tooltip("Amount of ending nodes.")]
        private int amountEnd = 1;

        [SerializeField]
        private bool isStartPartOfMainPath;

        [SerializeField]
        private bool isEndPartOfMainPath;
        
        [SerializeField]
        private bool isPerfectSegmentLane;

        [Tooltip("Runtime debug data.")] public DebugData DebugValues;

        public WorldMapStaticData CreateData()
        {
            var center = totalWorldSize / 2;
            var bounds = new Rect(center, totalWorldSize + new Vector2(SMALL_NUMBER, SMALL_NUMBER));
            bounds.center = center;
            return new WorldMapStaticData(
                amount,
                nodeWorldSize,
                isolationDistance,
                bounds,
                seed,
                iterations,
                parallelIterations,
                timeout,
                hasRandomSeed,
                DebugValues,
                orientation,
                amountStart,
                amountEnd,
                isStartPartOfMainPath,
                isEndPartOfMainPath,
                isPerfectSegmentLane);
        }

#if UNITY_EDITOR
        [Button]
        private void Refresh()
        {
            FindFirstObjectByType<BaseWorldMapController>().Create();
        }
#endif
        [Serializable]
        public class DebugData
        {
            public enum DrawMode
            {
                None = 0,
                Nodes = 1,
                Graph = 2,
                All = int.MaxValue,
            }
            public bool SelectOwnerOnCreate;
            public DrawMode Mode = DrawMode.All;
            public WorldMap.EDeletionReason DeletionReason = WorldMap.EDeletionReason.All;
        }
    }
}