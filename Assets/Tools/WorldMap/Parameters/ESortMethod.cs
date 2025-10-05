namespace Tools.WorldMapCore.Database
{
    /// <summary>
    /// Sorting method is used to order the nodes in the graph. 
    /// </summary>
    public enum ESortMethod
    {
        None = 0,
        Axis = 1, // the order of the nodes is created based on the X or Y position.
        Distance = 2, // the order of the nodes is created based on the distance to the end point.
    }
}