namespace AUS.DataStructures.KDTree;

internal class KDTreeDimensionalStack<TKey, TData> where TKey : IKDTreeKeyComparable<TKey>
{
    private readonly List<KDTreeDimensionalStackItem<TKey, TData>> _stack = new();
    
    public void Push(KDTreeNode<TKey, TData> node, int dimension)
    {
        _stack.Add(new KDTreeDimensionalStackItem<TKey, TData>
        {
            Node = node,
            Dimension = dimension
        });
    }
    
    public (KDTreeNode<TKey, TData> Node, int Dimension) Pop()
    {
        var item = _stack[_stack.Count - 1];
        _stack.RemoveAt(_stack.Count - 1);
        return (item.Node, item.Dimension);
    }
    
    public bool Any()
    {
        return _stack.Any();
    }
}

internal class KDTreeDimensionalStackItem<TKey, TData> where TKey : IKDTreeKeyComparable<TKey>
{
    public KDTreeNode<TKey, TData> Node { get; set; }
    public int Dimension { get; set; }
}
