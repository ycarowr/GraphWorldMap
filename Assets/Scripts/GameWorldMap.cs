using Tools.WorldMapCore.Database;
using Tools.WorldMapCore.Runtime;

namespace Game
{
    public class GameWorldMap : BaseWorldMapController<GameWorldMapNode, WorldMapParameters>
    {
#if UNITY_EDITOR
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            FindFirstObjectByType<BaseWorldMapController>()?.Create();
        }
#endif
    }
}