using System;
using Tools.Async;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public class GenerateWorldMapTask : TaskGroup
    {
        public GenerateWorldMapTask(WorldMapStaticData data, Action onComplete) : base(null, onComplete)
        {
            Data = data;
            OnComplete += () => HasCompleted = true;
        }

        public bool HasStarted { get; set; }
        public bool HasCompleted { get; set; }

        // Cached value of the perfect amount requested. Can be null in the end of the async process.
        private WorldMap PerfectWorldMap { get; set; }

        // Cached value of the nearest amount requested. Can be null in the end of the async process.
        private WorldMap NearIdealWorldMap { get; set; }
        private int NearIdealValue { get; set; } = int.MaxValue;

        private WorldMapStaticData Data { get; }

        private void GenerateWorldMapStep(int index)
        {
            if (PerfectWorldMap != null)
            {
                //Debug.Log($"Skipped {index}");
                return;
            }

            var worldMapInstance = new WorldMap(Data);
            var amount = Data.Amount;
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

        public void Dispatch()
        {
            if (HasStarted)
            {
                return;
            }

            HasStarted = true;
            ResetData();

            if (Data.ValidateTotalArea())
            {
                Debug.LogError("The requested amount of nodes is too large to fit in the area.");
                return;
            }

            for (var i = 0; i < Data.ParallelIterations; i++)
            {
                var index = i;
                AddTask(() => GenerateWorldMapStep(index));
            }

            ExecuteAll();
        }

        private void ResetData()
        {
            Clear();
            PerfectWorldMap = null;
            NearIdealWorldMap = null;
            NearIdealValue = int.MaxValue;
        }

        public WorldMap GetWorldMap()
        {
            return PerfectWorldMap ?? NearIdealWorldMap;
        }
    }
}