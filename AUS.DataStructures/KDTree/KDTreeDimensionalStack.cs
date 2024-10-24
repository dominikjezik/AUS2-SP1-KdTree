namespace AUS.DataStructures.KDTree;

internal class KDTreeDimensionalStack<TKey, TData> where TKey : IKDTreeKeyComparable<TKey>
{
    private readonly LinkedList<KDTreeNodeWithDimension<TKey, TData>> _stack = new();
    
    public void Push(KDTreeNode<TKey, TData> node, int dimension)
    {
        _stack.AddLast(new KDTreeNodeWithDimension<TKey, TData>(node, dimension));
    }
    
    public (KDTreeNode<TKey, TData> Node, int Dimension) Pop()
    {
        var item = _stack.Last.Value;
        _stack.RemoveLast();
        return (item.Node, item.Dimension);
    }
    
    public bool Any()
    {
        return _stack.Any();
    }
}
