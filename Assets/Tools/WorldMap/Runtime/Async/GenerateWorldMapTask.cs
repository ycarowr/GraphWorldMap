using System;
using Tools.Async;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public class GenerateWorldMapTask : TaskGroup
    {
        public GenerateWorldMapTask(WorldMapStaticData data, Action onComplete) : base(null, onComplete,
            data.Parameters.Timeout)
        {
            Data = data;
            OnStart += () => { Debug.Log("Parallel Iteration Started!"); };

            OnComplete += () =>
            {
                HasCompleted = true;
                Debug.Log("Parallel Iteration Completed!");
            };
        }

        public bool HasStarted { get; set; }
        public bool HasCompleted { get; set; }

        // Cached value of the perfect amount requested. Can be null in the end of the async process.
        private WorldMap PerfectWorldMap { get; set; }

        // Cached value of the nearest amount requested. Can be null in the end of the async process.
        private WorldMap NearIdealWorldMap { get; set; }
        private int NearIdealValue { get; set; } = int.MaxValue;

        private WorldMapStaticData Data { get; }

        private void GenerateWorldMapIteration(int index)
        {
            if (PerfectWorldMap != null)
            {
                //Debug.Log($"Skipped {index}");
                return;
            }

            var worldMapInstance = new WorldMap(Data);
            var amount = Data.Parameters.Amount;
            worldMapInstance.GenerateNodes();

            // if this is the ideal number we return it
            var currentAmount = worldMapInstance.Nodes.Count;
            if (currentAmount == amount)
            {
                Debug.Log($"Parallel Iteration index:{index} Seed:{worldMapInstance.Random.Seed} Perfect!");
                PerfectWorldMap = worldMapInstance;
                Cancel();
                return;
            }

            // if not, we compare to with near ideal and perhaps keep it
            var delta = Mathf.Abs(amount - currentAmount);
            if (delta < NearIdealValue)
            {
                NearIdealValue = delta;
                NearIdealWorldMap = worldMapInstance;
                Debug.Log($"Parallel Iteration index:{index} Seed:{NearIdealWorldMap.Random.Seed} Delta: {delta}");
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

            var iterations = Data.Parameters.IsRandomSeed ? Data.Parameters.ParallelIterations : 1;
            Debug.Log($"Dispatching Iterations. n = {iterations}");

            if (!Data.Parameters.UseAsync)
            {
                for (var index = 0; index < iterations; index++)
                {
                    GenerateWorldMapIteration(index);
                }

                OnComplete.Invoke();
                return;
            }

            for (var i = 0; i < iterations; i++)
            {
                var index = i;
                AddTask(() => GenerateWorldMapIteration(index));
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
            if (PerfectWorldMap != null)
            {
                return PerfectWorldMap;
            }

            return NearIdealWorldMap;
        }
    }
}