using UnityEngine;
using Random = System.Random;

namespace Tools.WorldMapCore.Runtime
{
    public class WorldMapRandom
    {
        public readonly Random RandomGenerator;
        public readonly int Seed;

        public WorldMapRandom(WorldMapStaticData data)
        {
            var newRandom = new Random();
            Seed = data.HasRandomSeed
                ? newRandom.Next()
                : data.Seed;
            RandomGenerator = new Random(Seed);
        }

        public Vector2 GenerateRandomPosition(WorldMapStaticData data)
        {
            var bounds = data.WorldBounds;
            var randX = RandomGenerator.Next((int)bounds.xMin, (int)bounds.xMax);
            var randY = RandomGenerator.Next((int)bounds.yMin, (int)bounds.yMax);
            return new Vector2(randX, randY);
        }
    }
}