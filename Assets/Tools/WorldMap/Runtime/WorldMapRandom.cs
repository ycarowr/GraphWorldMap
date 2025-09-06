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
            var randX = RandomGenerator.NextDouble() * bounds.size.x;
            var randY = RandomGenerator.NextDouble() * bounds.size.y;
            return new Vector2((float)randX, (float)randY);
        }
    }
}