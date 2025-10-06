using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public class WorldMapRegion : IBound
    {
        public readonly Rect Bounds;

        public WorldMapRegion(Vector2 worldPosition, Vector2 size)
        {
            Bounds = new Rect(worldPosition, size);
            Bounds.center = worldPosition;
            Bounds.Sanitize();
        }

        public Rect Bound => Bounds;
    }
}