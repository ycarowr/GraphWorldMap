using Tools.WorldMapCore.Database;
using Tools.WorldMapCore.Runtime;
using UnityEngine;

namespace Game
{
    public class GameWorldMap : BaseWorldMapController<GameWorldMapNode, WorldMapParameters>
    {
#if UNITY_EDITOR
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            GameObject.FindFirstObjectByType<BaseWorldMapController>().Create();
        }
#endif
    }
}