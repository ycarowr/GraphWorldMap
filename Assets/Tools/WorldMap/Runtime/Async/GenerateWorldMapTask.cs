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
            OnStart += () => { Debug.Log("Generation Started!"); };

            OnComplete += () =>
            {
                HasCompleted = true;
                Debug.Log("Generation Completed!");
            };
        }

        private bool HasStarted { get; set; }
        private bool HasCompleted { get; set; }

        private bool HasValue { get; set; }
        
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
                return;
            }

            var worldMapInstance = new WorldMap(Data);
            var amount = Data.Parameters.Amount;
            worldMapInstance.GenerateRegions();
            worldMapInstance.GenerateNodes();
            worldMapInstance.GenerateGraph();

            if (HasCompleted || HasValue)
            {
                return;
            }
            
            // if this is the ideal number we return it
            var currentAmount = worldMapInstance.Nodes.Count;
            if (currentAmount == amount)
            {
                HasValue = true;
                Debug.Log($"Iteration index: {index} Seed:{worldMapInstance.Random.Seed} Perfect!");
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
                Debug.Log($"Iteration index: {index} Seed:{NearIdealWorldMap.Random.Seed} Delta: {delta}");
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

            if (!Data.ValidateTotalArea())
            {
                Debug.LogError("The requested amount of nodes is too large to fit in the area.");
                return;
            }

            if (!Data.ValidateStarting())
            {
                Debug.LogError("The amount of Start nodes has to be greater than zero.");
                return;
            }

            if (!Data.ValidateEnding())
            {
                Debug.LogError("The amount of End nodes has to be greater than zero.");
                return;
            }

            if (!Data.ValidateAmount())
            {
                Debug.LogError("The amount of Start plus End nodes is larger than the total amount of nodes.");
                return;
            }
            
            if (!Data.Parameters.UseAsync)
            {
                Debug.Log("Synchronous call. Dispatching single iteration...");
                // If we don't use async we dispatch a single iteration and hope for the best.
                GenerateWorldMapIteration(0);
                OnComplete.Invoke();
                return;
            }

            var parallelIterations = Data.Parameters.ParallelIterations;
            Debug.Log($"Asynchronous call. Dispatching {parallelIterations} parallel iterations...");
            // If we are using async. We schedule the tasks.
            for (var i = 0; i < parallelIterations; i++)
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