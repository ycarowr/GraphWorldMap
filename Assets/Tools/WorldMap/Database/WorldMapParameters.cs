using System;
using Tools.Async;
using Tools.Attributes;
using Tools.WorldMapCore.Runtime;
using UnityEngine;

namespace Tools.WorldMapCore.Database
{
    [CreateAssetMenu(menuName = "Database/WorldMap/Parameters")]
    public class WorldMapParameters : ScriptableObject
    {
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

        [SerializeField] private DebugData DebugValues;
        private bool IsProcessing;
        private int NearIdealValue = int.MaxValue;
        private WorldMap NearIdealWorldMap;

        private TaskGroup ParallelTaskGroup;
        private WorldMap PerfectWorldMap;

        public int Iterations => iterations;

        public WorldMap GetWorldMap()
        {
            return PerfectWorldMap ?? NearIdealWorldMap;
        }

        private WorldMapStaticData CreateData()
        {
            var center = totalWorldSize / 2;
            var bounds = new Rect(center, totalWorldSize);
            bounds.center = center;
            return new WorldMapStaticData(
                amount,
                nodeWorldSize,
                isolationDistance,
                bounds,
                seed,
                iterations,
                hasRandomSeed,
                DebugValues);
        }

        public void GenerateWorldMap(Action OnComplete)
        {
            if (IsProcessing)
            {
                return;
            }

            IsProcessing = true;
            ResetData();

            if (ValidateParameters())
            {
                Debug.LogError("The requested amount of nodes is too large to fit in the area.");
                return;
            }

            ParallelTaskGroup?.Clear();
            ParallelTaskGroup = new TaskGroup(null, OnFinish);
            for (var i = 0; i < parallelIterations; i++)
            {
                var index = i;
                ParallelTaskGroup.AddTask(() => GenerateWorldMap(index));
            }

            ParallelTaskGroup.ExecuteAll();
            return;

            void OnFinish()
            {
                IsProcessing = false;
                OnComplete?.Invoke();
            }
        }

        private bool ValidateParameters()
        {
            var totalArea = totalWorldSize.x * totalWorldSize.y;
            var nodeArea = nodeWorldSize.x * nodeWorldSize.y;
            var totalNodeArea = nodeArea * amount;
            return totalNodeArea > totalArea;
        }

        private void ResetData()
        {
            PerfectWorldMap = null;
            NearIdealWorldMap = null;
            NearIdealValue = int.MaxValue;
        }

        private void GenerateWorldMap(int index)
        {
            if (PerfectWorldMap != null)
            {
                //Debug.Log($"Skipped {index}");
                return;
            }

            var worldData = CreateData();
            var worldMapInstance = new WorldMap(worldData);
            worldMapInstance.GenerateNodes();

            // if this is the ideal number we return it
            var currentAmount = worldMapInstance.Nodes.Count;
            if (currentAmount == amount)
            {
                Debug.Log($"Executed Parallel: {index} seed:{worldMapInstance.Random.Seed}");
                PerfectWorldMap = worldMapInstance;
                return;
            }

            // if not, we compare to with near ideal and perhaps keep it
            var delta = Mathf.Abs(amount - currentAmount);
            if (delta < NearIdealValue)
            {
                NearIdealValue = delta;
                NearIdealWorldMap = worldMapInstance;
                Debug.Log($"Near: Parallel: {index} seed:{NearIdealWorldMap.Random.Seed} delta: {delta}");
            }
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