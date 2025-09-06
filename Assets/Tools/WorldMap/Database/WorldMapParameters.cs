using Tools.WorldMapCore.Runtime;
using UnityEngine;

namespace Tools.WorldMapCore.Database
{
    [CreateAssetMenu(menuName = "Database/WorldMap/Parameters")]
    public class WorldMapParameters : ScriptableObject
    {
        [SerializeField][Tooltip("Total amount of nodes that will be created.")]
        private int amount = 32;
        [SerializeField][Tooltip("Minimum distance between each node.")]
        private float minDistance = 10;
        [SerializeField][Tooltip("Node size in world units from each node.")]
        private Vector2 nodeWorldSize = Vector2.one;
        [SerializeField][Tooltip("Total world map size in world units.")]
        private Vector2 totalWorldSize = new(90, 60);
        [SerializeField][Tooltip("Maximum amount of attempts to generate a map with the provided parameters.")]
        private int iterations = 65535;
        [SerializeField][Tooltip("Seed value used for generation of the map.")]
        private int seed = 1234;
        [SerializeField][Tooltip("Will the seed be used for generation of the map.")]
        private bool useRandomSeed = true;
        public Vector2 WorldMapTotalWorldSize => totalWorldSize;

        private WorldMapStaticData CreateData()
        {
            var center = totalWorldSize / 2;
            var bounds = new Rect(center, totalWorldSize);
            bounds.center = center;
            return new WorldMapStaticData(amount, nodeWorldSize, minDistance, bounds, seed, useRandomSeed);
        }

        public WorldMap GenerateWorldMap()
        {
            WorldMap nearIdealWorldMap = null;
            var nearIdealValue = int.MaxValue;
            for (var index = 0; index < iterations; index++)
            {
                var worldData = CreateData();
                var worldMapInstance = new WorldMap(worldData);
                worldMapInstance.GenerateNodes();

                // if this is the ideal number we return it
                var currentAmount = worldMapInstance.Nodes.Count;
                if (currentAmount == amount)
                {
                    return worldMapInstance;
                }

                // if not, we compare to with near ideal and perhaps keep it
                var delta = Mathf.Abs(amount - currentAmount);
                if (delta < nearIdealValue)
                {
                    nearIdealValue = delta;
                    nearIdealWorldMap = worldMapInstance;
                }
            }

            return nearIdealWorldMap;
        }
    }
}