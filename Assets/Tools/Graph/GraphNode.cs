namespace Tools.Graphs
{
    public abstract class GraphNode<T> : BaseGraphNode
    {
        protected GraphNode(T data)
        {
            Data = data;
        }

        public T Data { get; set; }
    }
}