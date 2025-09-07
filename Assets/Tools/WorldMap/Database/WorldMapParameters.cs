using System;
using Tools.Attributes;
using Tools.WorldMapCore.Runtime;
using UnityEngine;

namespace Tools.WorldMapCore.Database
{
    [CreateAssetMenu(menuName = "Database/WorldMap/Parameters")]
    public class WorldMapParameters : ScriptableObject
    {
        private const float FRACTION_THRESHOLD = 0.0001f;

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

        [SerializeField] [Tooltip("Seed value used for generation of the map.")]
        private int seed;

        [SerializeField] [Tooltip("Will the seed be used for generation of the map.")]
        private bool hasRandomSeed = true;

        [SerializeField] [Tooltip("Runtime debug data.")]
        private DebugData DebugValues;

        public WorldMapStaticData CreateData()
        {
            var center = totalWorldSize / 2;
            var bounds = new Rect(center, totalWorldSize + new Vector2(FRACTION_THRESHOLD, FRACTION_THRESHOLD));
            bounds.center = center;
            return new WorldMapStaticData(
                amount,
                nodeWorldSize,
                isolationDistance,
                bounds,
                seed,
                iterations,
                parallelIterations,
                hasRandomSeed,
                DebugValues);
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
            public bool DrawGizmos = true;
            public WorldMap.EDeletionReason DeletionReason = WorldMap.EDeletionReason.All;
        }
    }
}