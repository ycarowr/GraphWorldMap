namespace Tools.WorldMapCore.Database
{
    /// <summary>
    ///  Reason why a node was not generated.
    /// </summary>
    public enum EDeletionReason
    {
        None = 0,
        Overlap = 1,
        OutOfWorldBounds = 2,
        OutOfRegionBounds = 3,

        All = int.MaxValue,
    }
}