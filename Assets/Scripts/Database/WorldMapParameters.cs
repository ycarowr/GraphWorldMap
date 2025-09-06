using Tools.WorldMapCreation;
using UnityEngine;

namespace Database.WorldMap
{
    [CreateAssetMenu(menuName = "Database/WorldMap/Parameters")]
    public class WorldMapParameters : ScriptableObject
    {
        [SerializeField] private int Amount = 32;
        [SerializeField] private float MinDistance = 10;
        [SerializeField] private Vector2 RoomSize = Vector2.one;
        [SerializeField] private Vector2 Size = new Vector2(90, 60);
        [SerializeField] private int iterations = 65535;
        public Vector2 WorldMapSize => Size;

        private WorldMapStaticData CreateData()
        {
            var center = Size / 2;
            var bounds = new Rect(center, Size);
            bounds.center = center;
            return new WorldMapStaticData(Amount, RoomSize, MinDistance, bounds);
        }

        public Tools.WorldMapCreation.WorldMap GenerateDungeon()
        {
            Tools.WorldMapCreation.WorldMap nearestWorldMap = null;
            var distance = int.MaxValue;
            for (var index = 0; index < iterations; index++)
            {
                var dungeonData = CreateData();
                var dungeon = new Tools.WorldMapCreation.WorldMap(dungeonData);
                dungeon.Create();

                var currentAmount = dungeon.Nodes.Count;
                if (currentAmount == Amount)
                {
                    Debug.Log("Exact");
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