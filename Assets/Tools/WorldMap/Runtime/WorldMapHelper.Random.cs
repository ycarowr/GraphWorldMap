using UnityEngine;
using Random = System.Random;

namespace Tools.WorldMapCore.Runtime
{
    public static partial class WorldMapHelper
    {
        private static Random RandomGenerator;

        public static void GenerateSeed(WorldMapStaticData data)
        {
            RandomGenerator = data.HasRandomSeed
                ? new Random()
                : new Random(data.Seed);
        }

        public static Vector2 GenerateRandomPosition(WorldMapStaticData data)
        {
            var bounds = data.WorldBounds;
            var randX = RandomGenerator.Next((int)bounds.xMin, (int)bounds.xMax);
            var randY = RandomGenerator.Next((int)bounds.yMin, (int)bounds.yMax);
            return new Vector2(randX, randY);
        }
    }
}