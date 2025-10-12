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
            Seed = data.Parameters.IsRandomSeed
                ? newRandom.Next()
                : data.Parameters.Seed;
            RandomGenerator = new Random(Seed);
        }

        public Vector2 GenerateRandomWorldPosition(WorldMapStaticData data)
        {
            var bounds = data.WorldBounds;
            var randX = RandomGenerator.NextDouble() * bounds.size.x;
            var randY = RandomGenerator.NextDouble() * bounds.size.y;
            return new Vector2((float)randX, (float)randY);
        }

        public float GenerateRandomBetweenMinMax(float min, float max)
        {
            var delta = max - min;
            return (float)RandomGenerator.NextDouble() * delta + min;
        }
    }
}