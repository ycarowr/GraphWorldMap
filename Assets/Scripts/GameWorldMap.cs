using Tools.WorldMapCore.Database;
using Tools.WorldMapCore.Runtime;
using UnityEngine;

namespace Game
{
    public class GameWorldMap : BaseWorldMapController<GameWorldMapNode, WorldMapParameters>
    {
        protected override void OnRefreshMap()
        {
            base.OnRefreshMap();
            foreach (var start in WorldMap.Start)
            {
                foreach (var end in WorldMap.End)
                {
                    Debug.DrawLine(start.Center, end.Center, Color.cyan, 10000);
                }
            }
        }
    }
}