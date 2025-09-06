using Tools.WorldMapCore.Runtime;
using UnityEngine;

namespace Tools.WorldMapCore.Database
{
    [CreateAssetMenu(menuName = "Database/WorldMap/Parameters")]
    public class WorldMapParameters : ScriptableObject
    {
        [SerializeField] private int Amount = 32;
        [SerializeField] private float MinDistance = 10;
        [SerializeField] private Vector2 RoomSize = Vector2.one;
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
            return new WorldMapStaticData(Amount, RoomSize, MinDistance, bounds, seed, useRandomSeed);
        }

        public WorldMap GenerateDungeon()
        {
            WorldMap nearestWorldMap = null;
            var distance = int.MaxValue;
            for (var index = 0; index < iterations; index++)
            {
                var dungeonData = CreateData();
                var dungeon = new WorldMap(dungeonData);
                dungeon.Create();

                var currentAmount = dungeon.Nodes.Count;
                if (currentAmount == Amount)
                {
                    return dungeon;
                }

                var currentDistance = Mathf.Abs(Amount - currentAmount);
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    nearestWorldMap = dungeon;
                }
            }

            return nearestWorldMap;
        }
    }
}