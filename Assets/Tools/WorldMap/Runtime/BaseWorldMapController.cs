using Tools.Attributes;
using Tools.WorldMapCore.Database;
using Unity.Profiling;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public abstract class BaseWorldMapController : MonoBehaviour
    {
        public abstract void Create();
    }

    public abstract class BaseWorldMapController<TNode, TParameter> : BaseWorldMapController
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
            Create();
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            WorldMap?.OnDrawGizmos();
        }
#endif

        [Button]
        public override void Create()
        {
            ProfileMarker.Begin();
            DestroyImmediate(WorldMapRoot);
            WorldMapRoot = new GameObject("WorldMap");
            WorldMapRoot.transform.SetParent(transform);
            WorldMap = WorldMapParameters.GenerateWorldMap();
            ProfileMarker.End();

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