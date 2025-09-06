using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public static partial class WorldMapHelper
    {
        public static Vector2 GenerateRandomPosition(WorldMapStaticData data)
        {
            var bounds = data.WorldBounds;
            var randX = Random.Range(bounds.xMin, bounds.xMax);
            var randY = Random.Range(bounds.yMin, bounds.yMax);
            return new Vector2(randX, randY);
        }
    }
}