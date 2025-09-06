namespace Tools.WorldMapCore.Runtime
{
    public static partial class WorldMapHelper
    {
        private static int NodeID;

        public static int GenerateID()
        {
            return ++NodeID;
        }

        public static void ResetID()
        {
            NodeID = 0;
        }
    }
}