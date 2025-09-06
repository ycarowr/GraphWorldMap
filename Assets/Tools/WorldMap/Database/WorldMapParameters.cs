using Tools.WorldMapCore.Runtime;
using UnityEngine;

namespace Tools.WorldMapCore.Database
{
    [CreateAssetMenu(menuName = "Database/WorldMap/Parameters")]
    public class WorldMapParameters : ScriptableObject
    {
        [SerializeField] private int Amount = 32;
        [SerializeField] private float MinDistance = 10;
        [SerializeField] private Vector2 NodeSize = Vector2.one;
        [SerializeField] private Vector2 Size = new(90, 60);
        [SerializeField] private int iterations = 65535;
        [SerializeField] private int seed = 1234;
        [SerializeField] private bool useRandomSeed = true;
        public Vector2 WorldMapSize => Size;

        private WorldMapStaticData CreateData()
        {
            var center = Size / 2;
            var bounds = new Rect(center, Size);
            bounds.center = center;
            return new WorldMapStaticData(Amount, NodeSize, MinDistance, bounds, seed, useRandomSeed);
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
                if (currentAmount == Amount)
                {
                    return worldMapInstance;
                }

                // if not, we compare to with near ideal and perhaps keep it
                var delta = Mathf.Abs(Amount - currentAmount);
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