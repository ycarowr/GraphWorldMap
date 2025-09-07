using Tools.Attributes;
using Tools.WorldMapCore.Database;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    // Base non-generic class for the map controller
    public abstract class BaseWorldMapController : MonoBehaviour
    {
        public abstract void Create();
    }

    [ExecuteInEditMode]
    // Base generalized class for the map controller
    public abstract class BaseWorldMapController<TNode, TParameter>
        : BaseWorldMapController
        where TNode : BaseWorldMapNode
        where TParameter : WorldMapParameters
    {
        [SerializeField] protected TNode WorldMapNodePrefab;
        [SerializeField] protected TParameter WorldMapParameters;

        // Current instance of the world map.
        protected WorldMap WorldMap { get; private set; }

        // Root transform which the nodes are instantiated.
        protected GameObject WorldMapRoot { get; private set; }

        private GenerateWorldMapTask GenerateWorldMapTask { get; set; }

        private bool HasRefreshed { get; set; } = false;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            WorldMap?.OnDrawGizmos();
        }
#endif

        [Button]
        public override void Create()
        {
            Clean();
            SetupRoot();
            
            var data = WorldMapParameters.CreateData();
            GenerateWorldMapTask = new GenerateWorldMapTask(data, RefreshAsync);
            GenerateWorldMapTask.Dispatch();
        }

        private void SetupRoot()
        {
            WorldMapRoot = new GameObject("WorldMap");
            WorldMapRoot.transform.SetParent(transform);
        }

        private void Clean()
        {
            WorldMap = null;
            if (WorldMapRoot)
            {
                DestroyImmediate(WorldMapRoot);
            }
        }

        private void RefreshAsync()
        {
            WorldMap = GenerateWorldMapTask.GetWorldMap();
            HasRefreshed = true;
        }

        [Button]
        private void RefreshMap()
        {
            if (WorldMap == null)
            {
                return;
            }
            
            var count = WorldMap.Nodes.Count;
            for (var index = 0; index < count; ++index)
            {
                var node = WorldMap.Nodes[index];
                var worldMapNode = Instantiate(WorldMapNodePrefab, WorldMapRoot.transform);
                worldMapNode.name = "Node_" + index;
                worldMapNode.SetNode(node);
            }

            Debug.Log($"RefreshAsync: {WorldMap.Random.Seed} {WorldMapRoot.transform.childCount}");
        }

        
        private void Update()
        {
            if (HasRefreshed)
            {
                HasRefreshed = false;
                // Executing refresh on update due 
                // to main thread operations
                RefreshMap();
            }
        }
    }
}