using System;
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

    // Base generalized class for the map controller
    [ExecuteAlways]
    public abstract class BaseWorldMapController<TNode, TParameter>
        : BaseWorldMapController
        where TNode : BaseWorldMapNode
        where TParameter : WorldMapParameters
    {
        [SerializeField] protected TNode WorldMapNodePrefab;
        [SerializeField] protected TParameter WorldMapParameters;

        // Current instance of the world map.
        public WorldMap WorldMap { get; private set; }

        // Root transform which the nodes are instantiated.
        protected GameObject WorldMapRoot { get; private set; }

        private GenerateWorldMapTask GenerateWorldMapTask { get; set; }

        protected void Start()
        {
            Create();
        }

        protected virtual void OnEnable()
        {
            OnCreate += OnRefreshMap;
        }

        protected void OnDisable()
        {
            OnCreate -= OnRefreshMap;
        }

        private void OnDrawGizmos()
        {
            WorldMap?.OnDrawGizmos();
        }

        /// <summary>
        ///     Dispatched once the World is Created.
        /// </summary>
        public event Action OnCreate = () => { };

        /// <summary>
        ///     Dispatched right after the World is Created.
        /// </summary>
        public event Action OnPostCreate = () => { };

        [Button]
        public override void Create()
        {
            Clean();
            SetupRoot();

            var data = WorldMapParameters.CreateData();
            GenerateWorldMapTask = new GenerateWorldMapTask(data, RefreshAsync);
            GenerateWorldMapTask.Dispatch();
        }

        [Button]
        public void Cancel()
        {
            GenerateWorldMapTask.Cancel();
        }

        [Button]
        protected virtual void OnRefreshMap()
        {
            if (WorldMap == null)
            {
                return;
            }

            BaseWorldMapNode.IndexColor = 0;
            var count = WorldMap.Nodes.Count;
            for (var index = 0; index < count; ++index)
            {
                var node = WorldMap.Nodes[index];
                var worldMapNode = Instantiate(WorldMapNodePrefab, WorldMapRoot.transform);
                worldMapNode.name = "Node_" + index;
                if (WorldMap.Start.Contains(node))
                {
                    worldMapNode.IsStarting = true;
                }

                if (WorldMap.End.Contains(node))
                {
                    worldMapNode.IsEnding = true;
                }

                worldMapNode.SetNode(node);
            }

            if (Application.isPlaying)
            {
                Lines.Instance.Clear();
                WorldMap?.OnDrawGizmos();
            }

            WorldMapGraphGizmos.DrawTextDistance(WorldMap.GraphsRegistry, WorldMap.Data, WorldMapRoot);
            Debug.Log("Refresh Map");
            OnPostCreate?.Invoke();
        }

        private void SetupRoot()
        {
            WorldMapRoot = new GameObject("WorldMap");
            WorldMapRoot.transform.SetParent(transform);
        }

        private void Clean()
        {
            WorldMap = null;

            for (var i = 0; i < transform.childCount; i++)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }

            DestroyImmediate(WorldMapRoot);
        }

        private void RefreshAsync()
        {
            WorldMap = GenerateWorldMapTask.GetWorldMap();
            OnCreate.Invoke();
            Debug.Log($"Refresh Async: {WorldMap.Random.Seed}");
        }
    }
}