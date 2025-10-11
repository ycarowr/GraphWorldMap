using Tools.Graphs;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public class WorldMapNode : BaseGraphNode, IBound
    {
        public readonly int ID;
        public int RegionID { get; private set; }

        public WorldMapNode(int id, Vector2 worldPosition, Vector2 size)
        {
            ID = id;
            var bound = new Rect(worldPosition, size);
            bound.center = worldPosition;
            bound.Sanitize();
            Bound = bound;
        }
        
        public Rect Bound { get; }

        public void SetRegion(int regionIndex)
        {
            RegionID = regionIndex;
        }
    }
}