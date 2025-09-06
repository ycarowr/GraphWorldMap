using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public static partial class WorldMapHelper
    {
        public static void GenerateSeed(WorldMapStaticData data)
        {
            var seed = data.HasRandomSeed ? Random.Range(0, data.MaxSeed) : data.Seed;
            Random.InitState(seed);
        }
    }
}