using Tools.Graphs;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public class WorldMapNode : BaseGraphNode
    {
        public readonly Rect Bounds;
        public readonly int ID;

        public WorldMapNode(int id, Vector2 worldPosition, Vector2 size)
        {
            ID = id;
            Bounds = new Rect(worldPosition, size);
            Bounds.center = worldPosition;
        }

        public Vector3 Center => Bounds.center;
        public Vector2 Size => Bounds.size;
    }
}