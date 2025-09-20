namespace Tools.WorldMapCore.Runtime
{
    public partial class WorldMap
    {
        public enum EDeletionReason
        {
            None = 0,
            Overlap = 1,
            OutOfBounds = 2,

            All = int.MaxValue,
        }
    }
}