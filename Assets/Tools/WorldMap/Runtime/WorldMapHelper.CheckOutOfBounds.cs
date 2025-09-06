namespace Tools.WorldMapCore.Runtime
{
    public static partial class WorldMapHelper
    {
        public static bool CheckBounds(Node node, WorldMapStaticData data)
        {
            var worldBounds = data.WorldBounds;
            var nodeBounds = node.Bounds;
            return CheckRectContains(worldBounds, nodeBounds);
        }
    }
}