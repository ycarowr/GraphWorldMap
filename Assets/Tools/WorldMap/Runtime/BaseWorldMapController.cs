using Tools.WorldMapCore.Database;
using Unity.Profiling;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public abstract class BaseWorldMapController<TNode, TParameter> : MonoBehaviour
        where TNode : BaseWorldMapNode
        where TParameter : WorldMapParameters
    {
        protected static readonly ProfilerMarker ProfileMarker = new("WorldMapController::WorldMapController");
        [SerializeField] protected TNode WorldMapNodePrefab;
        [SerializeField] protected TParameter WorldMapParameters;
        protected WorldMap WorldMap;
        protected GameObject WorldMapRoot;

        protected virtual void Awake()
        {
            ProfileMarker.Begin();
            WorldMapRoot = new GameObject("WorldMap");
            WorldMapRoot.transform.position = -WorldMapParameters.WorldMapTotalWorldSize / 2;
            WorldMap = WorldMapParameters.GenerateWorldMap();
            ProfileMarker.End();

            CreateNodes();
        }

        protected virtual void CreateNodes()
        {
            var count = WorldMap.Nodes.Count;
            for (var index = 0; index < count; ++index)
            {
                var node = WorldMap.Nodes[index];
                var worldMapNode = Instantiate(WorldMapNodePrefab, WorldMapRoot.transform);
                worldMapNode.name = "Node_" + index;
                worldMapNode.SetNode(node);
            }
        }
    }
}