using Database.WorldMap;
using Unity.Profiling;
using UnityEngine;

namespace Runtime.WorldMap
{
    public class WorldMapController : MonoBehaviour
    {
        private static readonly ProfilerMarker ProfileMarker = new("WorldMapController::WorldMapController");
        private Tools.WorldMapCreation.WorldMap worldMap;
        [SerializeField] private GameObject worldMapRoot;
        [SerializeField] private WorldMapNode worldMapNodePrefab;
        [SerializeField] private WorldMapParameters worldMapParameters;

        private void Awake()
        {
            ProfileMarker.Begin();
            worldMapRoot = new GameObject("WorldMap");
            worldMapRoot.transform.position = -worldMapParameters.WorldMapSize / 2;
            worldMap = worldMapParameters.GenerateDungeon();
            ProfileMarker.End();
               
            CreateNodes();
        }

        private void CreateNodes()
        {
            var count = worldMap.Nodes.Count;
            for (var index = 0; index < count; ++index)
            {
                var node = worldMap.Nodes[index];
                var worldMapNode = Instantiate(worldMapNodePrefab, worldMapRoot.transform);
                worldMapNode.name = "Node_" + index;
                worldMapNode.SetNode(node);
            }
        }
    }
}